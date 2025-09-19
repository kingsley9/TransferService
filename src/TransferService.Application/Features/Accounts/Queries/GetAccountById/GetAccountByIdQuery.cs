using MediatR;
using TransferService.Application.DTO;

namespace TransferService.Application.Features.Accounts.Queries.GetAccountById
{
    public record GetAccountByIdQuery(int AccountId) : IRequest<AccountDetailsResponse?>;
}
