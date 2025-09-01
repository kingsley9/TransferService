using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransferService.Application.DTO;
using TransferService.Domain.Entities;

namespace TransferService.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerDto?> GetCustomerByIdAsync(Guid id);
        Task<Customer> RegisterCustomerAsync(CreateCustomerRequest request);
        Task<CustomerDto?> GetCustomerByUsernameAsync(string username);
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
        Task<bool> UpdateCustomerAsync(Guid id, UpdateCustomerRequest request);
        Task<bool> DeleteCustomerAsync(Guid id);
        Task<bool> AuthenticateAsync(string username, string password);
    }
}
