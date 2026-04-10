using Shesha.Domain.Attributes;
using System.ComponentModel;

namespace Sha.mzansilegal.Domain.Enums
{
    [ReferenceList("Mzl", "ContractFlagSeverities")]
    public enum RefListContractFlagSeverities : long
    {
        [Description("Red")]
        Red = 1,

        [Description("Amber")]
        Amber = 2,

        [Description("Green")]
        Green = 3
    }
}
