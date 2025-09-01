using TransferService.Domain.Enums;
using TransferService.Domain.Exceptions;

namespace TransferService.Domain.Entities
{
    public class Transaction
    {
        public int TransactionId { get; private set; }
        public int AccountId { get; private set; }
        public int? TargetAccountId { get; private set; }
        public decimal Amount { get; private set; }
        public TransactionType Type { get; private set; }
        public TransactionStatus Status { get; private set; }
        public DateTime Timestamp { get; private set; }

        private Transaction() { }

        private Transaction(
            int accountId,
            decimal amount,
            TransactionType type,
            int? targetAccountId = null
        )
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must not be less than zero");
            AccountId = accountId;
            TargetAccountId = targetAccountId;
            Amount = amount;
            Type = type;
            Timestamp = DateTime.UtcNow;
        }

        public static Transaction CreateDeposit(int accountId, decimal amount)
        {
            return new Transaction(accountId, amount, TransactionType.Deposit);
        }

        public static Transaction CreateWithdrawal(int accountId, decimal amount)
        {
            return new Transaction(accountId, amount, TransactionType.Withdrawal);
        }

        public static Transaction CreateTransfer(int accountId, decimal amount, int targetAccountId)
        {
            return new Transaction(accountId, amount, TransactionType.Transfer, targetAccountId);
        }
    }
}
