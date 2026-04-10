using Sha.mzansilegal.Domain.Enums;
using Shesha.Domain.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sha.mzansilegal.Domain.Domain
{
    [Entity(TypeShortAlias = "Mzl.Conversation")]
    [Table("conversations", Schema = "mzl")]
    public class Conversation : MzansiLegalEntityBase
    {
        [Column("title")]
        public virtual string? Title { get; set; }

        [Column("language")]
        public virtual string? Language { get; set; }

        [Column("status_lkp")]
        [ReferenceList("Mzl", "ConversationStatuses")]
        public virtual RefListConversationStatuses? Status { get; set; }

        [Column("last_activity_time")]
        public virtual System.DateTime? LastActivityTime { get; set; }

        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }

    [Entity(TypeShortAlias = "Mzl.Question")]
    [Table("questions", Schema = "mzl")]
    public class Question : MzansiLegalEntityBase
    {
        [Column("conversation_id")]
        public virtual Conversation Conversation { get; set; } = null!;

        [Column("question_text")]
        public virtual string QuestionText { get; set; } = string.Empty;

        [Column("language")]
        public virtual string? Language { get; set; }

        [Column("sequence_no")]
        public virtual int SequenceNo { get; set; }

        public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }

    [Entity(TypeShortAlias = "Mzl.Answer")]
    [Table("answers", Schema = "mzl")]
    public class Answer : MzansiLegalEntityBase
    {
        [Column("question_id")]
        public virtual Question Question { get; set; } = null!;

        [Column("answer_text")]
        public virtual string AnswerText { get; set; } = string.Empty;

        [Column("language")]
        public virtual string? Language { get; set; }

        [Column("answer_mode_lkp")]
        [ReferenceList("Mzl", "AnswerModes")]
        public virtual RefListAnswerModes? AnswerMode { get; set; }

        [Column("confidence_score")]
        public virtual decimal? ConfidenceScore { get; set; }

        [Column("needs_urgent_attention")]
        public virtual bool NeedsUrgentAttention { get; set; }

        public virtual ICollection<AnswerCitation> AnswerCitations { get; set; } = new List<AnswerCitation>();
    }

    [Entity(TypeShortAlias = "Mzl.AnswerCitation")]
    [Table("answer_citations", Schema = "mzl")]
    public class AnswerCitation : MzansiLegalEntityBase
    {
        [Column("answer_id")]
        public virtual Answer Answer { get; set; } = null!;

        [Column("legal_document_id")]
        public virtual LegalDocument? LegalDocument { get; set; }

        [Column("document_chunk_id")]
        public virtual DocumentChunk? DocumentChunk { get; set; }

        [Column("title")]
        public virtual string? Title { get; set; }

        [Column("citation_text")]
        public virtual string? CitationText { get; set; }

        [Column("chapter_title")]
        public virtual string? ChapterTitle { get; set; }

        [Column("section_number")]
        public virtual string? SectionNumber { get; set; }

        [Column("section_title")]
        public virtual string? SectionTitle { get; set; }

        [Column("sort_order")]
        public virtual int SortOrder { get; set; }
    }
}
