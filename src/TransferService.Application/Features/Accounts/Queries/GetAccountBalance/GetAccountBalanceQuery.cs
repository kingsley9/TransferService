using MediatR;

namespace TransferService.Application.Features.Accounts.Queries.GetAccountBalance
{
    public record GetAccountBalanceQuery(int AccountId) : IRequest<decimal?>;
}
