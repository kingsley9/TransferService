using TransferService.Application.DTO;
using TransferService.Domain.Entities;

namespace TransferService.Application.Interfaces
{
    public interface IAccountService
    {
        Task<AccountDetailsResponse?> GetAccountByIdAsync(int accountId);
        Task<IEnumerable<AccountDetailsResponse>> GetAllAccountsAsync();
        Task<AccountDetailsResponse> CreateAccountAsync(
            AccountRequest account,
            string ownerName,
            string username,
            Guid customerId
        );
        Task<bool> UpdateAccountAsync(Account account);
        Task<bool> DeleteAccountAsync(int accountId);
        Task<decimal?> GetAccountBalanceAsync(int accountId);
        Task<AccountDetailsResponse> GetOwnedAccountAsync(int accountId, string username);
        Task ChangeAccountPinAsync(Account account, string currentPin, string newPin);
    }
}
