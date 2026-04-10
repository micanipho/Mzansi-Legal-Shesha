using Shesha.Domain.Attributes;
using System.ComponentModel;

namespace Sha.mzansilegal.Domain.Enums
{
    [ReferenceList("Mzl", "AnswerModes")]
    public enum RefListAnswerModes : long
    {
        [Description("Grounded")]
        Grounded = 1,

        [Description("General Guidance")]
        GeneralGuidance = 2,

        [Description("Escalation")]
        Escalation = 3
    }
}
