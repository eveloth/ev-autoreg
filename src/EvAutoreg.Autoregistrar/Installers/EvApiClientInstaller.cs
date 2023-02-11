using EvAutoreg.Autoregistrar.Apis;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace EvAutoreg.Autoregistrar.Installers;

public static class EvApiClientInstaller
{
    public static void InstallEvApiHttpClient(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddHttpClient(
                EvApi.ClientName,
                client => client.DefaultRequestHeaders.Add("user-agent", "OperatorsAPI")
            )
            .AddTransientHttpErrorPolicy(
                policyBuilder =>
                    policyBuilder.WaitAndRetryAsync(
                        Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5)
                    )
            );
    }
}