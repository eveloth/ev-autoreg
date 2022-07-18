using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.RegularExpressions;
using Task = System.Threading.Tasks.Task;

namespace EVAutoreg;

class Program
{
    static readonly ManualResetEvent _quitEvent = new(false);

    public static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args).Build();
        IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

        #region Declarations

        string exchangeServerUrl = config.GetValue<string>("ExchangeCredentials:URL");
        string exchangeServerUsername = config.GetValue<string>("ExchangeCredentials:EmailAddress");
        string exchangeServerPassword = config.GetValue<string>("ExchangeCredentials:Password");

        string extraViewServerUrl = config.GetValue<string>("ExtraViewCredentials:URL");
        string extraViewServerUsername = config.GetValue<string>("ExtraViewCredentials:Username");
        string extraViewServerPassword = config.GetValue<string>("ExtraViewCredentials:Password");

        string[] apiQueryUpdateParameters = config.GetSection("QueryParameters:QueryUpdateParameters").Get<string[]>();

        Console.CancelKeyPress += (sender, eArgs) => {
            _quitEvent.Set();
            eArgs.Cancel = true;
        };

        #endregion

        Console.WriteLine("Welcome to EV Autoregistrator!");

        ExchangeService  exchangeService = Exchange.CreateService(exchangeServerUrl, exchangeServerUsername, exchangeServerPassword);
        StreamingSubscription  subscription = await Exchange.NewMailSubscribtion(exchangeService);
        StreamingSubscriptionConnection connection = new(exchangeService, 10);
        EVApiWrapper evapi = new(extraViewServerUrl, extraViewServerUsername, extraViewServerPassword);

        connection.OnNotificationEvent += OnNotificationEvent;
        connection.OnSubscriptionError += OnSubscriptionError;
        connection.OnDisconnect += OnDisconnect;
        connection.AddSubscription(subscription);
        connection.Open();
        PrintConnectionStatus(connection);

        SetHealthCheckTimer();

        _quitEvent.WaitOne();

        #region Methods

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
                Console.WriteLine($"{email.From.Address}, {issueNo}");
                Console.ResetColor();

                await evapi.GetIssue(issueNo);
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
                Console.WriteLine("Could not reestablish a connecntion:\n" + e.Message);
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

        void SetHealthCheckTimer()
        {
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMinutes(1);

            var timer = new Timer((e) =>
            {
                HealthCheck();
            }, null, startTimeSpan, periodTimeSpan);
        }

        void HealthCheck()
        {
            Console.WriteLine($"{DateTime.Now}: Application is running...");
        }

        #endregion
    }
}