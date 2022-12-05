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
                $"https://{StateManager.Settings!.AutoregistrarSettings.ExchangeServerUri}/ews/exchange.asmx"
            ),
            Credentials = new NetworkCredential(
                StateManager.Settings.ExchangeCredentials.Email,
                StateManager.Settings.ExchangeCredentials.Password
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