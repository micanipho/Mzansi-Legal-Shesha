using Sha.mzansilegal.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Sha.mzansilegal.Domain.Services.Contracts
{
    public class AnalyzeContractInput
    {
        public string FileName { get; set; } = string.Empty;

        public string ContractText { get; set; } = string.Empty;

        public string? Language { get; set; }
    }

    public class ContractAnalysisResult
    {
        public Guid AnalysisId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public RefListContractAnalysisStatuses Status { get; set; }

        public decimal OverallRiskScore { get; set; }

        public string Summary { get; set; } = string.Empty;

        public string PlainLanguageSummary { get; set; } = string.Empty;

        public DateTime AnalysedAt { get; set; }

        public List<ContractFlagResult> Flags { get; set; } = new();
    }

    public class ContractFlagResult
    {
        public Guid FlagId { get; set; }

        public RefListContractFlagSeverities Severity { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? ClauseReference { get; set; }

        public string? Recommendation { get; set; }

        public int SortOrder { get; set; }
    }
}
