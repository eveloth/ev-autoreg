using System.Text.RegularExpressions;
using Autoregistrar.Apis;
using Autoregistrar.Settings;
using Microsoft.Exchange.WebServices.Data;
using Task = System.Threading.Tasks.Task;

namespace Autoregistrar.App;

public class MailEventListener : IMailEventListener
{
    private readonly ILogger<MailEventListener> _logger;
    private readonly ExchangeApi _exchangeApi;

    private ExchangeService Exchange { get; set; }
    private StreamingSubscriptionConnection? Connection { get; set; }

    public MailEventListener(ExchangeApi exchangeApi, ILogger<MailEventListener> logger)
    {
        _exchangeApi = exchangeApi;
        _logger = logger;
    }

    public async Task OpenConnection(CancellationToken cts)
    {
        Exchange = _exchangeApi.CreateService();
        var subscription = await _exchangeApi.CreateStreamingSubscription(Exchange, cts);
        Connection = new StreamingSubscriptionConnection(Exchange, 20);

        Connection.OnNotificationEvent += OnNotificationEvent;
        Connection.OnDisconnect += OnDisconnect;
        Connection.OnSubscriptionError += OnSubscriptionError;

        Connection.AddSubscription(subscription);
        Connection.Open();
        Console.WriteLine("Connection open?" + Connection.IsOpen);
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
            var notification = (ItemEvent) ev;
            var email = await EmailMessage.Bind(Exchange, notification.ItemId);

            if (!Regex.IsMatch(email.Subject, StatusManager.Settings.AutoregSettings.NewIssueRegex))
            {
                Console.WriteLine("Won't process");
                continue;
            }

            Console.WriteLine(email.Subject);
        }
    }

    private async void OnDisconnect(object sender, SubscriptionErrorEventArgs args)
    {
        if (StatusManager.Status != Status.Started)
        {
            Console.WriteLine("Closed by user");
            return;
        }

        Console.WriteLine("Reopening connection");

        Connection!.Open();
        Console.WriteLine("Connection open?" + Connection.IsOpen);
    }

    private async void OnSubscriptionError(object sender, SubscriptionErrorEventArgs args)
    {
        Console.WriteLine("Subscription error");
    }
}