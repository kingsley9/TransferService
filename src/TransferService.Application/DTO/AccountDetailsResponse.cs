using TransferService.Domain.Enums;

namespace TransferService.Application.DTO
{
    public class AccountDetailsResponse
    {
        public int AccountId { get; set; }
        public string OwnerName { get; set; } = "";
        public decimal Balance { get; set; }

        public AccountType Type { get; set; }
    }
}
