using Data.Data;
using Data.SqlDataAccess;
using EVAutoreg.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Task = System.Threading.Tasks.Task;
using static EVAutoreg.Auxiliary.PrettyPrinter;

namespace EVAutoreg.App;

internal static class EVAutoreg
{
    private static readonly ManualResetEvent QuitEvent = new(false);

    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services => { 
                services.AddSingleton<Exchange>();
                services.AddSingleton<Rules>();
                services.AddSingleton<IMailEventListener, MailEventListener>();
                services.AddSingleton<IEVApiWrapper, EVApiWrapper>();
                services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
                services.AddSingleton<IIssueData, IssueData>();
            })
            .Build();

        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        var exchange = host.Services.GetRequiredService<Exchange>();
        
        Console.WriteLine("Welcome to EV Autoregistrator!");
        
        await exchange.StartService();
        
        Console.CancelKeyPress += (sender, eArgs) => {
            QuitEvent.Set();
            eArgs.Cancel = true;
        };

        var startTimeSpan = TimeSpan.Zero;
        var periodTimeSpan = TimeSpan.FromMinutes(2);

        var timer = new Timer((e) =>
        {
            HealthCheck();
        }, null, startTimeSpan, periodTimeSpan);

        QuitEvent.WaitOne();

        #region Methods

        void HealthCheck() => PrintNotification($"{DateTime.Now}: Application is running...", ConsoleColor.DarkGray);

        #endregion
    }
}