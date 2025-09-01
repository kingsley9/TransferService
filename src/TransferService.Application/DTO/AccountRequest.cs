using TransferService.Domain.Enums;

namespace TransferService.Application.DTO
{
    public class AccountRequest
    {
        public required AccountType Type { get; set; }
        public decimal Balance { get; set; }
        public AccountTier Tier { get; set; }
        public string Currency { get; set; } = "NGN";
        public string BankCode { get; set; } = "035";
        public required string SchemeCode { get; set; }
        public required string Pin { get; set; }
    }
}
