using Microsoft.EntityFrameworkCore;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;
using TransferService.Infrastructure.Data;

namespace TransferService.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(int accountId)
        {
            return await _context
                .Transactions.Where(t => t.AccountId == accountId || t.TargetAccountId == accountId)
                .ToListAsync();
        }

        public async Task<Transaction?> GetByIdAsync(int transactionId)
        {
            return await _context.Transactions.FindAsync(transactionId);
        }
    }
}
