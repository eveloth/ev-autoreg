using EvAutoreg.Autoregistrar.Apis;
using EvAutoreg.Autoregistrar.GrpcServices;
using EvAutoreg.Autoregistrar.Hubs;
using EvAutoreg.Autoregistrar.Installers;
using EvAutoreg.Autoregistrar.Mapping;
using EvAutoreg.Autoregistrar.Options;
using EvAutoreg.Autoregistrar.Reflection;
using EvAutoreg.Autoregistrar.Services;
using EvAutoreg.Autoregistrar.Services.Interfaces;
using EvAutoreg.Autoregistrar.Settings;
using EvAutoreg.Autoregistrar.State;
using EvAutoreg.Data.Extensions;
using MapsterMapper;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.InstallSerilog();

builder.Configuration.AddJsonFile("xmlIssueOptions.json", optional: false);
XmlIssueOptions xmlIssueOptions = new();
builder.Configuration.Bind(nameof(XmlIssueOptions), xmlIssueOptions);
builder.Services.AddSingleton(xmlIssueOptions);

builder.AddIssueXmlSerializer(xmlIssueOptions);
builder.Services.AddSingleton<IIssueDeserialzer, IssueDeserialzer>();

builder.GatherPropertiesInformation();

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

builder.InstallEvApiHttpClient();

builder.Services.AddSingleton<IMapper, Mapper>();
builder.Services.AddSingleton<ICredentialsDecryptor, CredentialsDecryptor>();
builder.Services.AddSingleton<ISettingsProvider, SettingsProvider>();
builder.Services.AddSingleton<IListenerProxy, ListenerProxy>();
builder.Services.AddSingleton<IStateManager, StateManager>();
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

app.ConfigureDbToDomainMapping();
app.ConfigureXmlToModelMapping();

var env = app.Environment;

app.UseForwardedHeaders(
    new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    }
);

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<AutoregistrarService>();

var grpcReflectionOptions = new GrpcReflectionOptions();
app.Configuration.Bind(nameof(GrpcReflectionOptions), grpcReflectionOptions);

if (env.IsDevelopment() || grpcReflectionOptions.Enabled)
{
    app.MapGrpcReflectionService();
}

app.MapHub<AutoregistrarHub>("/log");

app.Run();