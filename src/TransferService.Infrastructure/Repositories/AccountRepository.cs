using Microsoft.EntityFrameworkCore;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;
using TransferService.Infrastructure.Data;

namespace TransferService.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetByIdAsync(int accountId) =>
            await _context.Accounts.FindAsync(accountId);

        public async Task<IEnumerable<Account>> GetAllAsync() =>
            await _context.Accounts.ToListAsync();

        public async Task<bool> ExistsAsync(string accountNumber)
        {
            return await _context.Accounts.AnyAsync(a => a.AccountNumber == accountNumber);
        }

        public async Task UpdateAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account is not null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteRangeAsync(IReadOnlyCollection<Account> accounts)
        {
            _context.Accounts.RemoveRange(accounts);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Account>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _context.Accounts.Where(x => x.CustomerId == customerId).ToListAsync();
        }
    }
}
