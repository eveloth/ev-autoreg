using FluentMigrator;

namespace DataAccessLibrary.Migrations;

[Migration(202212180100)]
public class MigrationSeed : Migration
{
    public override void Up()
    {
        Execute.Script("../DataAccessLibrary/ev-autoreg.sql");
    }

    public override void Down()
    {
        Execute.Script("../DataAccessLibrary/ev-autoreg-drop-schema.sql");
    }
}