using Microsoft.EntityFrameworkCore;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;
using TransferService.Infrastructure.Data;

namespace TransferService.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByIdAsync(Guid id) =>
            await _context.Customers.FindAsync(id);

        public async Task<Customer?> GetCustomerWithAccountsByIdAsync(Guid id)
        {
            return await _context
                .Customers.Include(c => c.Accounts)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Customer>> GetAllAsync() => await _context.Customers.ToListAsync();

        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Customer customer)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string email) =>
            await _context.Customers.AnyAsync(c => c.Email == email);

        public async Task<Customer?> GetByUsernameAsync(string username) =>
            await _context.Customers.FirstOrDefaultAsync(c => c.Username == username);
    }
}
