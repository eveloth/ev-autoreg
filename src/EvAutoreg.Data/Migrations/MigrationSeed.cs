using FluentMigrator;

namespace EvAutoreg.Data.Migrations;

[Migration(202212180100)]
public class MigrationSeed : Migration
{
    public override void Up()
    {
        Execute.Script("../EvAutoreg.Data/ev-autoreg.sql");
    }

    public override void Down()
    {
        Execute.Script("../EvAutoreg.Data/ev-autoreg-drop-schema.sql");
    }
}