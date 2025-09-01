using TransferService.Domain.Entities;

namespace TransferService.Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetByAccountIdAsync(int accountId);
        Task<Transaction?> GetByIdAsync(int transactionId);
    }
}
