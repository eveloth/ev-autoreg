using Microsoft.Exchange.WebServices.Data;

namespace EvAutoreg.Console.Auxiliary;

public static class PrettyPrinter
{
    public static void PrintNotification(string str, ConsoleColor color = ConsoleColor.Gray)
    {
        System.Console.ForegroundColor = color;
        System.Console.WriteLine(str);
        System.Console.ResetColor();
    }

    public static void PrintConnectionStatus(StreamingSubscriptionConnection connection)
    {
        if (connection.IsOpen)
        {
            PrintNotification(
                $"{DateTime.Now}\nConnection opened, listening on events...",
                ConsoleColor.Green
            );

            foreach (var sub in connection.CurrentSubscriptions)
            {
                System.Console.WriteLine(
                    $"Subscription debugging info:\nID: {sub.Id}\nService: {sub.Service.Url}\n"
                );
            }
        }
        else
        {
            PrintNotification("Error opening a connection", ConsoleColor.Red);
        }
    }
}
