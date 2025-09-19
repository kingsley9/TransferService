using MediatR;
using TransferService.Application.DTO;

namespace TransferService.Application.Features.Transactions.Commands.CreateDeposit
{
    public record CreateDepositCommand(TransactionRequest Request) : IRequest<TransactionResponse>;
}
