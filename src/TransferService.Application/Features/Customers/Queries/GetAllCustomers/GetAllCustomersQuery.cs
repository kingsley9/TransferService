using MediatR;
using TransferService.Application.DTO;

namespace TransferService.Application.Features.Customers.Queries.GetAllCustomers
{
    public record GetAllCustomersQuery : IRequest<IEnumerable<CustomerDto>>;
}
