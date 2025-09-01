using TransferService.Domain.Entities;

namespace TransferService.Application.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int accountId);
        Task<IEnumerable<Account>> GetAllAsync();
        Task<bool> ExistsAsync(string accountNumber);
        Task UpdateAsync(Account account);
        Task AddAsync(Account account);
        Task DeleteAsync(int accountId);
        Task DeleteRangeAsync(IReadOnlyCollection<Account> accounts);
    }
}
