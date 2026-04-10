using Shesha.Domain.Attributes;
using System.ComponentModel;

namespace Sha.mzansilegal.Domain.Enums
{
    [ReferenceList("Mzl", "IngestionJobStatuses")]
    public enum RefListIngestionJobStatuses : long
    {
        [Description("Queued")]
        Queued = 1,

        [Description("Extracting")]
        Extracting = 2,

        [Description("Transforming")]
        Transforming = 3,

        [Description("Loading")]
        Loading = 4,

        [Description("Completed")]
        Completed = 5,

        [Description("Failed")]
        Failed = 6
    }
}
