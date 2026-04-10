using System;
using System.Collections.Generic;

namespace Sha.mzansilegal.Application.Dto.Assistant
{
    public class AskQuestionResponseDto
    {
        public Guid ConversationId { get; set; }

        public Guid QuestionId { get; set; }

        public Guid AnswerId { get; set; }

        public string AnswerText { get; set; } = string.Empty;

        public string AnswerMode { get; set; } = string.Empty;

        public decimal? ConfidenceScore { get; set; }

        public bool NeedsUrgentAttention { get; set; }

        public List<AskQuestionCitationDto> Citations { get; set; } = new();
    }

    public class AskQuestionCitationDto
    {
        public Guid? LegalDocumentId { get; set; }

        public Guid? DocumentChunkId { get; set; }

        public string? Title { get; set; }

        public string? CitationText { get; set; }

        public string? ChapterTitle { get; set; }

        public string? SectionNumber { get; set; }

        public string? SectionTitle { get; set; }

        public int SortOrder { get; set; }
    }
}
