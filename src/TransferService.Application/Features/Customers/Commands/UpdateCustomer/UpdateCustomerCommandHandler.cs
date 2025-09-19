using MediatR;
using Microsoft.AspNetCore.Identity;
using TransferService.Application.DTO;
using TransferService.Application.Features.Customers.Commands.UpdateCustomer;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;

namespace TransferService.Application.Features.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Customer>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly PasswordHasher<Customer> _passwordHasher = new();

        public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Customer> Handle(
            UpdateCustomerCommand request,
            CancellationToken cancellationToken
        )
        {
            var customer = await _customerRepository.GetByIdAsync(request.Id);
            if (customer == null)
                return null!;

            var r = request.Request;
            if (!string.IsNullOrWhiteSpace(r.Username))
                customer.UpdateUsername(r.Username);

            if (
                !string.IsNullOrWhiteSpace(r.FirstName)
                || !string.IsNullOrWhiteSpace(r.MiddleName)
                || !string.IsNullOrWhiteSpace(r.LastName)
            )
            {
                customer.UpdateName(
                    new CustomerName(
                        r.FirstName ?? customer.Name.FirstName,
                        r.MiddleName ?? customer.Name.MiddleName ?? string.Empty,
                        r.LastName ?? customer.Name.LastName
                    )
                );
            }

            if (!string.IsNullOrWhiteSpace(r.Password))
            {
                var newHash = _passwordHasher.HashPassword(customer, r.Password);
                customer.UpdatePasswordHash(newHash);
            }

            var hasAddressUpdate =
                r.Street != null
                || r.City != null
                || r.State != null
                || r.PostalCode != null
                || r.Country != null;

            if (hasAddressUpdate)
            {
                var newAddress = new Address(
                    r.Street ?? customer.Address.Street,
                    r.City ?? customer.Address.City,
                    r.State ?? customer.Address.State,
                    r.PostalCode ?? customer.Address.PostalCode,
                    r.Country ?? customer.Address.Country
                );

                customer.UpdateAddress(newAddress);
            }

            await _customerRepository.UpdateAsync(customer);
            return customer;
        }
    }
}
