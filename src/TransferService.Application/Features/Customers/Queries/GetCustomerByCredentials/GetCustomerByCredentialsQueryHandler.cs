using MediatR;
using Microsoft.AspNetCore.Identity;
using TransferService.Application.Features.Customers.Queries.GetCustomerByCredentials;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;

namespace TransferService.Application.Features.Customers.Queries.GetCustomerByCredentials
{
    public class GetCustomerByCredentialsQueryHandler
        : IRequestHandler<GetCustomerByCredentialsQuery, bool>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly PasswordHasher<Customer> _passwordHasher;

        public GetCustomerByCredentialsQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
            _passwordHasher = new PasswordHasher<Customer>();
        }

        public async Task<bool> Handle(
            GetCustomerByCredentialsQuery request,
            CancellationToken cancellationToken
        )
        {
            var customer = await _customerRepository.GetByUsernameAsync(request.Username);
            if (customer == null)
                return false;

            var result = _passwordHasher.VerifyHashedPassword(
                customer,
                customer.PasswordHash,
                request.Password
            );

            return result == PasswordVerificationResult.Success;
        }
    }
}
