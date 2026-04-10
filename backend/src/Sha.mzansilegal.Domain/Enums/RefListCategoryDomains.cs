using Shesha.Domain.Attributes;
using System.ComponentModel;

namespace Sha.mzansilegal.Domain.Enums
{
    [ReferenceList("Mzl", "CategoryDomains")]
    public enum RefListCategoryDomains : long
    {
        [Description("Legal")]
        Legal = 1,

        [Description("Financial")]
        Financial = 2
    }
}
