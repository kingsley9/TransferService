using MediatR;
using TransferService.Application.DTO;

namespace TransferService.Application.Features.Accounts.Queries.GetOwnedAccount
{
    public record GetOwnedAccountQuery(int AccountId, string Username)
        : IRequest<AccountDetailsResponse>;
}
