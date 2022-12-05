using Autoregistrar.App;
using Autoregistrar.GrpcServices;
using Autoregistrar.Mapping;
using Autoregistrar.Services;
using Autoregistrar.Settings;
using DataAccessLibrary.Extensions;
using MapsterMapper;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.

builder
    .AddNpgsql()
    .UseAffixForModelMapping("Model")
    .AddDapperSnakeCaseConvention()
    .AddRepositories();

builder.Services.AddSingleton<IMapper, Mapper>();
builder.Services.AddSingleton<ICredentialsDecryptor, CredentialsDecryptor>();
builder.Services.AddSingleton<ISettingsProvider, SettingsProvider>();
builder.Services.AddSingleton<IMailEventListener, MailEventListener>();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();

app.ConfigureDbToDomainMapping();

var env = app.Environment;

// Configure the HTTP request pipeline.
app.MapGrpcService<AutoregistrarService>();

if (env.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGet(
    "/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
);

app.Run();