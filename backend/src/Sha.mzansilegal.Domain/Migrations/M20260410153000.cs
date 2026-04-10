using FluentMigrator;
using Shesha.FluentMigrator;

namespace Sha.mzansilegal.Domain.Migrations
{
    [Migration(20260410153000)]
    public class M20260410153000 : OneWayMigration
    {
        public override void Up()
        {
            // MzansiLegal.Conversation
            Create.Table("conversations").InSchema("mzl")
                .WithIdAsGuid("id")
                .WithFullAuditColumns(SnakeCaseDbObjectNames.Instance, true)
                .WithTenantIdAsNullable(SnakeCaseDbObjectNames.Instance).Indexed()
                .WithColumn("title").AsString(200).Nullable()
                .WithColumn("language").AsString(20).Nullable()
                .WithColumn("status_lkp").AsInt64().Nullable()
                .WithColumn("last_activity_time").AsDateTime().Nullable();

            // MzansiLegal.Question
            Create.Table("questions").InSchema("mzl")
                .WithIdAsGuid("id")
                .WithFullAuditColumns(SnakeCaseDbObjectNames.Instance, true)
                .WithTenantIdAsNullable(SnakeCaseDbObjectNames.Instance).Indexed()
                .WithColumn("conversation_id").AsGuid().Indexed()
                .WithColumn("question_text").AsStringMax()
                .WithColumn("language").AsString(20).Nullable()
                .WithColumn("sequence_no").AsInt32().WithDefaultValue(1);

            Create.ForeignKey("fk_mzl_questions_conversation_id_conversations_id")
                .FromTable("questions").InSchema("mzl").ForeignColumn("conversation_id")
                .ToTable("conversations").InSchema("mzl").PrimaryColumn("id");

            // MzansiLegal.Answer
            Create.Table("answers").InSchema("mzl")
                .WithIdAsGuid("id")
                .WithFullAuditColumns(SnakeCaseDbObjectNames.Instance, true)
                .WithTenantIdAsNullable(SnakeCaseDbObjectNames.Instance).Indexed()
                .WithColumn("question_id").AsGuid().Indexed()
                .WithColumn("answer_text").AsStringMax()
                .WithColumn("language").AsString(20).Nullable()
                .WithColumn("answer_mode_lkp").AsInt64().Nullable()
                .WithColumn("confidence_score").AsDecimal(5, 4).Nullable()
                .WithColumn("needs_urgent_attention").AsBoolean().WithDefaultValue(false);

            Create.ForeignKey("fk_mzl_answers_question_id_questions_id")
                .FromTable("answers").InSchema("mzl").ForeignColumn("question_id")
                .ToTable("questions").InSchema("mzl").PrimaryColumn("id");

            // MzansiLegal.AnswerCitation
            Create.Table("answer_citations").InSchema("mzl")
                .WithIdAsGuid("id")
                .WithFullAuditColumns(SnakeCaseDbObjectNames.Instance, true)
                .WithTenantIdAsNullable(SnakeCaseDbObjectNames.Instance).Indexed()
                .WithColumn("answer_id").AsGuid().Indexed()
                .WithColumn("legal_document_id").AsGuid().Nullable().Indexed()
                .WithColumn("document_chunk_id").AsGuid().Nullable().Indexed()
                .WithColumn("title").AsString(300).Nullable()
                .WithColumn("citation_text").AsStringMax().Nullable()
                .WithColumn("chapter_title").AsString(250).Nullable()
                .WithColumn("section_number").AsString(50).Nullable()
                .WithColumn("section_title").AsString(250).Nullable()
                .WithColumn("sort_order").AsInt32().WithDefaultValue(0);

            Create.ForeignKey("fk_mzl_answer_citations_answer_id_answers_id")
                .FromTable("answer_citations").InSchema("mzl").ForeignColumn("answer_id")
                .ToTable("answers").InSchema("mzl").PrimaryColumn("id");

            Create.ForeignKey("fk_mzl_answer_citations_legal_document_id_legal_documents_id")
                .FromTable("answer_citations").InSchema("mzl").ForeignColumn("legal_document_id")
                .ToTable("legal_documents").InSchema("mzl").PrimaryColumn("id");

            Create.ForeignKey("fk_mzl_answer_citations_document_chunk_id_document_chunks_id")
                .FromTable("answer_citations").InSchema("mzl").ForeignColumn("document_chunk_id")
                .ToTable("document_chunks").InSchema("mzl").PrimaryColumn("id");
        }
    }
}
