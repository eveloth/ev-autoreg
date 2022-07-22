using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Configuration;

namespace EVAutoreg;

public class Exchange
{
    private readonly IConfiguration _config;
    private readonly string _url;
    private readonly string _emailAddress;
    private readonly string _password;


    public Exchange(IConfiguration config)
    {
        _config = config;
        _url = _config.GetValue<string>("ExchangeCredentials:URL");
        _emailAddress = _config.GetValue<string>("ExchangeCredentials:EmailAddress");
        _password = _config.GetValue<string>("ExchangeCredentials:Password");
    }

    public ExchangeService CreateService()
    {       
        return new ExchangeService
        {
            Url = new Uri($"https://{_url}/ews/exchange.asmx"),
            Credentials = new WebCredentials(_emailAddress, _password)
        };
    }

    public async Task<StreamingSubscription> NewMailSubscribtion(ExchangeService service)
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
