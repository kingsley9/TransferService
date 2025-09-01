using Microsoft.AspNetCore.Identity;
using TransferService.Application.DTO;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;
using TransferService.Domain.Enums;
using TransferService.Domain.Rules;

namespace TransferService.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly AccountValidator _accountValidator;
        private readonly PasswordHasher<Account> _passwordHasher = new();

        public TransactionService(
            ITransactionRepository transactionRepo,
            IAccountRepository accountRepo,
            AccountValidator accountValidator
        )
        {
            _transactionRepo = transactionRepo;
            _accountRepo = accountRepo;
            _accountValidator = accountValidator;
        }

        public async Task<TransactionResponse> TransferAsync(TransactionRequest request)
        {
            var fromAccount = await _accountRepo.GetByIdAsync(request.AccountId);
            var toAccount = request.TargetAccountId.HasValue
                ? await _accountRepo.GetByIdAsync(request.TargetAccountId.Value)
                : null;
            if (fromAccount == null || toAccount == null)
                throw new ArgumentException("One or both accounts not found");

            if (request.AccountId == request.TargetAccountId)
                throw new InvalidOperationException(
                    "You cannot transfer to the same account you are transferring from"
                );

            if (!VerifyPin(fromAccount, request.Pin))
                throw new UnauthorizedAccessException("Invalid PIN");
            var fromContext = new AccountValidationContext
            {
                Account = fromAccount,
                DepositAmount = request.Amount,
            };

            _accountValidator.Validate(fromContext);

            var transferTransaction = fromAccount.TransferTo(request.Amount, toAccount);

            await _accountRepo.UpdateAsync(fromAccount);
            await _accountRepo.UpdateAsync(toAccount);

            await _transactionRepo.AddAsync(transferTransaction);

            return new TransactionResponse
            {
                TransactionId = transferTransaction.TransactionId,
                Balance = fromAccount.Balance,
                Status = TransactionStatus.Success,
                Type = TransactionType.Transfer,
                Timestamp = transferTransaction.Timestamp,
            };
        }

        public async Task<TransactionResponse> Deposit(TransactionRequest request)
        {
            var account = await _accountRepo.GetByIdAsync(request.AccountId);
            if (account is null)
                throw new Exception($"Account {request.AccountId} does not exist");
            if (!VerifyPin(account, request.Pin))
                throw new UnauthorizedAccessException("Invalid PIN");
            var depositContext = new AccountValidationContext
            {
                Account = account,
                DepositAmount = request.Amount,
            };
            _accountValidator.Validate(depositContext);
            var depositTransaction = account.Deposit(request.Amount, false);

            await _accountRepo.UpdateAsync(account);

            await _transactionRepo.AddAsync(depositTransaction);

            return new TransactionResponse
            {
                TransactionId = depositTransaction.TransactionId,
                Status = TransactionStatus.Success,
                Balance = account.Balance,
                Type = TransactionType.Deposit,
                Timestamp = depositTransaction.Timestamp,
            };
        }

        public async Task<TransactionResponse> Withdraw(TransactionRequest request)
        {
            var account = await _accountRepo.GetByIdAsync(request.AccountId);

            if (account is null)
                throw new Exception($"Account {request.AccountId} does not exist");

            if (!VerifyPin(account, request.Pin))
                throw new UnauthorizedAccessException("Invalid PIN");

            var withdrawContext = new AccountValidationContext
            {
                Account = account,
                WithdrawalAmount = request.Amount,
            };
            _accountValidator.Validate(withdrawContext);

            var withdrawalTransaction = account.Withdraw(request.Amount);

            await _accountRepo.UpdateAsync(account);

            await _transactionRepo.AddAsync(withdrawalTransaction);
            Console.WriteLine("Withdraw called, account:" + account);
            return new TransactionResponse
            {
                TransactionId = withdrawalTransaction.TransactionId,
                Status = TransactionStatus.Success,
                Balance = account.Balance,
                Type = TransactionType.Withdrawal,
                Timestamp = withdrawalTransaction.Timestamp,
            };
        }

        public async Task<IEnumerable<TransactionResponse>> GetTransactionsForAccountAsync(
            int accountId
        )
        {
            var transactions = await _transactionRepo.GetByAccountIdAsync(accountId);

            return transactions.Select(t => new TransactionResponse
            {
                TransactionId = t.TransactionId,
                Status = t.Status,
                Type = t.Type,
                Timestamp = t.Timestamp,
            });
        }

        public async Task<TransactionResponse?> GetTransactionDetailsAsync(
            int accountId,
            int transactionId
        )
        {
            var transaction = await _transactionRepo.GetByIdAsync(transactionId);
            if (transaction == null)
                return null;

            var isOwner =
                (transaction.AccountId == accountId) || (transaction.TargetAccountId == accountId);
            if (!isOwner)
                return null;

            var account = await _accountRepo.GetByIdAsync(accountId);

            return new TransactionResponse
            {
                TransactionId = transaction.TransactionId,
                Status = TransactionStatus.Success,
                Timestamp = transaction.Timestamp,
                Balance = account.Balance,
            };
        }

        public bool VerifyPin(Account account, string rawPin)
        {
            var result = _passwordHasher.VerifyHashedPassword(account, account.PinHash, rawPin);
            return result == PasswordVerificationResult.Success;
        }
    }
}
