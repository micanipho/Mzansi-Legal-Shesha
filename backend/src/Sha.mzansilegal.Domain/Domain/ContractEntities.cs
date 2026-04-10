using Sha.mzansilegal.Domain.Enums;
using Shesha.Domain;
using Shesha.Domain.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sha.mzansilegal.Domain.Domain
{
    [Entity(TypeShortAlias = "Mzl.ContractAnalysis")]
    [Table("contract_analyses", Schema = "mzl")]
    public class ContractAnalysis : MzansiLegalEntityBase
    {
        [Column("file_name")]
        public virtual string FileName { get; set; } = string.Empty;

        [Column("original_file_id")]
        public virtual StoredFile? OriginalFile { get; set; }

        [Column("language")]
        public virtual string? Language { get; set; }

        [Column("status_lkp")]
        [ReferenceList("Mzl", "ContractAnalysisStatuses")]
        public virtual RefListContractAnalysisStatuses? Status { get; set; }

        [Column("overall_risk_score")]
        public virtual decimal? OverallRiskScore { get; set; }

        [Column("summary")]
        public virtual string? Summary { get; set; }

        [Column("plain_language_summary")]
        public virtual string? PlainLanguageSummary { get; set; }

        [Column("analysed_at")]
        public virtual System.DateTime? AnalysedAt { get; set; }

        public virtual ICollection<ContractFlag> ContractFlags { get; set; } = new List<ContractFlag>();
    }

    [Entity(TypeShortAlias = "Mzl.ContractFlag")]
    [Table("contract_flags", Schema = "mzl")]
    public class ContractFlag : MzansiLegalEntityBase
    {
        [Column("contract_analysis_id")]
        public virtual ContractAnalysis ContractAnalysis { get; set; } = null!;

        [Column("severity_lkp")]
        [ReferenceList("Mzl", "ContractFlagSeverities")]
        public virtual RefListContractFlagSeverities? Severity { get; set; }

        [Column("title")]
        public virtual string Title { get; set; } = string.Empty;

        [Column("description")]
        public virtual string Description { get; set; } = string.Empty;

        [Column("clause_reference")]
        public virtual string? ClauseReference { get; set; }

        [Column("recommendation")]
        public virtual string? Recommendation { get; set; }

        [Column("sort_order")]
        public virtual int SortOrder { get; set; }
    }
}
