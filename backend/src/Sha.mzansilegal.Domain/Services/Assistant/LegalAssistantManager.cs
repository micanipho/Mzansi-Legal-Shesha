using Abp.Domain.Repositories;
using Abp.Dependency;
using Abp.UI;
using Sha.mzansilegal.Domain.Domain;
using Sha.mzansilegal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sha.mzansilegal.Domain.Services.Assistant
{
    public class LegalAssistantManager : ILegalAssistantManager, ITransientDependency
    {
        private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "a", "an", "and", "are", "be", "by", "for", "from", "how", "i", "in", "is", "it",
            "me", "of", "on", "or", "our", "the", "their", "this", "to", "us", "was", "what",
            "when", "where", "which", "who", "why", "with", "you", "your"
        };

        private readonly IRepository<Conversation, Guid> _conversationRepository;
        private readonly IRepository<Question, Guid> _questionRepository;
        private readonly IRepository<Answer, Guid> _answerRepository;
        private readonly IRepository<AnswerCitation, Guid> _answerCitationRepository;
        private readonly IRepository<DocumentChunk, Guid> _documentChunkRepository;

        public LegalAssistantManager(
            IRepository<Conversation, Guid> conversationRepository,
            IRepository<Question, Guid> questionRepository,
            IRepository<Answer, Guid> answerRepository,
            IRepository<AnswerCitation, Guid> answerCitationRepository,
            IRepository<DocumentChunk, Guid> documentChunkRepository)
        {
            _conversationRepository = conversationRepository;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _answerCitationRepository = answerCitationRepository;
            _documentChunkRepository = documentChunkRepository;
        }

        public async Task<AskQuestionResult> AskQuestionAsync(AskQuestionInput input, int? tenantId, long? userId)
        {
            if (string.IsNullOrWhiteSpace(input.Question))
                throw new UserFriendlyException("Question is required.");

            var maxCitations = Math.Min(Math.Max(input.MaxCitations, 1), 10);
            var conversation = await GetOrCreateConversationAsync(input, tenantId);

            var question = new Question
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Conversation = conversation,
                QuestionText = input.Question.Trim(),
                Language = input.Language?.Trim(),
                SequenceNo = _questionRepository.GetAll().Count(q => q.Conversation.Id == conversation.Id) + 1,
            };

            await _questionRepository.InsertAsync(question);

            var rankedChunks = RankChunks(input.Question, input.LegalDocumentId)
                .Take(maxCitations)
                .ToList();

            var hasCitations = rankedChunks.Any();
            var answerMode = hasCitations ? RefListAnswerModes.Grounded : RefListAnswerModes.Escalation;
            var confidenceScore = hasCitations
                ? Math.Round(Math.Min(0.95m, 0.45m + ((decimal)rankedChunks.First().Score / 25m)), 4)
                : 0.15m;
            var needsUrgentAttention = RequiresUrgentAttention(input.Question);
            var answerText = BuildAnswerText(rankedChunks, needsUrgentAttention);

            var answer = new Answer
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Question = question,
                AnswerText = answerText,
                Language = input.Language?.Trim(),
                AnswerMode = answerMode,
                ConfidenceScore = confidenceScore,
                NeedsUrgentAttention = needsUrgentAttention,
            };

            await _answerRepository.InsertAsync(answer);

            var citationResults = new List<AskQuestionCitationResult>();
            var sortOrder = 1;

            foreach (var item in rankedChunks)
            {
                var citation = new AnswerCitation
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Answer = answer,
                    LegalDocument = item.Chunk.Document,
                    DocumentChunk = item.Chunk,
                    Title = item.Chunk.Document?.Title,
                    CitationText = CreateExcerpt(item.Chunk.Content, 400),
                    ChapterTitle = item.Chunk.ChapterTitle,
                    SectionNumber = item.Chunk.SectionNumber,
                    SectionTitle = item.Chunk.SectionTitle,
                    SortOrder = sortOrder,
                };

                await _answerCitationRepository.InsertAsync(citation);

                citationResults.Add(new AskQuestionCitationResult
                {
                    LegalDocumentId = item.Chunk.Document?.Id,
                    DocumentChunkId = item.Chunk.Id,
                    Title = citation.Title,
                    CitationText = citation.CitationText,
                    ChapterTitle = citation.ChapterTitle,
                    SectionNumber = citation.SectionNumber,
                    SectionTitle = citation.SectionTitle,
                    SortOrder = sortOrder,
                });

                sortOrder++;
            }

            conversation.LastActivityTime = DateTime.UtcNow;
            conversation.Status = RefListConversationStatuses.Active;
            await _conversationRepository.UpdateAsync(conversation);

            return new AskQuestionResult
            {
                ConversationId = conversation.Id,
                QuestionId = question.Id,
                AnswerId = answer.Id,
                AnswerText = answer.AnswerText,
                AnswerMode = answer.AnswerMode ?? RefListAnswerModes.Escalation,
                ConfidenceScore = answer.ConfidenceScore,
                NeedsUrgentAttention = answer.NeedsUrgentAttention,
                Citations = citationResults,
            };
        }

        private async Task<Conversation> GetOrCreateConversationAsync(AskQuestionInput input, int? tenantId)
        {
            if (input.ConversationId.HasValue)
            {
                var existingConversation = await _conversationRepository.FirstOrDefaultAsync(input.ConversationId.Value);
                if (existingConversation == null)
                    throw new UserFriendlyException($"Conversation '{input.ConversationId}' was not found.");

                return existingConversation;
            }

            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Title = string.IsNullOrWhiteSpace(input.ConversationTitle)
                    ? TrimToLength(input.Question.Trim(), 120)
                    : TrimToLength(input.ConversationTitle.Trim(), 200),
                Language = input.Language?.Trim(),
                Status = RefListConversationStatuses.Active,
                LastActivityTime = DateTime.UtcNow,
            };

            await _conversationRepository.InsertAsync(conversation);

            return conversation;
        }

        private IEnumerable<RankedChunk> RankChunks(string question, Guid? legalDocumentId)
        {
            var normalizedQuestion = question.Trim();
            var terms = Tokenize(normalizedQuestion).ToList();
            if (!terms.Any())
                return Enumerable.Empty<RankedChunk>();

            var chunks = _documentChunkRepository
                .GetAllIncluding(x => x.Document)
                .Where(x => !legalDocumentId.HasValue || x.Document.Id == legalDocumentId.Value)
                .ToList();

            return chunks
                .Select(chunk => new RankedChunk(chunk, CalculateScore(chunk, normalizedQuestion, terms)))
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Chunk.Document?.Title)
                .ThenBy(x => x.Chunk.SortOrder);
        }

        private static double CalculateScore(DocumentChunk chunk, string normalizedQuestion, List<string> terms)
        {
            var combinedText = string.Join(" ", new[]
            {
                chunk.Document?.Title,
                chunk.Document?.ShortName,
                chunk.ChapterTitle,
                chunk.SectionNumber,
                chunk.SectionTitle,
                chunk.Content
            }.Where(x => !string.IsNullOrWhiteSpace(x))).ToLowerInvariant();

            double score = 0;
            var distinctMatches = 0;

            foreach (var term in terms)
            {
                var occurrences = CountOccurrences(combinedText, term);
                if (occurrences <= 0)
                    continue;

                distinctMatches++;
                score += 4 + occurrences;

                if (!string.IsNullOrWhiteSpace(chunk.Document?.Title) &&
                    chunk.Document.Title.Contains(term, StringComparison.OrdinalIgnoreCase))
                    score += 4;

                if ((!string.IsNullOrWhiteSpace(chunk.SectionTitle) &&
                        chunk.SectionTitle.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(chunk.ChapterTitle) &&
                        chunk.ChapterTitle.Contains(term, StringComparison.OrdinalIgnoreCase)))
                    score += 2;
            }

            if (combinedText.Contains(normalizedQuestion.ToLowerInvariant(), StringComparison.Ordinal))
                score += 8;

            if (distinctMatches >= 2)
                score += distinctMatches * 1.5;

            return score;
        }

        private static IEnumerable<string> Tokenize(string input)
        {
            return Regex
                .Split(input.ToLowerInvariant(), "[^a-z0-9]+")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Where(x => x.Length > 2)
                .Where(x => !StopWords.Contains(x))
                .Distinct();
        }

        private static int CountOccurrences(string haystack, string needle)
        {
            var count = 0;
            var index = 0;

            while ((index = haystack.IndexOf(needle, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += needle.Length;
            }

            return count;
        }

        private static bool RequiresUrgentAttention(string question)
        {
            var urgentTerms = new[]
            {
                "urgent", "deadline", "court", "summons", "eviction", "dismissal", "arrest"
            };

            return urgentTerms.Any(term => question.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        private static string BuildAnswerText(List<RankedChunk> rankedChunks, bool needsUrgentAttention)
        {
            if (!rankedChunks.Any())
            {
                var notFoundBuilder = new StringBuilder();
                notFoundBuilder.Append("I couldn't find a grounded answer for this question in the local legal knowledge base. ");
                notFoundBuilder.Append("Try narrowing the question to a specific law, topic, or section, or review the generated CRUD data for the relevant legal documents.");

                if (needsUrgentAttention)
                    notFoundBuilder.Append(" Because your question looks time-sensitive, it would be wise to escalate this for immediate human review.");

                return notFoundBuilder.ToString();
            }

            var builder = new StringBuilder();
            builder.Append("Based on the local legal knowledge base, these are the most relevant passages for your question: ");

            for (var i = 0; i < rankedChunks.Count; i++)
            {
                var chunk = rankedChunks[i].Chunk;
                if (i > 0)
                    builder.Append(' ');

                builder.Append($"{i + 1}. ");
                builder.Append(chunk.Document?.Title ?? "Untitled document");

                var sectionLabel = BuildSectionLabel(chunk);
                if (!string.IsNullOrWhiteSpace(sectionLabel))
                    builder.Append($" ({sectionLabel})");

                builder.Append(": ");
                builder.Append(CreateExcerpt(chunk.Content, 260));
            }

            builder.Append(" Review the cited provisions directly before relying on them as formal legal advice.");

            if (needsUrgentAttention)
                builder.Append(" This question also looks urgent, so a human legal review is recommended.");

            return builder.ToString();
        }

        private static string BuildSectionLabel(DocumentChunk chunk)
        {
            var pieces = new[]
            {
                chunk.ChapterTitle,
                string.IsNullOrWhiteSpace(chunk.SectionNumber) ? null : $"s. {chunk.SectionNumber}",
                chunk.SectionTitle
            };

            return string.Join(", ", pieces.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private static string CreateExcerpt(string? value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var normalized = Regex.Replace(value.Trim(), "\\s+", " ");
            return normalized.Length <= maxLength
                ? normalized
                : $"{normalized[..maxLength].TrimEnd()}...";
        }

        private static string TrimToLength(string value, int maxLength)
        {
            if (value.Length <= maxLength)
                return value;

            return value[..maxLength].TrimEnd();
        }

        private sealed class RankedChunk
        {
            public RankedChunk(DocumentChunk chunk, double score)
            {
                Chunk = chunk;
                Score = score;
            }

            public DocumentChunk Chunk { get; }

            public double Score { get; }
        }
    }
}
