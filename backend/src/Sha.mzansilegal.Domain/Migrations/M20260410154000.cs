using FluentMigrator;
using Shesha.FluentMigrator;

namespace Sha.mzansilegal.Domain.Migrations
{
    [Migration(20260410154000)]
    public class M20260410154000 : OneWayMigration
    {
        public override void Up()
        {
            // MzansiLegal.ContractAnalysis
            Create.Table("contract_analyses").InSchema("mzl")
                .WithIdAsGuid("id")
                .WithFullAuditColumns(SnakeCaseDbObjectNames.Instance, true)
                .WithTenantIdAsNullable(SnakeCaseDbObjectNames.Instance).Indexed()
                .WithColumn("file_name").AsString(260)
                .WithColumn("original_file_id").AsGuid().Nullable().Indexed()
                .WithColumn("language").AsString(20).Nullable()
                .WithColumn("status_lkp").AsInt64().Nullable()
                .WithColumn("overall_risk_score").AsDecimal(5, 2).Nullable()
                .WithColumn("summary").AsStringMax().Nullable()
                .WithColumn("plain_language_summary").AsStringMax().Nullable()
                .WithColumn("analysed_at").AsDateTime().Nullable();

            Create.ForeignKey("fk_mzl_contract_analyses_original_file_id_stored_files_id")
                .FromTable("contract_analyses").InSchema("mzl").ForeignColumn("original_file_id")
                .ToTable("stored_files").InSchema("frwk").PrimaryColumn("id");

            // MzansiLegal.ContractFlag
            Create.Table("contract_flags").InSchema("mzl")
                .WithIdAsGuid("id")
                .WithFullAuditColumns(SnakeCaseDbObjectNames.Instance, true)
                .WithTenantIdAsNullable(SnakeCaseDbObjectNames.Instance).Indexed()
                .WithColumn("contract_analysis_id").AsGuid().Indexed()
                .WithColumn("severity_lkp").AsInt64().Nullable()
                .WithColumn("title").AsString(200)
                .WithColumn("description").AsStringMax()
                .WithColumn("clause_reference").AsString(200).Nullable()
                .WithColumn("recommendation").AsStringMax().Nullable()
                .WithColumn("sort_order").AsInt32().WithDefaultValue(0);

            Create.ForeignKey("fk_mzl_contract_flags_contract_analysis_id_contract_analyses_id")
                .FromTable("contract_flags").InSchema("mzl").ForeignColumn("contract_analysis_id")
                .ToTable("contract_analyses").InSchema("mzl").PrimaryColumn("id");

            // MzansiLegal.IngestionJob
            Create.Table("ingestion_jobs").InSchema("mzl")
                .WithIdAsGuid("id")
                .WithFullAuditColumns(SnakeCaseDbObjectNames.Instance, true)
                .WithTenantIdAsNullable(SnakeCaseDbObjectNames.Instance).Indexed()
                .WithColumn("legal_document_id").AsGuid().Nullable().Indexed()
                .WithColumn("source_file_id").AsGuid().Nullable().Indexed()
                .WithColumn("status_lkp").AsInt64().Nullable()
                .WithColumn("started_at").AsDateTime().Nullable()
                .WithColumn("completed_at").AsDateTime().Nullable()
                .WithColumn("total_chunks").AsInt32().Nullable()
                .WithColumn("processed_chunks").AsInt32().Nullable()
                .WithColumn("error_message").AsStringMax().Nullable()
                .WithColumn("processing_notes").AsStringMax().Nullable();

            Create.ForeignKey("fk_mzl_ingestion_jobs_legal_document_id_legal_documents_id")
                .FromTable("ingestion_jobs").InSchema("mzl").ForeignColumn("legal_document_id")
                .ToTable("legal_documents").InSchema("mzl").PrimaryColumn("id");

            Create.ForeignKey("fk_mzl_ingestion_jobs_source_file_id_stored_files_id")
                .FromTable("ingestion_jobs").InSchema("mzl").ForeignColumn("source_file_id")
                .ToTable("stored_files").InSchema("frwk").PrimaryColumn("id");
        }
    }
}
