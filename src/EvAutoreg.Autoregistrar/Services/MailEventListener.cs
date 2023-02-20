using EvAutoreg.Autoregistrar.Apis;
using EvAutoreg.Autoregistrar.Services.Interfaces;
using EvAutoreg.Autoregistrar.Settings;
using EvAutoreg.Autoregistrar.State;
using Microsoft.Exchange.WebServices.Data;
using Npgsql;
using Task = System.Threading.Tasks.Task;

namespace EvAutoreg.Autoregistrar.Services;

public class MailEventListener : IMailEventListener
{
    private readonly IIssueProcessor _issueProcessor;
    private readonly ILogger<MailEventListener> _logger;
    private readonly ILogDispatcher<MailEventListener> _logDispatcher;
    private ExchangeService _exchange = null!;
    private StreamingSubscriptionConnection? _connection;

    public MailEventListener(
        IIssueProcessor issueProcessor,
        ILogDispatcher<MailEventListener> logDispatcher,
        ILogger<MailEventListener> logger
    )
    {
        _issueProcessor = issueProcessor;
        _logDispatcher = logDispatcher;
        _logger = logger;
    }

    public async Task OpenConnection(CancellationToken cts)
    {
        _exchange = ExchangeApi.CreateService();
        await CreateConnection();

        _connection!.Open();

        if (_connection!.IsOpen)
        {
            await _logDispatcher.Log(
                $"A connection was opened for user ID {StateManager.GetOperator()}"
            );
        }
    }

    public void CloseConnection()
    {
        _connection?.Close();
        _connection = null;
    }

    private async void OnNotificationEvent(object sender, NotificationEventArgs args)
    {
        var newIssueRegex = GlobalSettings.AutoregistrarSettings!.NewIssueRegex;
        var issueNoRegex = GlobalSettings.AutoregistrarSettings.IssueNoRegex;

        foreach (var ev in args.Events)
        {
            var notification = (ItemEvent)ev;
            var email = await GetEmail(notification.ItemId);

            if (!newIssueRegex.IsMatch(email.Subject))
            {
                continue;
            }

            var issueNo = issueNoRegex.Match(email.Subject).Groups[1].Value;

            await _logDispatcher.Log($"Received new issue, ID {issueNo}");

            try
            {
                await _issueProcessor.ProcessEvent(issueNo);
            }
            catch (Exception e) when (e is EvApiException or NpgsqlException)
            {
                await _logDispatcher.Log($"Processing an issue {issueNo} resulted in an error");
                _logger.LogError(
                    "Processing an issue ID {IssueId} resulted in an error: {Error}",
                    issueNo,
                    e
                );
            }
            catch (HttpRequestException e)
            {
                await _logDispatcher.Log(
                    $"Processing an issue {issueNo} resulted in an error: couldn't connect to EV server"
                );
                _logger.LogError(
                    "Processing an issue ID {Issueid} resulted in an error: {Error}",
                    issueNo,
                    e
                );
            }
            catch (Exception e)
            {
                await _logDispatcher.Log($"Processing an issue {issueNo} resulted in an error");
                _logger.LogError(
                    "Processing an issue ID {Issueid} resulted in an error: {Error}",
                    issueNo,
                    e
                );
            }
        }
    }

    private async void OnDisconnect(object sender, SubscriptionErrorEventArgs args)
    {
        if (!StateManager.IsStarted())
        {
            await _logDispatcher.Log(
                $"The connection was gracefully closed for user ID {StateManager.GetOperator()}"
            );
            return;
        }

        await _logDispatcher.Log(
            $"The connection was automatically closed for user ID {StateManager.GetOperator()}, reopening connection"
        );

        await ReconnectWithRetry(TimeSpan.FromSeconds(3));
    }

    private async void OnSubscriptionError(object sender, SubscriptionErrorEventArgs args)
    {
        await _logDispatcher.Log("A subscription error occured, trying to recover...");
        _logger.LogError("A subscription error occured: {Error}", args.Exception);

        await ReconnectWithRetry(TimeSpan.FromSeconds(3));
    }

    private async Task ReconnectWithRetry(TimeSpan delaySec)
    {
        while (!ConnectionIsOpen())
        {
            if (!StateManager.IsStarted())
            {
                break;
            }

            try
            {
                await TryReconnect();
            }
            catch (Exception e)
            {
                await _logDispatcher.Log(
                    $"Counldn't restore a connection, trying again in {delaySec:%s}s..."
                );
                _logger.LogError("Could not open connection, error: {Error}", e);
                await Task.Delay(delaySec);
            }
        }
    }

    private async Task TryReconnect()
    {
        _connection = null;
        await CreateConnection();
        _connection!.Open();

        if (_connection.IsOpen)
        {
            await _logDispatcher.Log(
                $"A connection was opened for user ID {StateManager.GetOperator()}"
            );
        }
    }

    private async Task CreateConnection()
    {
        var subscription = await ExchangeApi.CreateStreamingSubscription(_exchange);
        _connection = new StreamingSubscriptionConnection(_exchange, 20);

        _connection.OnNotificationEvent += OnNotificationEvent;
        _connection.OnDisconnect += OnDisconnect;
        _connection.OnSubscriptionError += OnSubscriptionError;

        _connection.AddSubscription(subscription);
    }

    private bool ConnectionIsOpen()
    {
        return _connection?.IsOpen ?? false;
    }

    private async Task<EmailMessage> GetEmail(ItemId emailId)
    {
        EmailMessage email = null!;

        while (email is null)
        {
            try
            {
                email = await EmailMessage.Bind(_exchange, emailId);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    "Couldn't get an email from Exchange server: {ErrorMessage}, retrying...",
                    e.Message
                );
                await Task.Delay(500);
            }
        }

        return email;
    }
}