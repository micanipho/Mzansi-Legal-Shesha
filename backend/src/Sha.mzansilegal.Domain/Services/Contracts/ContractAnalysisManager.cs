using Abp.Domain.Repositories;
using Abp.UI;
using Sha.mzansilegal.Domain.Domain;
using Sha.mzansilegal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sha.mzansilegal.Domain.Services.Contracts
{
    public class ContractAnalysisManager : IContractAnalysisManager
    {
        private readonly IRepository<ContractAnalysis, Guid> _contractAnalysisRepository;
        private readonly IRepository<ContractFlag, Guid> _contractFlagRepository;

        public ContractAnalysisManager(
            IRepository<ContractAnalysis, Guid> contractAnalysisRepository,
            IRepository<ContractFlag, Guid> contractFlagRepository)
        {
            _contractAnalysisRepository = contractAnalysisRepository;
            _contractFlagRepository = contractFlagRepository;
        }

        public async Task<ContractAnalysisResult> AnalyzeAsync(AnalyzeContractInput input, int? tenantId, long? userId)
        {
            if (string.IsNullOrWhiteSpace(input.FileName))
                throw new UserFriendlyException("File name is required.");

            if (string.IsNullOrWhiteSpace(input.ContractText))
                throw new UserFriendlyException("Contract text is required.");

            var normalizedText = NormalizeWhitespace(input.ContractText);
            var analysedAt = DateTime.UtcNow;
            var matchedFlags = GetRules()
                .Where(rule => rule.IsMatch(normalizedText))
                .Select((rule, index) => new ContractFlagResult
                {
                    FlagId = Guid.NewGuid(),
                    Severity = rule.Severity,
                    Title = rule.Title,
                    Description = rule.Description,
                    ClauseReference = rule.FindClauseReference(normalizedText),
                    Recommendation = rule.Recommendation,
                    SortOrder = index + 1,
                })
                .ToList();

            if (!matchedFlags.Any())
            {
                matchedFlags.Add(new ContractFlagResult
                {
                    FlagId = Guid.NewGuid(),
                    Severity = RefListContractFlagSeverities.Green,
                    Title = "No immediate heuristic red flags detected",
                    Description = "The local rule set did not find any of the configured high-risk contract patterns in the submitted text.",
                    Recommendation = "Still review the agreement manually for business, compliance, and legal context before signing.",
                    SortOrder = 1,
                });
            }

            var overallRiskScore = CalculateRiskScore(matchedFlags);
            var summary = BuildSummary(matchedFlags, overallRiskScore);
            var plainLanguageSummary = BuildPlainLanguageSummary(matchedFlags, overallRiskScore);

            var analysis = new ContractAnalysis
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                FileName = input.FileName.Trim(),
                Language = input.Language?.Trim(),
                Status = RefListContractAnalysisStatuses.Completed,
                OverallRiskScore = overallRiskScore,
                Summary = summary,
                PlainLanguageSummary = plainLanguageSummary,
                AnalysedAt = analysedAt,
            };

            await _contractAnalysisRepository.InsertAsync(analysis);

            foreach (var flagResult in matchedFlags)
            {
                var flag = new ContractFlag
                {
                    Id = flagResult.FlagId,
                    TenantId = tenantId,
                    ContractAnalysis = analysis,
                    Severity = flagResult.Severity,
                    Title = flagResult.Title,
                    Description = flagResult.Description,
                    ClauseReference = flagResult.ClauseReference,
                    Recommendation = flagResult.Recommendation,
                    SortOrder = flagResult.SortOrder,
                };

                await _contractFlagRepository.InsertAsync(flag);
            }

            return new ContractAnalysisResult
            {
                AnalysisId = analysis.Id,
                FileName = analysis.FileName,
                Status = analysis.Status ?? RefListContractAnalysisStatuses.Completed,
                OverallRiskScore = overallRiskScore,
                Summary = summary,
                PlainLanguageSummary = plainLanguageSummary,
                AnalysedAt = analysedAt,
                Flags = matchedFlags,
            };
        }

        private static decimal CalculateRiskScore(List<ContractFlagResult> flags)
        {
            var redCount = flags.Count(x => x.Severity == RefListContractFlagSeverities.Red);
            var amberCount = flags.Count(x => x.Severity == RefListContractFlagSeverities.Amber);
            var greenCount = flags.Count(x => x.Severity == RefListContractFlagSeverities.Green);

            return Math.Min(100m, (redCount * 28m) + (amberCount * 14m) + (greenCount * 2m));
        }

        private static string BuildSummary(List<ContractFlagResult> flags, decimal overallRiskScore)
        {
            var redCount = flags.Count(x => x.Severity == RefListContractFlagSeverities.Red);
            var amberCount = flags.Count(x => x.Severity == RefListContractFlagSeverities.Amber);

            return $"Heuristic contract analysis completed with risk score {overallRiskScore:0.##}. " +
                $"Detected {redCount} red flag(s) and {amberCount} amber flag(s). " +
                $"Key issues: {string.Join("; ", flags.Take(3).Select(x => x.Title))}.";
        }

        private static string BuildPlainLanguageSummary(List<ContractFlagResult> flags, decimal overallRiskScore)
        {
            if (flags.All(x => x.Severity == RefListContractFlagSeverities.Green))
            {
                return $"This contract looks relatively low risk under the current local rule set, with a score of {overallRiskScore:0.##}, but it should still be reviewed by a person before signature.";
            }

            var highestSeverity = flags.Any(x => x.Severity == RefListContractFlagSeverities.Red) ? "high" : "moderate";
            return $"This contract has {highestSeverity} risk indicators under the local rule set, with a score of {overallRiskScore:0.##}. Review the flagged clauses carefully before signing.";
        }

        private static IReadOnlyCollection<ContractFlagRule> GetRules()
        {
            return new[]
            {
                new ContractFlagRule(
                    RefListContractFlagSeverities.Red,
                    "Broad indemnity obligation",
                    "The contract appears to impose an indemnity obligation, which can shift substantial liability.",
                    "Limit the indemnity scope, carve out indirect loss where appropriate, and align it to fault-based events.",
                    "indemnif(y|ies|ication)|hold harmless"),
                new ContractFlagRule(
                    RefListContractFlagSeverities.Red,
                    "Unlimited liability wording",
                    "The contract appears to remove or weaken liability caps.",
                    "Add a clear liability cap and list any narrow carve-outs explicitly.",
                    "unlimited liability|without limit"),
                new ContractFlagRule(
                    RefListContractFlagSeverities.Red,
                    "Penalty or liquidated damages clause",
                    "The contract references a penalty or liquidated damages clause that may carry heavy financial exposure.",
                    "Review the penalty mechanics and confirm they are commercially acceptable and enforceable.",
                    "penalty|liquidated damages"),
                new ContractFlagRule(
                    RefListContractFlagSeverities.Amber,
                    "Automatic renewal clause",
                    "The contract appears to renew automatically unless cancelled in time.",
                    "Add a clear reminder period, notice mechanism, and exit option before renewal.",
                    "automatic renewal|auto[- ]renew|renew automatically"),
                new ContractFlagRule(
                    RefListContractFlagSeverities.Amber,
                    "Exclusivity or restraint wording",
                    "The contract appears to restrict the ability to work with others or compete.",
                    "Confirm the duration, territory, and scope are all narrowly tailored and commercially justified.",
                    "exclusive|exclusivity|restraint of trade|non-compete"),
                new ContractFlagRule(
                    RefListContractFlagSeverities.Amber,
                    "Arbitration or jurisdiction clause",
                    "The contract specifies dispute resolution or jurisdiction wording that may affect enforcement cost and venue.",
                    "Check whether the chosen forum is practical and commercially appropriate.",
                    "arbitration|jurisdiction|governing law"),
                new ContractFlagRule(
                    RefListContractFlagSeverities.Amber,
                    "Data protection wording",
                    "The contract refers to personal information or data handling obligations.",
                    "Review POPIA and information security obligations carefully, especially for operator and responsible party roles.",
                    "personal information|data protection|POPIA|privacy"),
            };
        }

        private static string NormalizeWhitespace(string value)
        {
            return Regex.Replace(value.Trim(), "\\s+", " ");
        }

        private sealed class ContractFlagRule
        {
            private readonly Regex _regex;

            public ContractFlagRule(
                RefListContractFlagSeverities severity,
                string title,
                string description,
                string recommendation,
                string pattern)
            {
                Severity = severity;
                Title = title;
                Description = description;
                Recommendation = recommendation;
                _regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }

            public RefListContractFlagSeverities Severity { get; }

            public string Title { get; }

            public string Description { get; }

            public string Recommendation { get; }

            public bool IsMatch(string value)
            {
                return _regex.IsMatch(value);
            }

            public string? FindClauseReference(string value)
            {
                var match = _regex.Match(value);
                if (!match.Success)
                    return null;

                var start = Math.Max(0, match.Index - 60);
                var length = Math.Min(180, value.Length - start);
                return value.Substring(start, length).Trim();
            }
        }
    }
}
