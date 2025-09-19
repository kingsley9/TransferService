using MediatR;
using TransferService.Application.DTO;

namespace TransferService.Application.Features.Accounts.Queries.GetAccountsByCustomer
{
    public record GetAccountsByCustomerQuery(Guid CustomerId)
        : IRequest<IEnumerable<AccountDetailsResponse>>;
}
