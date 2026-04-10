using Shesha.Domain.Attributes;
using System.ComponentModel;

namespace Sha.mzansilegal.Domain.Enums
{
    [ReferenceList("Mzl", "ConversationStatuses")]
    public enum RefListConversationStatuses : long
    {
        [Description("Active")]
        Active = 1,

        [Description("Resolved")]
        Resolved = 2,

        [Description("Archived")]
        Archived = 3
    }
}
