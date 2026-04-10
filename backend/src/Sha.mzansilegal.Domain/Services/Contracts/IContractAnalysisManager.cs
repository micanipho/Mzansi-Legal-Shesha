using System.Threading.Tasks;

namespace Sha.mzansilegal.Domain.Services.Contracts
{
    public interface IContractAnalysisManager
    {
        Task<ContractAnalysisResult> AnalyzeAsync(AnalyzeContractInput input, int? tenantId, long? userId);
    }
}
