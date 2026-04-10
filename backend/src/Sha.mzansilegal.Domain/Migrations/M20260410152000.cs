using FluentMigrator;
using Shesha.FluentMigrator;

namespace Sha.mzansilegal.Domain.Migrations
{
    [Migration(20260410152000)]
    public class M20260410152000 : OneWayMigration
    {
        public override void Up()
        {
            // MzansiLegal.DocumentChunk
            Create.Table("document_chunks").InSchema("mzl")
                .WithIdAsGuid("id")
                .WithFullAuditColumns(SnakeCaseDbObjectNames.Instance, true)
                .WithTenantIdAsNullable(SnakeCaseDbObjectNames.Instance).Indexed()
                .WithColumn("document_id").AsGuid().Indexed()
                .WithColumn("chapter_title").AsString(250).Nullable()
                .WithColumn("section_number").AsString(50).Nullable()
                .WithColumn("section_title").AsString(250).Nullable()
                .WithColumn("content").AsStringMax()
                .WithColumn("token_count").AsInt32().WithDefaultValue(0)
                .WithColumn("sort_order").AsInt32().WithDefaultValue(0);

            Create.ForeignKey("fk_mzl_document_chunks_document_id_legal_documents_id")
                .FromTable("document_chunks").InSchema("mzl").ForeignColumn("document_id")
                .ToTable("legal_documents").InSchema("mzl").PrimaryColumn("id");

            // MzansiLegal.ChunkEmbedding
            Create.Table("chunk_embeddings").InSchema("mzl")
                .WithIdAsGuid("id")
                .WithFullAuditColumns(SnakeCaseDbObjectNames.Instance, true)
                .WithTenantIdAsNullable(SnakeCaseDbObjectNames.Instance).Indexed()
                .WithColumn("chunk_id").AsGuid().Indexed()
                .WithColumn("model_name").AsString(150).Nullable()
                .WithColumn("dimensions").AsInt32().WithDefaultValue(0)
                .WithColumn("embedding_json").AsStringMax()
                .WithColumn("vector_hash").AsString(128).Nullable();

            Create.ForeignKey("fk_mzl_chunk_embeddings_chunk_id_document_chunks_id")
                .FromTable("chunk_embeddings").InSchema("mzl").ForeignColumn("chunk_id")
                .ToTable("document_chunks").InSchema("mzl").PrimaryColumn("id");
        }
    }
}
