using Sha.mzansilegal.Domain.Enums;
using Shesha.Domain;
using Shesha.Domain.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sha.mzansilegal.Domain.Domain
{
    [Entity(TypeShortAlias = "Mzl.IngestionJob")]
    [Table("ingestion_jobs", Schema = "mzl")]
    public class IngestionJob : MzansiLegalEntityBase
    {
        [Column("legal_document_id")]
        public virtual LegalDocument? LegalDocument { get; set; }

        [Column("source_file_id")]
        public virtual StoredFile? SourceFile { get; set; }

        [Column("status_lkp")]
        [ReferenceList("Mzl", "IngestionJobStatuses")]
        public virtual RefListIngestionJobStatuses? Status { get; set; }

        [Column("started_at")]
        public virtual System.DateTime? StartedAt { get; set; }

        [Column("completed_at")]
        public virtual System.DateTime? CompletedAt { get; set; }

        [Column("total_chunks")]
        public virtual int? TotalChunks { get; set; }

        [Column("processed_chunks")]
        public virtual int? ProcessedChunks { get; set; }

        [Column("error_message")]
        public virtual string? ErrorMessage { get; set; }

        [Column("processing_notes")]
        public virtual string? ProcessingNotes { get; set; }
    }
}
