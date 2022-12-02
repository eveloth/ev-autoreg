using System.Net;
using Autoregistrar.Settings;
using Microsoft.Exchange.WebServices.Data;

namespace Autoregistrar.Apis;

public class ExchangeApi {

    public ExchangeService CreateService()
    {
        return new ExchangeService()
        {
            Url = new Uri(
                $"https://{StatusManager.Settings.AutoregSettings.ExchangeServerUri}/ews/exchange.asmx"
            ),
            Credentials = new NetworkCredential(
                StatusManager.Settings.ExchangeCredentials.Email,
                StatusManager.Settings.ExchangeCredentials.Password
            )
        };
    }

    public async Task<StreamingSubscription> CreateStreamingSubscription(
        ExchangeService exchange,
        CancellationToken cts
    )
    {
        return await exchange.SubscribeToStreamingNotifications(
            new FolderId[] { WellKnownFolderName.Inbox },
            EventType.NewMail
        );
    }
}