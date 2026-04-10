using Abp.Application.Services;
using Sha.mzansilegal.Application.Dto.Contracts;
using System.Threading.Tasks;

namespace Sha.mzansilegal.Application.Services.Contracts
{
    public interface IMzansiLegalContractAnalysisAppService : IApplicationService
    {
        Task<ContractAnalysisResponseDto> AnalyzeContract(AnalyzeContractRequestDto input);
    }
}
