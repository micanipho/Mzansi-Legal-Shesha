using Abp.Application.Services;
using Abp.Authorization;
using Sha.mzansilegal.Application.Dto.Contracts;
using Sha.mzansilegal.Domain.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Sha.mzansilegal.Application.Services.Contracts
{
    [AbpAuthorize]
    public class MzansiLegalContractAnalysisAppService : ApplicationService, IMzansiLegalContractAnalysisAppService
    {
        private readonly IContractAnalysisManager _contractAnalysisManager;

        public MzansiLegalContractAnalysisAppService(IContractAnalysisManager contractAnalysisManager)
        {
            _contractAnalysisManager = contractAnalysisManager;
        }

        public async Task<ContractAnalysisResponseDto> AnalyzeContract(AnalyzeContractRequestDto input)
        {
            var result = await _contractAnalysisManager.AnalyzeAsync(new AnalyzeContractInput
            {
                FileName = input.FileName,
                ContractText = input.ContractText,
                Language = input.Language,
            }, AbpSession.TenantId, AbpSession.UserId);

            return new ContractAnalysisResponseDto
            {
                AnalysisId = result.AnalysisId,
                FileName = result.FileName,
                Status = result.Status.ToString(),
                OverallRiskScore = result.OverallRiskScore,
                Summary = result.Summary,
                PlainLanguageSummary = result.PlainLanguageSummary,
                AnalysedAt = result.AnalysedAt,
                Flags = result.Flags.Select(flag => new ContractFlagDto
                {
                    FlagId = flag.FlagId,
                    Severity = flag.Severity.ToString(),
                    Title = flag.Title,
                    Description = flag.Description,
                    ClauseReference = flag.ClauseReference,
                    Recommendation = flag.Recommendation,
                    SortOrder = flag.SortOrder,
                }).ToList(),
            };
        }
    }
}
