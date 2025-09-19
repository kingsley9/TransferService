using MediatR;
using TransferService.Application.DTO;

namespace TransferService.Application.Features.Transactions.Commands.CreateTransfer
{
    public record CreateTransferCommand(TransactionRequest Request) : IRequest<TransactionResponse>;
}
