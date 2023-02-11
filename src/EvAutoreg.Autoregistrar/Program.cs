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
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.InstallSerilog();

builder.Configuration.AddJsonFile("xmlIssueOptions.json", optional: false);
XmlIssueOptions xmlIssueOptions = new();
builder.Configuration.Bind(nameof(XmlIssueOptions), xmlIssueOptions);
builder.Services.AddSingleton(xmlIssueOptions);
builder.AddIssueXmlSerializer(xmlIssueOptions);
builder.Services.AddSingleton<IIssueDeserialzer, IssueDeserialzer>();

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

// Configure the HTTP request pipeline.
app.MapGrpcService<AutoregistrarService>();

var grpcReflectionOptions = new GrpcReflectionOptions();
app.Configuration.Bind(nameof(GrpcReflectionOptions), grpcReflectionOptions);

if (env.IsDevelopment() || grpcReflectionOptions.Enabled)
{
    app.MapGrpcReflectionService();
}

app.MapHub<AutoregistrarHub>("/log");

app.Run();