using Shesha.Domain;
using Shesha.Domain.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sha.mzansilegal.Domain.Domain
{
    [Entity(TypeShortAlias = "Mzl.LegalDocument")]
    [Table("legal_documents", Schema = "mzl")]
    public class LegalDocument : MzansiLegalEntityBase
    {
        [Column("title")]
        public virtual string Title { get; set; } = string.Empty;

        [Column("short_name")]
        public virtual string? ShortName { get; set; }

        [Column("act_number")]
        public virtual string? ActNumber { get; set; }

        [Column("year")]
        public virtual int? Year { get; set; }

        [Column("file_name")]
        public virtual string? FileName { get; set; }

        [Column("original_pdf_id")]
        public virtual StoredFile? OriginalPdf { get; set; }

        [Column("category_id")]
        public virtual Category? Category { get; set; }

        [Column("is_processed")]
        public virtual bool IsProcessed { get; set; }

        [Column("total_chunks")]
        public virtual int TotalChunks { get; set; }

        public virtual ICollection<DocumentChunk> DocumentChunks { get; set; } = new List<DocumentChunk>();

        public virtual ICollection<AnswerCitation> AnswerCitations { get; set; } = new List<AnswerCitation>();

        public virtual ICollection<IngestionJob> IngestionJobs { get; set; } = new List<IngestionJob>();
    }
}
