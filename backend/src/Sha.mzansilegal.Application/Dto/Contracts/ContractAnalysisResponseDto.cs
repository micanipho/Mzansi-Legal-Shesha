using System;
using System.Collections.Generic;

namespace Sha.mzansilegal.Application.Dto.Contracts
{
    public class ContractAnalysisResponseDto
    {
        public Guid AnalysisId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public decimal OverallRiskScore { get; set; }

        public string Summary { get; set; } = string.Empty;

        public string PlainLanguageSummary { get; set; } = string.Empty;

        public DateTime AnalysedAt { get; set; }

        public List<ContractFlagDto> Flags { get; set; } = new();
    }

    public class ContractFlagDto
    {
        public Guid FlagId { get; set; }

        public string Severity { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? ClauseReference { get; set; }

        public string? Recommendation { get; set; }

        public int SortOrder { get; set; }
    }
}
