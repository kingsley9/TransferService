using MediatR;
using TransferService.Application.DTO;

namespace TransferService.Application.Features.Accounts.Queries.GetAllAccounts
{
    public record GetAllAccountsQuery : IRequest<IEnumerable<AccountDetailsResponse>>;
}
