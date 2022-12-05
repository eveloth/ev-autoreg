using Autoregistrar.Apis;
using Autoregistrar.Hubs;
using Autoregistrar.Settings;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Exchange.WebServices.Data;
using Task = System.Threading.Tasks.Task;
using static Autoregistrar.Settings.StateManager;

namespace Autoregistrar.Services;

public class MailEventListener : IMailEventListener
{
    private readonly ILogger<MailEventListener> _logger;
    private readonly IHubContext<AutoregistrarHub, IAutoregistrarClient> _hubContext;
    private ExchangeService Exchange { get; set; } = null!;
    private StreamingSubscriptionConnection? Connection { get; set; }

    public MailEventListener(
        ILogger<MailEventListener> logger,
        IHubContext<AutoregistrarHub, IAutoregistrarClient> hubContext
    )
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task OpenConnection(CancellationToken cts)
    {
        Exchange = ExchangeApi.CreateService();
        var subscription = await ExchangeApi.CreateStreamingSubscription(Exchange);
        Connection = new StreamingSubscriptionConnection(Exchange, 20);

        Connection.OnNotificationEvent += OnNotificationEvent;
        Connection.OnDisconnect += OnDisconnect;
        Connection.OnSubscriptionError += OnSubscriptionError;

        Connection.AddSubscription(subscription);
        Connection.Open();

        if (Connection.IsOpen)
        {
            _logger.LogInformation("A connection was opened for user ID {UserId}", StartedForUserId);
            await _hubContext.Clients.All.ReceiveLog(
                $"A connection was opened for user ID {StartedForUserId}"
            );
        }
    }

    public void CloseConnection()
    {
        Connection!.Close();
        Connection = null;
    }

    private async void OnNotificationEvent(object sender, NotificationEventArgs args)
    {
        foreach (var ev in args.Events)
        {
            var notification = (ItemEvent)ev;
            var email = await EmailMessage.Bind(Exchange, notification.ItemId);

            if (!StateManager.Settings!.AutoregistrarSettings.NewIssueRegex.IsMatch(email.Subject))
            {
                _logger.LogInformation(
                    "Received an email that is not a new issue notification, skipping"
                );
                await _hubContext.Clients.All.ReceiveLog(
                    "Received an email that is not a new issue notification, skipping"
                );
                continue;
            }

            var issueNo = StateManager.Settings.AutoregistrarSettings.IssueNoRegex
                .Match(email.Subject)
                .Groups[1].Value;
            _logger.LogInformation("Received new issue, ID {IssueNo}", issueNo);
            await _hubContext.Clients.All.ReceiveLog($"Received new issue, ID {issueNo}");
        }
    }

    private async void OnDisconnect(object sender, SubscriptionErrorEventArgs args)
    {
        if (StateManager.Status != Status.Started)
        {
            _logger.LogInformation(
                "The connection was gracefully closed for user ID {UserId}",
                StartedForUserId
            );
            await _hubContext.Clients.All.ReceiveLog(
                $"The connection was gracefully closed for user ID {StartedForUserId}"
            );
            return;
        }

        _logger.LogInformation(
            "The connection was automatically closed for user ID {UserId}, reopening connection",
            StartedForUserId
        );
        await _hubContext.Clients.All.ReceiveLog(
            $"The connection was automatically closed for user ID {StartedForUserId}, reopening connection"
        );

        Connection!.Open();

        if (Connection.IsOpen)
        {
            _logger.LogInformation(
                "A connection was opened for user ID {UserId}",
                StartedForUserId
            );
            await _hubContext.Clients.All.ReceiveLog($"A connection was opened for user ID {StartedForUserId}");
        }
    }

    private async void OnSubscriptionError(object sender, SubscriptionErrorEventArgs args)
    {
        _logger.LogError("A subscription error occured, details: {ErrorMessage}", args.Exception);
        await _hubContext.Clients.All.ReceiveLog($"A subscription error occured, details: {args.Exception}");
    }
}