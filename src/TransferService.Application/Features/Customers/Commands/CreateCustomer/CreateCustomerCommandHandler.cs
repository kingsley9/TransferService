using MediatR;
using Microsoft.AspNetCore.Identity;
using TransferService.Application.DTO;
using TransferService.Application.Features.Customers.Commands.CreateCustomer;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;

namespace TransferService.Application.Features.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Customer>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly PasswordHasher<Customer> _passwordHasher = new();

        public CreateCustomerCommandHandler(
            ICustomerRepository customerRepository,
            IAccountRepository accountRepository
        )
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
        }

        public async Task<Customer> Handle(
            CreateCustomerCommand request,
            CancellationToken cancellationToken
        )
        {
            var r = request.Request;
            if (await _customerRepository.ExistsAsync(r.Email))
                throw new InvalidOperationException("A customer with this email already exists.");

            var tempCustomer = new Customer(
                new CustomerName(r.FirstName, r.MiddleName ?? string.Empty, r.LastName),
                r.DateOfBirth,
                r.PhoneNumber,
                r.Email,
                new Address(
                    r.Street ?? string.Empty,
                    r.City ?? string.Empty,
                    r.State ?? string.Empty,
                    r.PostalCode ?? string.Empty,
                    r.Country ?? string.Empty
                ),
                r.Username,
                r.Password
            );

            var passwordHash = _passwordHasher.HashPassword(tempCustomer, r.Password);
            tempCustomer.UpdatePasswordHash(passwordHash);

            await _customerRepository.AddAsync(tempCustomer);
            return tempCustomer;
        }
    }
}
