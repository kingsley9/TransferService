using MediatR;

namespace TransferService.Application.Features.Customers.Queries.GetCustomerByCredentials
{
    public record GetCustomerByCredentialsQuery(string Username, string Password) : IRequest<bool>;
}
