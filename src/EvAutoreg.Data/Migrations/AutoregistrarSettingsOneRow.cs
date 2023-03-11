using FluentMigrator;

namespace EvAutoreg.Data.Migrations;

[Migration(202303102200)]
public class AutoregistrarSettingsOneRow : Migration
{
    private const string AutoregistrarSettingsOneRowUp =
        @"ALTER TABLE autoregistrar_settings DROP COLUMN id;
          CREATE UNIQUE INDEX autoregistrar_settings_one_row
          ON autoregistrar_settings((exchange_server_uri IS NOT NULL))";

    private const string AutoregistrarSettingsOneRowDown =
        @"DROP INDEX autoregistrar_settings_one_row;
          ALTER TABLE autoregistrar_settings ADD COLUMN
          id BIGSERIAL PRIMARY KEY";

    public override void Up()
    {
        Execute.Sql(AutoregistrarSettingsOneRowUp);
    }

    public override void Down()
    {
        Execute.Sql(AutoregistrarSettingsOneRowDown);
    }
}