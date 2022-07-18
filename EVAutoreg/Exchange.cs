using Microsoft.Exchange.WebServices.Data;

namespace EVAutoreg;

public static class Exchange
{
    public static ExchangeService CreateService(string url, string username, string password)
    {       
        return new ExchangeService
        {
            Url = new Uri($"https://{url}/ews/exchange.asmx"),
            Credentials = new WebCredentials(username, password)
        };
    }

    public static async Task<StreamingSubscription> NewMailSubscribtion(ExchangeService service)
    {
        StreamingSubscription subscription;

        try
        {
            subscription = await service.SubscribeToStreamingNotifications(
                new FolderId[] { WellKnownFolderName.Inbox }, EventType.NewMail);
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nCouldn't authenticate agaist the Exchange Server.\n" +
            "Possible reasons are: invalid username and/or password, or domain was specicifed incorrectly.\n" + e.Message);
            Console.ResetColor();
            throw;
        }

        return subscription;
    }
}
