using TransferService.Application.DTO;
using TransferService.Domain.Entities;

namespace TransferService.Application.Interfaces
{
    public interface IAccountService
    {
        Task<Account?> GetAccountByIdAsync(int accountId);
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<Account> CreateAccountAsync(
            AccountRequest account,
            string ownerName,
            string username,
            Guid customerId
        );
        Task<bool> UpdateAccountAsync(Account account);
        Task<bool> DeleteAccountAsync(int accountId);
        Task<decimal?> GetAccountBalanceAsync(int accountId);
        Task<Account> GetOwnedAccountAsync(int accountId, string username);
    }
}
