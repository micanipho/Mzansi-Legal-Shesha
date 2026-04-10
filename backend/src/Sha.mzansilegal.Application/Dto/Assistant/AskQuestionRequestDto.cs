using System;

namespace Sha.mzansilegal.Application.Dto.Assistant
{
    public class AskQuestionRequestDto
    {
        public Guid? ConversationId { get; set; }

        public Guid? LegalDocumentId { get; set; }

        public string Question { get; set; } = string.Empty;

        public string? Language { get; set; }

        public string? ConversationTitle { get; set; }

        public int MaxCitations { get; set; } = 3;
    }
}
