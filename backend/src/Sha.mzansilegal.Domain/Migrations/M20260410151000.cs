using FluentMigrator;
using Shesha.FluentMigrator;

namespace Sha.mzansilegal.Domain.Migrations
{
    [Migration(20260410151000)]
    public class M20260410151000 : OneWayMigration
    {
        public override void Up()
        {
            // MzansiLegal.Category
            Create.Table("categories").InSchema("mzl")
                .WithIdAsGuid("id")
                .WithFullAuditColumns(SnakeCaseDbObjectNames.Instance, true)
                .WithTenantIdAsNullable(SnakeCaseDbObjectNames.Instance).Indexed()
                .WithColumn("name").AsString(200)
                .WithColumn("icon").AsString(100).Nullable()
                .WithColumn("domain_lkp").AsInt64()
                .WithColumn("sort_order").AsInt32().WithDefaultValue(0);

            Alter.Table("categories").InSchema("mzl")
                .AddColumn("parent_id").AsGuid().Nullable().Indexed();

            Create.ForeignKey("fk_mzl_categories_parent_id_categories_id")
                .FromTable("categories").InSchema("mzl").ForeignColumn("parent_id")
                .ToTable("categories").InSchema("mzl").PrimaryColumn("id");

            Create.Index("ix_mzl_categories_name").OnTable("categories").InSchema("mzl")
                .OnColumn("name").Ascending()
                .WithOptions().NonClustered();

            // MzansiLegal.LegalDocument
            Create.Table("legal_documents").InSchema("mzl")
                .WithIdAsGuid("id")
                .WithFullAuditColumns(SnakeCaseDbObjectNames.Instance, true)
                .WithTenantIdAsNullable(SnakeCaseDbObjectNames.Instance).Indexed()
                .WithColumn("title").AsString(300)
                .WithColumn("short_name").AsString(150).Nullable()
                .WithColumn("act_number").AsString(50).Nullable()
                .WithColumn("year").AsInt32().Nullable()
                .WithColumn("file_name").AsString(260).Nullable()
                .WithColumn("original_pdf_id").AsGuid().Nullable().Indexed()
                .WithColumn("category_id").AsGuid().Nullable().Indexed()
                .WithColumn("is_processed").AsBoolean().WithDefaultValue(false)
                .WithColumn("total_chunks").AsInt32().WithDefaultValue(0);

            Create.ForeignKey("fk_mzl_legal_documents_category_id_categories_id")
                .FromTable("legal_documents").InSchema("mzl").ForeignColumn("category_id")
                .ToTable("categories").InSchema("mzl").PrimaryColumn("id");

            Create.ForeignKey("fk_mzl_legal_documents_original_pdf_id_stored_files_id")
                .FromTable("legal_documents").InSchema("mzl").ForeignColumn("original_pdf_id")
                .ToTable("stored_files").InSchema("frwk").PrimaryColumn("id");

            Create.Index("ix_mzl_legal_documents_title").OnTable("legal_documents").InSchema("mzl")
                .OnColumn("title").Ascending()
                .WithOptions().NonClustered();
        }
    }
}
