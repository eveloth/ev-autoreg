using Microsoft.Exchange.WebServices.Data;
using System.Text.RegularExpressions;
using Task = System.Threading.Tasks.Task;

namespace EVAutoreg;

class Program
{
    static ManualResetEvent _quitEvent = new ManualResetEvent(false);

    public static async Task Main(string[] args)
    {
        Console.CancelKeyPress += (sender, eArgs) => {
            _quitEvent.Set();
            eArgs.Cancel = true;
        };

        Console.WriteLine("Welcome to EV Hack!");

        Console.WriteLine("Enter server adress: ");
        string? serverAddress = Console.ReadLine();
        Console.WriteLine("Enter username: ");
        string? username = Console.ReadLine();
        Console.WriteLine("Enter password: ");
        string? password = Console.ReadLine();

        if (String.IsNullOrEmpty(serverAddress) || String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Server address, username and password cannot be empty. Exiting.");
            Console.ResetColor();
            throw new ArgumentNullException();
        }

        Exchange exchangeConfigurator = new Exchange();

        ExchangeService  exchangeService = exchangeConfigurator.CreateService(serverAddress, username, password);
        StreamingSubscription  subscription = await exchangeConfigurator.NewMailSubscribtion(exchangeService);
        
        foreach (EmailMessage email in await exchangeService.FindItems(WellKnownFolderName.Inbox, new ItemView(100)))
        {
            Console.WriteLine(email.Subject.ToString());
        }

        StreamingSubscriptionConnection connection = new(exchangeService, 10);
        
        connection.OnNotificationEvent += OnNotificationEvent;
        connection.OnSubscriptionError += OnSubscriptionError;
        connection.OnDisconnect += OnDisconnect;
        connection.AddSubscription(subscription);

        connection.Open();

        PrintConnectionStatus(connection);

        

        var startTimeSpan = TimeSpan.Zero;
        var periodTimeSpan = TimeSpan.FromMinutes(1);

        var timer = new System.Threading.Timer((e) =>
        {
            HealthCheck();   
        }, null, startTimeSpan, periodTimeSpan);

        _quitEvent.WaitOne();

        async void OnNotificationEvent(Object sender, NotificationEventArgs args)
        {
            ItemEvent notification = (ItemEvent?)args.Events.FirstOrDefault() ?? throw new ArgumentNullException("Notification was null, which was most possibly Exchange's fault...");

            EmailMessage email = await EmailMessage.Bind(exchangeService, notification.ItemId);

            Console.WriteLine("Recieved an email that we won't process.");
            
            if(Regex.IsMatch(email.Subject, @"^\[.+\]: Новое"))
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"\n{DateTime.Now}\n------New Mail:------");
                Console.ResetColor();
                Console.WriteLine(email.Subject);
                Console.Write("Case No. to process: ");

                Match match = Regex.Match(email.Subject, @"^\[.+(\d{6})\]");
                Console.ForegroundColor = ConsoleColor.DarkMagenta;

                string issueNo = match.Groups[1].Value;
                Console.WriteLine(email.From.Address, issueNo);
                Console.ResetColor();

                switch (email.From.Address)
                {
                    case "support":
                        Console.WriteLine($"TODO: Register as support issue ({issueNo})");
                        break;
                    case "service":
                        Console.WriteLine($"TODO: Register as service issue ({issueNo})");
                        break;
                    default:
                        break;
                }
            }              
        }

        void OnSubscriptionError(object sender, SubscriptionErrorEventArgs args)
        {
            Console.WriteLine("Subscription error occured. Exiting...");
        }

        void OnDisconnect(object sender, SubscriptionErrorEventArgs args)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n------<Disconnected.>------");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Trying to reestablish a connection...");
            Console.ResetColor();

            try
            {
                connection.Open();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Could not reestablish a connecntion:\n", e.Message);
                Console.ResetColor();
                throw;
            }       

            PrintConnectionStatus(connection);
        }

        void PrintConnectionStatus(StreamingSubscriptionConnection connection)
        {
            if (connection.IsOpen)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{DateTime.Now}\nConnection opened, listening on events...");
                Console.ResetColor();

                foreach (StreamingSubscription sub in connection.CurrentSubscriptions)
                {
                    System.Console.WriteLine(
                        "Subscription debugging info:\n" +
                        $"ID: {sub.Id}\n" +
                        $"Service: {sub.Service.Url}\n"
                    );
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error opening a connection");
                Console.ResetColor();
            }
        }

        void HealthCheck()
        {
            System.Console.WriteLine($"{DateTime.Now}: Application is running...");
        }

    }
}