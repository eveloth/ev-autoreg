using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Text.RegularExpressions;
using Task = System.Threading.Tasks.Task;
using static ColouredConsole;

namespace EVAutoreg;

internal static class Program
{
    static readonly ManualResetEvent _quitEvent = new(false);

    public static async Task Main(string[] args)
    {
        #region Declarations
        using var host = Host.CreateDefaultBuilder(args).Build();
        var config = host.Services.GetRequiredService<IConfiguration>();

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
        StreamingSubscriptionConnection connection = new(exchangeService, 15);
        
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

        async void OnNotificationEvent(object sender, NotificationEventArgs args)
        {
            ItemEvent notification = (ItemEvent?)args.Events.FirstOrDefault() ?? 
                                     throw new ArgumentNullException("Notification was null, which was most possibly Exchange's fault...");

            EmailMessage email = await EmailMessage.Bind(exchangeService, notification.ItemId);

            string subject = email.Subject;
            string content = email.Body.Text;

            if (Regex.IsMatch(subject, @"^\[.+\]: Новое"))
            {
                string issueNo = Regex.Match(email.Subject, @"^\[.+(\d{6})\]").Groups[1].Value;

                PrintInBlue($"\n{DateTime.Now}\n------New Mail:------");

                Console.WriteLine(subject);

                Console.Write("Case No. to process: ");
                PrintInPurple($"{email.From.Address}, {issueNo}");

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
                    content.Contains("на PROBLEM")||
                    Regex.IsMatch(subject, @"^\[.+\]: Новое.{1,4}\d{6}(?:.{0,2})?$"))
                {
                    Console.WriteLine("Received monitoring issue, processing...");
                    try
                    {
                        await AssignIssueToFirstLineOperators(issueNo, apiQueryRegisterParameters, apiQueryInWorkParameters);
                    }
                    catch (Exception e)
                    {
                        PrintInRed("Failed assigning an issue,\n" + e.Message);
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
            PrintInRed("Subscription error occured. Exiting...");
        }

        void OnDisconnect(object sender, SubscriptionErrorEventArgs args)
        {
            PrintInOrange("\n------<Disconnected.>------");
            PrintInYellow("Trying to reestablish a connection...");

            try
            {
                connection.Open();
            }
            catch (Exception e)
            {
                PrintInRed("Could not reestablish a connection:\n" + e.Message);
                throw;
            }       

            PrintConnectionStatus(connection);
        }

        async Task AssignIssueToFirstLineOperators(string issueNumber, string[] registeringParameters, string[] assigningParameters)
        {
            HttpStatusCode isOkRegistered = await evapi.UpdateIssue(issueNumber, registeringParameters);

            if (isOkRegistered == HttpStatusCode.OK)
            {
                var isOkInWork = await evapi.UpdateIssue(issueNumber, assigningParameters);
                if (isOkInWork == HttpStatusCode.OK)
                {
                    Console.WriteLine($"Succesfully assigned issue no. {issueNumber} to first line operators");
                }
            }
        }

        void PrintConnectionStatus(StreamingSubscriptionConnection connection)
        {
            if (connection.IsOpen)
            {
                PrintInGreen($"{DateTime.Now}\nConnection opened, listening on events...");

                foreach (StreamingSubscription sub in connection.CurrentSubscriptions)
                {
                    Console.WriteLine(
                        "Subscription debugging info:\n" +
                        $"ID: {sub.Id}\n" +
                        $"Service: {sub.Service.Url}\n"
                    );
                }
            }
            else
            {
                PrintInRed("Error opening a connection");
            }
        }

        void HealthCheck()
        {
            Console.WriteLine($"{DateTime.Now}: Application is running...");
        }

        #endregion
    }
}