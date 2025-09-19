using MediatR;
using TransferService.Application.DTO;
using TransferService.Application.Features.Customers.Queries.GetCustomerByUsername;
using TransferService.Application.Interfaces;

namespace TransferService.Application.Features.Customers.Queries.GetCustomerByUsername
{
    public class GetCustomerByUsernameQueryHandler
        : IRequestHandler<GetCustomerByUsernameQuery, CustomerDto?>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerByUsernameQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerDto?> Handle(
            GetCustomerByUsernameQuery request,
            CancellationToken cancellationToken
        )
        {
            var customer = await _customerRepository.GetByUsernameAsync(request.Username);
            if (customer == null)
                return null;

            return new CustomerDto
            {
                Id = customer.Id,
                Username = customer.Username,
                FullName =
                    $"{customer.Name.FirstName} {customer.Name.MiddleName} {customer.Name.LastName}".Trim(),
                Email = customer.Email,
            };
        }
    }
}
