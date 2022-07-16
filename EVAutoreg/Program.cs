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


        var exchangeConfigurator = new Exchange();

        if (String.IsNullOrEmpty(serverAddress) || String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
        {
            Console.WriteLine("Server address, username and password cannot be empty. Exiting.");
        }

        var exchangeService = exchangeConfigurator.CreateService($"https://{serverAddress}/ews/exchange.asmx", username, password);
        var subscription = await exchangeConfigurator.NewMailSubscribtion(exchangeService);



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
            Console.WriteLine("\n------New Mail:");

            foreach (var notification in args.Events)
            {
                var itemEvent = (ItemEvent)notification;
                Console.WriteLine($"{DateTime.Now}\nID: {itemEvent.ItemId}");
                EmailMessage email = await EmailMessage.Bind(exchangeService, itemEvent.ItemId);
                
                if(Regex.IsMatch(email.Subject, @"^\[.+\]: Новое"))
                {
                    System.Console.WriteLine(email.Subject);
                    Console.Write("Case No. to process: ");

                    Match match = Regex.Match(email.Subject, @"^\[.+(\d{6})\]");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(match.Groups[1].Value);
                    Console.ResetColor();
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
            Console.WriteLine("\n------Disconnected.");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Trying to reestablish a connection...");
            Console.ResetColor();

            try
            {
                connection.Open();
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
                throw;
            }       

            PrintConnectionStatus(connection);
        }

        void PrintConnectionStatus(StreamingSubscriptionConnection connection)
        {
            if (connection.IsOpen)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{DateTime.Now}\nOpened connection..");
                Console.ResetColor();

                foreach (StreamingSubscription sub in connection.CurrentSubscriptions)
                {
                    System.Console.WriteLine(
                        "Subscription debugging info:\n" +
                        $"ID: {sub.Id}\n" +
                        $"Service: {sub.Service.Url}\n" +
                        $"Watermark: {sub.Watermark}"
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