using EvAutoreg.Autoregistrar.Apis;
using EvAutoreg.Autoregistrar.GrpcServices;
using EvAutoreg.Autoregistrar.Hubs;
using EvAutoreg.Autoregistrar.Installers;
using EvAutoreg.Autoregistrar.Mapping;
using EvAutoreg.Autoregistrar.Options;
using EvAutoreg.Autoregistrar.Services;
using EvAutoreg.Autoregistrar.Services.Interfaces;
using EvAutoreg.Autoregistrar.Settings;
using EvAutoreg.Data.Extensions;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("xmlIssueOptions.json", optional: false);
XmlIssueOptions xmlIssueOptions = new();
builder.Configuration.Bind(nameof(XmlIssueOptions), xmlIssueOptions);
builder.AddExtendedXmlSerializer(xmlIssueOptions);

Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.

builder.AddJwtAuthentication();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UseRegistrar", policy => policy.RequireClaim("Permission", "UseRegistrar"));
    options.AddPolicy(
        "ForceStopRegistrar",
        policy => policy.RequireClaim("Permission", "ForceStopRegistrar")
    );
});

builder
    .AddNpgsql()
    .UseAffixForModelMapping("Model")
    .AddDapperSnakeCaseConvention()
    .AddRepositories();

builder.Services
    .AddHttpClient(EvApi.ClientName)
    .AddTransientHttpErrorPolicy(
        policyBuilder =>
            policyBuilder.WaitAndRetryAsync(
                Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5)
            )
    );

builder.Services.AddSingleton<IMapper, Mapper>();
builder.Services.AddSingleton<ICredentialsDecryptor, CredentialsDecryptor>();
builder.Services.AddSingleton<ISettingsProvider, SettingsProvider>();
builder.Services.AddSingleton<IMailEventListener, MailEventListener>();
builder.Services.AddSingleton<IEvApi, EvApi>();
builder.Services.AddSingleton<IIssueProcessor, IssueProcessor>();
builder.Services.AddSingleton<IIssueAnalyzer, IssueAnalyzer>();

builder.Services.TryAdd(
    ServiceDescriptor.Singleton(typeof(ILogDispatcher<>), typeof(LogDispatcher<>))
);

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.AddSignalR();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var httpClient = scope.ServiceProvider.GetService<HttpClient>();
    httpClient!.DefaultRequestHeaders.Add("User-agent", "OperatorsAPI");
}

app.ConfigureDbToDomainMapping();
app.ConfigureXmlToModelMapping();

var env = app.Environment;

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
app.MapGrpcService<AutoregistrarService>();

if (env.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapHub<AutoregistrarHub>("/log");

app.Run();