using MediatR;
using TransferService.Application.DTO;
using TransferService.Application.Features.Customers.Queries.GetCustomerById;
using TransferService.Application.Interfaces;

namespace TransferService.Application.Features.Customers.Queries.GetCustomerById
{
    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerDto?> Handle(
            GetCustomerByIdQuery request,
            CancellationToken cancellationToken
        )
        {
            var customer = await _customerRepository.GetByIdAsync(request.Id);
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
