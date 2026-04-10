using FluentMigrator;
using Shesha.FluentMigrator;

namespace Sha.mzansilegal.Domain.Migrations
{
    [Migration(20260410150000)]
    public class M20260410150000 : OneWayMigration
    {
        public override void Up()
        {
            Execute.Sql(@"
                IF SCHEMA_ID('mzl') IS NULL
                BEGIN
                    EXEC('CREATE SCHEMA mzl');
                END
            ");
        }
    }
}
