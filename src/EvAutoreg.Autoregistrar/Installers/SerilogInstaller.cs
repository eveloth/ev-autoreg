using EvAutoreg.Autoregistrar.Options;
using Serilog;

namespace EvAutoreg.Autoregistrar.Installers;

public static class SerilogInstaller
{

    public static WebApplicationBuilder InstallSerilog(this WebApplicationBuilder builder)
    {
        var serilogOptions = new SerilogOptions();
        builder.Configuration.Bind(nameof(SerilogOptions), serilogOptions);

        var seqOptions = new SeqOptions();
        builder.Configuration.Bind(nameof(SeqOptions), seqOptions);

        Log.Logger = new LoggerConfiguration().ReadFrom
            .Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Conditional(
                _ => serilogOptions.EnableConsole,
                configuration => configuration.Console()
            )
            .WriteTo.Conditional(
                _ => serilogOptions.EnableFile,
                configuration => configuration.File("../logs/autoregistrar-.log")
            )
            .WriteTo.Conditional(
                _ => serilogOptions.EnableSeq,
                configuration => configuration.Seq(seqOptions.ServerUrl, apiKey: seqOptions.ApiKey)
            )
            .CreateLogger();
        builder.Host.UseSerilog();
        return builder;
    }
}