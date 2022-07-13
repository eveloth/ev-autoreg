using Microsoft.Exchange.WebServices.Data;
using System.Text.RegularExpressions;

namespace EVAutoreg;

class Program
{

    private volatile static bool _stop = false;
    public static void Main(string[] args)
    {
        Console.CancelKeyPress += new ConsoleCancelEventHandler(OnCtrlCPressed);

        while (!_stop)
        {
            //Logic
        }

    }

    private static void OnCtrlCPressed(object? sender, ConsoleCancelEventArgs e)
    {
        _stop = true;
        Console.WriteLine("SIGINT recieved, shutting down the apllication.");
    }
}