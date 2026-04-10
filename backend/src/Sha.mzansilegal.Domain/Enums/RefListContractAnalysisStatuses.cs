using Shesha.Domain.Attributes;
using System.ComponentModel;

namespace Sha.mzansilegal.Domain.Enums
{
    [ReferenceList("Mzl", "ContractAnalysisStatuses")]
    public enum RefListContractAnalysisStatuses : long
    {
        [Description("Pending")]
        Pending = 1,

        [Description("Processing")]
        Processing = 2,

        [Description("Completed")]
        Completed = 3,

        [Description("Failed")]
        Failed = 4
    }
}
