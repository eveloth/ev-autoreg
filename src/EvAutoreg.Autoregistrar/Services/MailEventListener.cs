using EvAutoreg.Autoregistrar.Apis;
using EvAutoreg.Autoregistrar.Services.Interfaces;
using EvAutoreg.Autoregistrar.Settings;
using EvAutoreg.Autoregistrar.State;
using Microsoft.Exchange.WebServices.Data;
using Task = System.Threading.Tasks.Task;

namespace EvAutoreg.Autoregistrar.Services;

public class MailEventListener : IMailEventListener
{
    private readonly IIssueProcessor _issueProcessor;
    private readonly ILogDispatcher<MailEventListener> _logDispatcher;
    private ExchangeService _exchange = null!;
    private StreamingSubscriptionConnection? _connection;

    public MailEventListener(
        IIssueProcessor issueProcessor,
        ILogDispatcher<MailEventListener> logDispatcher
    )
    {
        _issueProcessor = issueProcessor;
        _logDispatcher = logDispatcher;
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
        foreach (var ev in args.Events)
        {
            var notification = (ItemEvent)ev;
            var email = await EmailMessage.Bind(_exchange, notification.ItemId);

            if (!GlobalSettings.AutoregistrarSettings!.NewIssueRegex.IsMatch(email.Subject))
            {
                await _logDispatcher.Log(
                    $"A connection was opened for user ID {StateManager.GetOperator()}"
                );
                continue;
            }

            var issueNo = GlobalSettings.AutoregistrarSettings.IssueNoRegex
                .Match(email.Subject)
                .Groups[1].Value;

            await _logDispatcher.Log($"Received new issue, ID {issueNo}");
            try
            {
                await _issueProcessor.ProcessEvent(issueNo);
            }
            catch (Exception)
            {
                await _logDispatcher.Log("Processing an issue resulted in an error");
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

        _connection = null;
        await CreateConnection();
        await OpenConnectionWithRetry(TimeSpan.FromSeconds(3));
    }

    private async void OnSubscriptionError(object sender, SubscriptionErrorEventArgs args)
    {
        await _logDispatcher.Log($"A subscription error occured, details: {args.Exception}");

        _connection = null;

        await CreateConnection();
        await OpenConnectionWithRetry(TimeSpan.FromSeconds(3));
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

    private async Task OpenConnectionWithRetry(TimeSpan delaySec)
    {
        while (!_connection!.IsOpen)
        {
            try
            {
                _connection.Open();

                if (_connection.IsOpen)
                {
                    await _logDispatcher.Log(
                        $"A connection was opened for user ID {StateManager.GetOperator()}"
                    );
                }
            }
            catch (Exception)
            {
                await _logDispatcher.Log($"Counldn't restore a connection, trying again in {delaySec:%s}s...");
                await Task.Delay(delaySec);
            }
        }
    }
}