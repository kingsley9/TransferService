using TransferService.Application.DTO;

namespace TransferService.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionResponse> TransferAsync(TransactionRequest request);
        Task<TransactionResponse> Deposit(TransactionRequest request);
        Task<TransactionResponse> Withdraw(TransactionRequest request);
        Task<IEnumerable<TransactionResponse>> GetTransactionsForAccountAsync(int accountId);
        Task<TransactionResponse?> GetTransactionDetailsAsync(int accountId, int transactionId);
    }
}
