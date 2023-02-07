using FluentMigrator;

namespace EvAutoreg.Data.Migrations;

[Migration(202212180100)]
public class MigrationSeed : Migration
{
    private static string? RunningInContainer => Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");

    private const string DevPathUp = "../EvAutoreg.Data/ev-autoreg.sql";
    private const string DevPathDown = "../EvAutoreg.Data/ev-autoreg-drop-schema.sql";
    private const string DockerPathUp = "/app/ev-autoreg.sql";
    private const string DockerPathDown = "/app/ev-autoreg-drop-schema.sql";

    public override void Up()
    {
        Execute.Script(RunningInContainer is not null ? DockerPathUp : DevPathUp);
    }

    public override void Down()
    {
        Execute.Script(RunningInContainer is not null ? DockerPathDown : DevPathDown);
    }
}