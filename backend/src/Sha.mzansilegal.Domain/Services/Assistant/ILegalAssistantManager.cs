using System.Threading.Tasks;

namespace Sha.mzansilegal.Domain.Services.Assistant
{
    public interface ILegalAssistantManager
    {
        Task<AskQuestionResult> AskQuestionAsync(AskQuestionInput input, int? tenantId, long? userId);
    }
}
