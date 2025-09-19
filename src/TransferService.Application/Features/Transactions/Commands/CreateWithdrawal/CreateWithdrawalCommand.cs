using MediatR;
using TransferService.Application.DTO;

namespace TransferService.Application.Features.Transactions.Commands.CreateWithdrawal
{
    public record CreateWithdrawalCommand(TransactionRequest Request)
        : IRequest<TransactionResponse>;
}
