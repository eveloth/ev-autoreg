using Microsoft.Exchange.WebServices.Data;

namespace EVAutoreg;

public static class PrettyPrinter
{
    public static void PrintNotification(string str, ConsoleColor color = ConsoleColor.Gray)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(str);
        Console.ResetColor();
    }

    public static void PrintConnectionStatus(StreamingSubscriptionConnection connection)
    {
        if (connection.IsOpen)
        {
            PrintNotification($"{DateTime.Now}\nConnection opened, listening on events...", ConsoleColor.Green);

            foreach (var sub in connection.CurrentSubscriptions)
            {
                Console.WriteLine(
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