using Serilog;

namespace Api.Installers;

public static class SerilogInstaller
{
    public static WebApplicationBuilder InstallSerilog(this WebApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration().ReadFrom
            .Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);

        return builder;
    }
}