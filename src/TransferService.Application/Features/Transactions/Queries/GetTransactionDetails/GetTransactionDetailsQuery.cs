using MediatR;
using TransferService.Application.DTO;

namespace TransferService.Application.Features.Transactions.Queries.GetTransactionDetails
{
    public record GetTransactionDetailsQuery(int AccountId, int TransactionId)
        : IRequest<TransactionResponse?>;
}
