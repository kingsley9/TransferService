using MediatR;
using TransferService.Application.DTO;

namespace TransferService.Application.Features.Transactions.Queries.GetTransactionsForAccount
{
    public record GetTransactionsForAccountQuery(int AccountId)
        : IRequest<IEnumerable<TransactionResponse>>;
}
