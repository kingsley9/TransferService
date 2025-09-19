using MediatR;
using TransferService.Application.DTO;
using TransferService.Application.Features.Transactions.Queries.GetTransactionDetails;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;

namespace TransferService.Application.Features.Transactions.Queries.GetTransactionDetails
{
    public class GetTransactionDetailsQueryHandler
        : IRequestHandler<GetTransactionDetailsQuery, TransactionResponse?>
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly IAccountRepository _accountRepo;

        public GetTransactionDetailsQueryHandler(
            ITransactionRepository transactionRepo,
            IAccountRepository accountRepo
        )
        {
            _transactionRepo = transactionRepo;
            _accountRepo = accountRepo;
        }

        public async Task<TransactionResponse?> Handle(
            GetTransactionDetailsQuery request,
            CancellationToken cancellationToken
        )
        {
            var transaction = await _transactionRepo.GetByIdAsync(request.TransactionId);
            if (transaction == null)
                return null;

            var isOwner =
                (transaction.AccountId == request.AccountId)
                || (transaction.TargetAccountId == request.AccountId);
            if (!isOwner)
                return null;

            var account = await _accountRepo.GetByIdAsync(request.AccountId);
            if (account == null)
                return null;

            return new TransactionResponse
            {
                TransactionId = transaction.TransactionId,
                Status = transaction.Status,
                Type = transaction.Type,
                Timestamp = transaction.Timestamp,
                Balance = account.Balance,
            };
        }
    }
}
