using Abp.Application.Services;
using Sha.mzansilegal.Application.Dto.Assistant;
using System.Threading.Tasks;

namespace Sha.mzansilegal.Application.Services.Assistant
{
    public interface IMzansiLegalAssistantAppService : IApplicationService
    {
        Task<AskQuestionResponseDto> AskQuestion(AskQuestionRequestDto input);
    }
}
