using MediatR;
using TransferService.Application.DTO;

namespace TransferService.Application.Features.Customers.Queries.GetCustomerById
{
    public record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDto?>;
}
