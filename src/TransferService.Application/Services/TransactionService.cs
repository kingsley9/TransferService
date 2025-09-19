using MediatR;
using TransferService.Application.DTO;
using TransferService.Application.Features.Transactions.Commands.CreateDeposit;
using TransferService.Application.Features.Transactions.Commands.CreateTransfer;
using TransferService.Application.Features.Transactions.Commands.CreateWithdrawal;
using TransferService.Application.Features.Transactions.Queries.GetTransactionDetails;
using TransferService.Application.Features.Transactions.Queries.GetTransactionsForAccount;
using TransferService.Application.Interfaces;

namespace TransferService.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IMediator _mediator;

        public TransactionService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TransactionResponse> TransferAsync(TransactionRequest request)
        {
            return await _mediator.Send(new CreateTransferCommand(request));
        }

        public async Task<TransactionResponse> Deposit(TransactionRequest request)
        {
            return await _mediator.Send(new CreateDepositCommand(request));
        }

        public async Task<TransactionResponse> Withdraw(TransactionRequest request)
        {
            return await _mediator.Send(new CreateWithdrawalCommand(request));
        }

        public async Task<IEnumerable<TransactionResponse>> GetTransactionsForAccountAsync(
            int accountId
        )
        {
            return await _mediator.Send(new GetTransactionsForAccountQuery(accountId));
        }

        public async Task<TransactionResponse?> GetTransactionDetailsAsync(
            int accountId,
            int transactionId
        )
        {
            return await _mediator.Send(new GetTransactionDetailsQuery(accountId, transactionId));
        }
    }
}
