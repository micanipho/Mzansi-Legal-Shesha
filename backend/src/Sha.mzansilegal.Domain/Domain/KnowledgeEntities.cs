using Shesha.Domain.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sha.mzansilegal.Domain.Domain
{
    [Entity(TypeShortAlias = "Mzl.DocumentChunk")]
    [Table("document_chunks", Schema = "mzl")]
    public class DocumentChunk : MzansiLegalEntityBase
    {
        [Column("document_id")]
        public virtual LegalDocument Document { get; set; } = null!;

        [Column("chapter_title")]
        public virtual string? ChapterTitle { get; set; }

        [Column("section_number")]
        public virtual string? SectionNumber { get; set; }

        [Column("section_title")]
        public virtual string? SectionTitle { get; set; }

        [Column("content")]
        public virtual string Content { get; set; } = string.Empty;

        [Column("token_count")]
        public virtual int TokenCount { get; set; }

        [Column("sort_order")]
        public virtual int SortOrder { get; set; }

        public virtual ICollection<ChunkEmbedding> ChunkEmbeddings { get; set; } = new List<ChunkEmbedding>();

        public virtual ICollection<AnswerCitation> AnswerCitations { get; set; } = new List<AnswerCitation>();
    }

    [Entity(TypeShortAlias = "Mzl.ChunkEmbedding")]
    [Table("chunk_embeddings", Schema = "mzl")]
    public class ChunkEmbedding : MzansiLegalEntityBase
    {
        [Column("chunk_id")]
        public virtual DocumentChunk Chunk { get; set; } = null!;

        [Column("model_name")]
        public virtual string? ModelName { get; set; }

        [Column("dimensions")]
        public virtual int Dimensions { get; set; }

        [Column("embedding_json")]
        public virtual string EmbeddingJson { get; set; } = string.Empty;

        [Column("vector_hash")]
        public virtual string? VectorHash { get; set; }
    }
}
