using Microsoft.AspNetCore.Identity;
using TransferService.Application.DTO;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;

namespace TransferService.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly PasswordHasher<Customer> _passwordHasher = new();

        public CustomerService(
            ICustomerRepository customerRepository,
            IAccountRepository accountRepository
        )
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
        }

        public async Task<Customer> RegisterCustomerAsync(CreateCustomerRequest request)
        {
            if (await _customerRepository.ExistsAsync(request.Email))
                throw new InvalidOperationException("A customer with this email already exists.");

            var tempCustomer = new Customer(
                new CustomerName(
                    request.FirstName,
                    request.MiddleName ?? string.Empty,
                    request.LastName
                ),
                request.DateOfBirth,
                request.PhoneNumber,
                request.Email,
                new Address(
                    request.Street ?? string.Empty,
                    request.City ?? string.Empty,
                    request.State ?? string.Empty,
                    request.PostalCode ?? string.Empty,
                    request.Country ?? string.Empty
                ),
                request.Username,
                request.Password
            );

            var passwordHash = _passwordHasher.HashPassword(tempCustomer, request.Password);
            tempCustomer.UpdatePasswordHash(passwordHash);

            await _customerRepository.AddAsync(tempCustomer);
            return tempCustomer;
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer is null)
                return null;
            Console.WriteLine("Customer found" + customer.Name);
            return new CustomerDto
            {
                Id = customer.Id,
                Username = customer.Username,
                FullName =
                    $"{customer.Name.FirstName} {customer.Name.MiddleName} {customer.Name.LastName}",
                Email = customer.Email,
            };
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                Username = c.Username,
                FullName = $"{c.Name.FirstName} {c.Name.LastName}",
                Email = c.Email,
            });
        }

        public async Task<bool> UpdateCustomerAsync(Guid id, UpdateCustomerRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer is null)
                return false;

            if (!string.IsNullOrWhiteSpace(request.Username))
                customer.UpdateUsername(request.Username);

            if (
                !string.IsNullOrWhiteSpace(request.FirstName)
                || !string.IsNullOrWhiteSpace(request.MiddleName)
                || !string.IsNullOrWhiteSpace(request.LastName)
            )
            {
                customer.UpdateName(
                    new CustomerName(
                        request.FirstName ?? customer.Name.FirstName,
                        request.MiddleName ?? customer.Name.MiddleName ?? string.Empty,
                        request.LastName ?? customer.Name.LastName
                    )
                );
            }

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var newHash = _passwordHasher.HashPassword(customer, request.Password);
                customer.UpdatePasswordHash(newHash);
            }

            var hasAddressUpdate =
                request.Street != null
                || request.City != null
                || request.State != null
                || request.PostalCode != null
                || request.Country != null;

            if (hasAddressUpdate)
            {
                var newAddress = new Address(
                    request.Street ?? customer.Address.Street,
                    request.City ?? customer.Address.City,
                    request.State ?? customer.Address.State,
                    request.PostalCode ?? customer.Address.PostalCode,
                    request.Country ?? customer.Address.Country
                );

                customer.UpdateAddress(newAddress);
            }

            await _customerRepository.UpdateAsync(customer);
            return true;
        }

        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            var customer = await _customerRepository.GetCustomerWithAccountsByIdAsync(id);
            if (customer is null)
                return false;
            await _accountRepository.DeleteRangeAsync(customer.Accounts);
            await _customerRepository.DeleteAsync(customer);
            return true;
        }

        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            var customer = await _customerRepository.GetByUsernameAsync(username);
            if (customer is null)
                return false;

            var result = _passwordHasher.VerifyHashedPassword(
                customer,
                customer.PasswordHash,
                password
            );

            return result == PasswordVerificationResult.Success;
        }

        public async Task<CustomerDto?> GetCustomerByUsernameAsync(string username)
        {
            var customer = await _customerRepository.GetByUsernameAsync(username);
            if (customer is null)
                return null;
            return new CustomerDto
            {
                Id = customer.Id,
                Username = customer.Username,
                FullName =
                    $"{customer.Name.FirstName} {customer.Name.MiddleName} {customer.Name.LastName}",
                Email = customer.Email,
            };
        }
    }
}
