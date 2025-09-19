using MediatR;
using TransferService.Application.DTO;

namespace TransferService.Application.Features.Customers.Queries.GetCustomerByUsername
{
    public record GetCustomerByUsernameQuery(string Username) : IRequest<CustomerDto?>;
}
