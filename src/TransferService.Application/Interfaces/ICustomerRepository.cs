using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransferService.Domain.Entities;

namespace TransferService.Application.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid id);
        Task<Customer?> GetByUsernameAsync(string username);
        Task<Customer?> GetCustomerWithAccountsByIdAsync(Guid id);
        Task<List<Customer>> GetAllAsync();
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Customer customer);
        Task<bool> ExistsAsync(string email);
    }
}
