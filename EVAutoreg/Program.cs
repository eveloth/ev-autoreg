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
        #region Declarations
        using IHost host = Host.CreateDefaultBuilder(args).Build();
        IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

        Exchange exchange = new Exchange(config);
        EVApiWrapper evapi = new(config);

        string[] apiQueryRegisterParameters = config.GetSection("QueryParameters:QueryRegisterParameters").Get<string[]>();
        string[] apiQueryInWorkParameters = config.GetSection("QueryParameters:QueryInWorkParameters").Get<string[]>();

        Console.CancelKeyPress += (sender, eArgs) => {
            _quitEvent.Set();
            eArgs.Cancel = true;
        };

        #endregion

        Console.WriteLine("Welcome to EV Autoregistrator!");

        

        ExchangeService  exchangeService = exchange.CreateService();
        StreamingSubscription  subscription = await exchange.NewMailSubscribtion(exchangeService);
        StreamingSubscriptionConnection connection = new(exchangeService, 10);
        

        connection.OnNotificationEvent += OnNotificationEvent;
        connection.OnSubscriptionError += OnSubscriptionError;
        connection.OnDisconnect += OnDisconnect;
        connection.AddSubscription(subscription);
        connection.Open();
        PrintConnectionStatus(connection);

        var startTimeSpan = TimeSpan.Zero;
        var periodTimeSpan = TimeSpan.FromMinutes(1);

        var timer = new Timer((e) =>
        {
            HealthCheck();
        }, null, startTimeSpan, periodTimeSpan);

        _quitEvent.WaitOne();

        #region Methods

        async void OnNotificationEvent(Object sender, NotificationEventArgs args)
        {
            ItemEvent notification = (ItemEvent?)args.Events.FirstOrDefault() ?? throw new ArgumentNullException("Notification was null, which was most possibly Exchange's fault...");

            EmailMessage email = await EmailMessage.Bind(exchangeService, notification.ItemId);

            string subject = email.Subject;

            if (Regex.IsMatch(subject, @"^\[.+\]: Новое"))
            {
                string issueNo = Regex.Match(email.Subject, @"^\[.+(\d{6})\]").Groups[1].Value;

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"\n{DateTime.Now}\n------New Mail:------");
                Console.ResetColor();

                Console.WriteLine(subject);

                Console.Write("Case No. to process: ");
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($"{email.From.Address}, {issueNo}");
                Console.ResetColor();

                //await evapi.GetIssue(issueNo);

                if (subject.Contains("is unreachable") ||
                    subject.Contains("unavailable by", StringComparison.InvariantCultureIgnoreCase) ||
                    subject.Contains("is not OK") ||
                    subject.Contains("has just been restarted") ||
                    subject.Contains("Free disk space") ||
                    subject.Contains("are not responding") ||
                    subject.Contains("ping response time") ||
                    subject.Contains("is above critical threshold") ||
                    subject.Contains("does not send any pings") ||
                    subject.Contains("high utilization") ||
                    subject.Contains("more than") ||
                    Regex.IsMatch(subject, @"^\[.+\]: Новое.{1,4}\d{6}(?:.{,2})?$"))
                {
                    Console.WriteLine("Recieved monitoring issue, processing...");
                    try
                    {
                        //await AssignIssueToFirstLineOperators(issueNo, apiQueryRegisterParameters, apiQueryInWorkParameters);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed assigning an issue,\n" + e.Message);
                    }                    
                }
            }
            else
            {
                Console.WriteLine("Recieved an email that we won't process.");
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

        async Task AssignIssueToFirstLineOperators(string issueNumber, string[] registeringParameters, string[] assigningParameters)
        {
            HttpResponseMessage registeredResponse = await evapi.UpdateIssue(issueNumber, registeringParameters);
            HttpResponseMessage inWorkResponse;

            if (registeredResponse.IsSuccessStatusCode)
            {
                inWorkResponse = await evapi.UpdateIssue(issueNumber, assigningParameters);
                if (inWorkResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Succesfully assigned issue no. {issueNumber} to first line operators");
                }
            }
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
            Console.WriteLine($"{DateTime.Now}: Application is running...");
        }

        #endregion
    }
}