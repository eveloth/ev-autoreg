using Microsoft.Exchange.WebServices.Data;

namespace EVAutoreg;

class Exchange
{
    public ExchangeService CreateService(string url, string username, string password)
    {
        return new ExchangeService
        {
            Url = new Uri(url),
            Credentials = new WebCredentials(username, password)
        };
    }

    public Task<StreamingSubscription> NewMailSubscribtion(ExchangeService service)
    {
        return service.SubscribeToStreamingNotifications(
    new FolderId[] { WellKnownFolderName.Inbox }, EventType.NewMail);
    }

}
