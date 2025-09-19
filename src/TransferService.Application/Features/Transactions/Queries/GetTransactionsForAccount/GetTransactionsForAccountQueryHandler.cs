using MediatR;
using TransferService.Application.DTO;
using TransferService.Application.Features.Transactions.Queries.GetTransactionsForAccount;
using TransferService.Application.Interfaces;

namespace TransferService.Application.Features.Transactions.Queries.GetTransactionsForAccount
{
    public class GetTransactionsForAccountQueryHandler
        : IRequestHandler<GetTransactionsForAccountQuery, IEnumerable<TransactionResponse>>
    {
        private readonly ITransactionRepository _transactionRepo;

        public GetTransactionsForAccountQueryHandler(ITransactionRepository transactionRepo)
        {
            _transactionRepo = transactionRepo;
        }

        public async Task<IEnumerable<TransactionResponse>> Handle(
            GetTransactionsForAccountQuery request,
            CancellationToken cancellationToken
        )
        {
            var transactions = await _transactionRepo.GetByAccountIdAsync(request.AccountId);
            return transactions.Select(t => new TransactionResponse
            {
                TransactionId = t.TransactionId,
                Status = t.Status,
                Type = t.Type,
                Timestamp = t.Timestamp,
            });
        }
    }
}
