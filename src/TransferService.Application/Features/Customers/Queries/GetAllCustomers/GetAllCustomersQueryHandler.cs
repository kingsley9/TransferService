using MediatR;
using TransferService.Application.DTO;
using TransferService.Application.Features.Customers.Queries.GetAllCustomers;
using TransferService.Application.Interfaces;

namespace TransferService.Application.Features.Customers.Queries.GetAllCustomers
{
    public class GetAllCustomersQueryHandler
        : IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetAllCustomersQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<CustomerDto>> Handle(
            GetAllCustomersQuery request,
            CancellationToken cancellationToken
        )
        {
            var customers = await _customerRepository.GetAllAsync();
            return customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                Username = c.Username,
                FullName = $"{c.Name.FirstName} {c.Name.LastName}".Trim(),
                Email = c.Email,
            });
        }
    }
}
