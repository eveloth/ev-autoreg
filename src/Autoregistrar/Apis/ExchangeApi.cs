using System.Net;
using Autoregistrar.Settings;
using Microsoft.Exchange.WebServices.Data;

namespace Autoregistrar.Apis;

public static class ExchangeApi {

    public static ExchangeService CreateService()
    {
        return new ExchangeService()
        {
            Url = new Uri(
                $"https://{GlobalSettings.AutoregistrarSettings.ExchangeServerUri}/ews/exchange.asmx"
            ),
            Credentials = new NetworkCredential(
                GlobalSettings.ExchangeCredentials.Email,
                GlobalSettings.ExchangeCredentials.Password
            )
        };
    }

    public static async Task<StreamingSubscription> CreateStreamingSubscription(
        ExchangeService exchange
    )
    {
        return await exchange.SubscribeToStreamingNotifications(
            new FolderId[] { WellKnownFolderName.Inbox },
            EventType.NewMail
        );
    }
}