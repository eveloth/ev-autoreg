using System.Text.RegularExpressions;
using EVAutoregConsole.Interfaces;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Configuration;
using static EVAutoregConsole.Auxiliary.PrettyPrinter;
using Task = System.Threading.Tasks.Task;

namespace EVAutoregConsole.App;

public class Exchange
{
    private readonly string _url;
    private readonly string _emailAddress;
    private readonly string _password;
    private readonly ExchangeService _exchangeService;
    private readonly IMailEventListener _listener;
    private readonly Rules _rules;
    private StreamingSubscriptionConnection? Connection { get; set; }

    public Exchange(IConfiguration config, IMailEventListener listener, Rules rules)
    {
        _url = config.GetValue<string>("ExchangeCredentials:URL");
        _emailAddress = config.GetValue<string>("ExchangeCredentials:EmailAddress");
        _password = config.GetValue<string>("ExchangeCredentials:Password");
        _exchangeService = this.CreateService();
        _listener = listener;
        _rules = rules;
    }

    public async Task StartService()
    {
        await CreateAndOpenConnection();
    }

    private async void OnNotificationEvent(object sender, NotificationEventArgs args)
    {
        foreach (var e in args.Events)
        {
            var notification = (ItemEvent) e;
            var email = await EmailMessage.Bind(_exchangeService, notification.ItemId);
            
            if (!Regex.IsMatch(email.Subject, _rules.RegexNewIssue))
            {
                Console.WriteLine("Received an email that we won't process.");
                continue;
            }
            
            PrintNotification($"\n{DateTime.Now}\n------New Mail:------", ConsoleColor.Blue);
            Console.WriteLine(email.Subject);
            await _listener.ProcessEvent(email);
        }
    }

    private async void OnSubscriptionError(object sender, SubscriptionErrorEventArgs args)
    {
        PrintNotification("Subscription error occurred. Trying to resubscribe...", ConsoleColor.DarkYellow);

        if (Connection is null)
        {
            await CreateAndOpenConnection();
        }
        else
        {
            try
            {
                if (Connection.IsOpen) Connection.Close();

                var subscription = await NewMailSubscription();

                Connection.AddSubscription(subscription);
                Connection.Open();

                PrintConnectionStatus(Connection);
            }
            catch (Exception)
            {
                PrintNotification("Could not restore subscription, exiting.", ConsoleColor.Red);
                throw;
            } 
        }
    }

    private async void OnDisconnect(object sender, SubscriptionErrorEventArgs args)
    {
        PrintNotification("\n------<Disconnected.>------", ConsoleColor.DarkYellow);
        PrintNotification("Trying to reestablish a connection...", ConsoleColor.Yellow);

        if (Connection is null)
        {
            await CreateAndOpenConnection();
        }
        else
        {
            while (!Connection!.IsOpen)
            {
                try
                {
                    Connection.Open();
                    PrintConnectionStatus(Connection);
                }
                catch (Exception e)
                {
                    PrintNotification($"Could not reestablish a connection:\n{e.Message}\nTrying again in 3s...", ConsoleColor.Red);
                    await Task.Delay(3000);
                }
            } 
        }
    }

    private ExchangeService CreateService()
    {       
        return new ExchangeService
        {
            Url = new Uri($"https://{_url}/ews/exchange.asmx"),
            Credentials = new WebCredentials(_emailAddress, _password)
        };
    }

    private async Task<StreamingSubscription> NewMailSubscription()
    {
        StreamingSubscription subscription;

        try
        {
            subscription = await _exchangeService.SubscribeToStreamingNotifications(
                new FolderId[] { WellKnownFolderName.Inbox }, EventType.NewMail);
        }
        catch (Exception e)
        {
            PrintNotification("\nCouldn't authenticate against the Exchange Server.\n" +
            "Possible reasons are: invalid username and/or password, or domain was specified incorrectly.\n" +
            e.Message, ConsoleColor.Red);
            throw;
        }

        return subscription;
    }

    private async Task CreateAndOpenConnection()
    {
        var subscription = await NewMailSubscription();
        Connection = new StreamingSubscriptionConnection(_exchangeService, 20);

        Connection.OnNotificationEvent += OnNotificationEvent;
        Connection.OnSubscriptionError += OnSubscriptionError;
        Connection.OnDisconnect += OnDisconnect;
        Connection.AddSubscription(subscription);

        Connection.Open();
        PrintConnectionStatus(Connection);
    }
}
