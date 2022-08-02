using System.Text.RegularExpressions;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Configuration;
using Task = System.Threading.Tasks.Task;
using static EVAutoreg.PrettyPrinter;

namespace EVAutoreg;

public class Exchange
{
    private readonly string _url;
    private readonly string _emailAddress;
    private readonly string _password;
    private readonly ExchangeService _exchangeService;
    private readonly IMailEventListener _listener;
    private StreamingSubscriptionConnection? Connection { get; set; }

    public Exchange(IConfiguration config, IMailEventListener listener)
    {
        _url = config.GetValue<string>("ExchangeCredentials:URL");
        _emailAddress = config.GetValue<string>("ExchangeCredentials:EmailAddress");
        _password = config.GetValue<string>("ExchangeCredentials:Password");
        _exchangeService = this.CreateService();
        _listener = listener;
    }

    public async Task StartService()
    {
        var subscription = await NewMailSubscribtion();
        Connection = new StreamingSubscriptionConnection(_exchangeService, 15);

        Connection.OnNotificationEvent += OnNotificationEvent;
        Connection.OnSubscriptionError += OnSubscriptionError;
        Connection.OnDisconnect += OnDisconnect;
        Connection.AddSubscription(subscription);
        
        Connection.Open();
        PrintConnectionStatus(Connection);
    }

    private async void OnNotificationEvent(object sender, NotificationEventArgs args)
    {
        var notification = (ItemEvent) args.Events.FirstOrDefault()!;
        var email = await EmailMessage.Bind(_exchangeService, notification.ItemId);

        if (!Regex.IsMatch(email.Subject, @"^\[.+\]: Новое"))
        {
            Console.WriteLine("Recieved an email that we won't process.");
            return;
        }
        
        PrintNotification($"\n{DateTime.Now}\n------New Mail:------", ConsoleColor.Blue);
        Console.WriteLine(email.Subject);
        await _listener.ProcessEvent(email);
    }

    private void OnSubscriptionError(object sender, SubscriptionErrorEventArgs args)
    {
        
        PrintNotification("Subscription error occured. Exiting...", ConsoleColor.Red);
    }
    
    void OnDisconnect(object sender, SubscriptionErrorEventArgs args)
    {
        PrintNotification("\n------<Disconnected.>------", ConsoleColor.DarkYellow);
        PrintNotification("Trying to reestablish a connection...", ConsoleColor.Yellow);

        try
        {
            Connection?.Open();
        }
        catch (Exception e)
        {
            PrintNotification($"Could not reestablish a connection:\n{e.Message}", ConsoleColor.Red);
            throw;
        }

        if (Connection != null) PrintConnectionStatus(Connection);
    }

    private ExchangeService CreateService()
    {       
        return new ExchangeService
        {
            Url = new Uri($"https://{_url}/ews/exchange.asmx"),
            Credentials = new WebCredentials(_emailAddress, _password)
        };
    }

    private async Task<StreamingSubscription> NewMailSubscribtion()
    {
        StreamingSubscription subscription;

        try
        {
            subscription = await _exchangeService.SubscribeToStreamingNotifications(
                new FolderId[] { WellKnownFolderName.Inbox }, EventType.NewMail);
        }
        catch (Exception e)
        {
            PrintNotification("\nCouldn't authenticate agaist the Exchange Server.\n" +
            "Possible reasons are: invalid username and/or password, or domain was specicifed incorrectly.\n" +
            e.Message, ConsoleColor.Red);
            throw;
        }

        return subscription;
    }
}
