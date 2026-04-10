using Abp.Application.Services;
using Abp.Authorization;
using Sha.mzansilegal.Application.Dto.Assistant;
using Sha.mzansilegal.Domain.Services.Assistant;
using System.Linq;
using System.Threading.Tasks;

namespace Sha.mzansilegal.Application.Services.Assistant
{
    [AbpAuthorize]
    public class MzansiLegalAssistantAppService : ApplicationService, IMzansiLegalAssistantAppService
    {
        private readonly ILegalAssistantManager _legalAssistantManager;

        public MzansiLegalAssistantAppService(ILegalAssistantManager legalAssistantManager)
        {
            _legalAssistantManager = legalAssistantManager;
        }

        public async Task<AskQuestionResponseDto> AskQuestion(AskQuestionRequestDto input)
        {
            var result = await _legalAssistantManager.AskQuestionAsync(new AskQuestionInput
            {
                ConversationId = input.ConversationId,
                LegalDocumentId = input.LegalDocumentId,
                Question = input.Question,
                Language = input.Language,
                ConversationTitle = input.ConversationTitle,
                MaxCitations = input.MaxCitations,
            }, AbpSession.TenantId, AbpSession.UserId);

            return new AskQuestionResponseDto
            {
                ConversationId = result.ConversationId,
                QuestionId = result.QuestionId,
                AnswerId = result.AnswerId,
                AnswerText = result.AnswerText,
                AnswerMode = result.AnswerMode.ToString(),
                ConfidenceScore = result.ConfidenceScore,
                NeedsUrgentAttention = result.NeedsUrgentAttention,
                Citations = result.Citations.Select(citation => new AskQuestionCitationDto
                {
                    LegalDocumentId = citation.LegalDocumentId,
                    DocumentChunkId = citation.DocumentChunkId,
                    Title = citation.Title,
                    CitationText = citation.CitationText,
                    ChapterTitle = citation.ChapterTitle,
                    SectionNumber = citation.SectionNumber,
                    SectionTitle = citation.SectionTitle,
                    SortOrder = citation.SortOrder,
                }).ToList(),
            };
        }
    }
}
