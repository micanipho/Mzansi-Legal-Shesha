namespace Sha.mzansilegal.Application.Dto.Contracts
{
    public class AnalyzeContractRequestDto
    {
        public string FileName { get; set; } = string.Empty;

        public string ContractText { get; set; } = string.Empty;

        public string? Language { get; set; }
    }
}
