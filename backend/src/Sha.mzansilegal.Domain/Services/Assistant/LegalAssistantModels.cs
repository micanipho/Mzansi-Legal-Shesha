using Sha.mzansilegal.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Sha.mzansilegal.Domain.Services.Assistant
{
    public class AskQuestionInput
    {
        public Guid? ConversationId { get; set; }

        public Guid? LegalDocumentId { get; set; }

        public string Question { get; set; } = string.Empty;

        public string? Language { get; set; }

        public string? ConversationTitle { get; set; }

        public int MaxCitations { get; set; } = 3;
    }

    public class AskQuestionResult
    {
        public Guid ConversationId { get; set; }

        public Guid QuestionId { get; set; }

        public Guid AnswerId { get; set; }

        public string AnswerText { get; set; } = string.Empty;

        public RefListAnswerModes AnswerMode { get; set; }

        public decimal? ConfidenceScore { get; set; }

        public bool NeedsUrgentAttention { get; set; }

        public List<AskQuestionCitationResult> Citations { get; set; } = new();
    }

    public class AskQuestionCitationResult
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
