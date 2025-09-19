using TransferService.Domain.Enums;

namespace TransferService.Application.DTO
{
    public class AccountDetailsResponse
    {
        public int AccountId { get; set; }
        public string OwnerName { get; set; } = "";
        public string AccountName { get; set; } = "";
        public string Username { get; set; } = "";
        public AccountTier Tier { get; set; }
        public Guid CustomerId { get; set; }

        public string Currency { get; set; } = "";
        public string AccountNumber { get; set; } = "";

        public decimal Balance { get; set; }

        public AccountType Type { get; set; }
    }
}
