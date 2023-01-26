using Serilog;

namespace Api.Installers;

public static class SerilogInstaller
{
    public static WebApplicationBuilder InstallSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration().ReadFrom
            .Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .CreateLogger();
        builder.Host.UseSerilog();
        return builder;
    }
}