using Autoregistrar.Apis;
using Autoregistrar.Settings;
using Microsoft.Exchange.WebServices.Data;
using Task = System.Threading.Tasks.Task;
using static Autoregistrar.Settings.StateManager;

namespace Autoregistrar.App;

public class MailEventListener : IMailEventListener
{
    private readonly ILogger<MailEventListener> _logger;
    private ExchangeService Exchange { get; set; } = null!;
    private StreamingSubscriptionConnection? Connection { get; set; }

    public MailEventListener(ILogger<MailEventListener> logger)
    {
        _logger = logger;
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
        _logger.LogInformation("A connection was opened for user ID {UserId}", StartedForUserId);
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
                continue;
            }

            var issueNo = StateManager.Settings.AutoregistrarSettings.IssueNoRegex
                .Match(email.Subject)
                .Groups[1].Value;
            _logger.LogInformation("Received new issue, ID {IssueNo}", issueNo);
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
            return;
        }

        _logger.LogInformation(
            "The connection was automatically closed for user ID {UserId}, reopening connection",
            StartedForUserId
        );

        Connection!.Open();

        if (Connection.IsOpen)
        {
            _logger.LogInformation("A connection was opened for user ID {UserId}", StartedForUserId);
        }
    }

    private async void OnSubscriptionError(object sender, SubscriptionErrorEventArgs args)
    {
        _logger.LogError("A subscription error occured, details: {ErrorMessage}", args.Exception);
    }
}