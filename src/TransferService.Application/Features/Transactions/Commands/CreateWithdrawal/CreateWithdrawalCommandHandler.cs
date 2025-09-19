using MediatR;
using TransferService.Application.DTO;
using TransferService.Application.Features.Transactions.Commands.CreateWithdrawal;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;
using TransferService.Domain.Enums;
using TransferService.Domain.Rules;

namespace TransferService.Application.Features.Transactions.Commands.CreateWithdrawal
{
    public class CreateWithdrawalCommandHandler
        : IRequestHandler<CreateWithdrawalCommand, TransactionResponse>
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IAccountValidator _accountValidator;
        private readonly IPinService _pinService;

        public CreateWithdrawalCommandHandler(
            ITransactionRepository transactionRepo,
            IAccountRepository accountRepo,
            IAccountValidator accountValidator,
            IPinService pinService
        )
        {
            _transactionRepo = transactionRepo;
            _accountRepo = accountRepo;
            _accountValidator = accountValidator;
            _pinService = pinService;
        }

        public async Task<TransactionResponse> Handle(
            CreateWithdrawalCommand request,
            CancellationToken cancellationToken
        )
        {
            var r = request.Request;
            var account = await _accountRepo.GetByIdAsync(r.AccountId);
            if (account == null)
                throw new ArgumentException($"Account {r.AccountId} does not exist");

            if (!_pinService.VerifyPin(account, r.Pin))
                throw new UnauthorizedAccessException("Invalid PIN");

            var withdrawalTransaction = Transaction.CreateWithdrawal(account.AccountId, r.Amount);

            try
            {
                var withdrawContext = new AccountValidationContext
                {
                    Account = account,
                    WithdrawalAmount = r.Amount,
                };
                _accountValidator.Validate(withdrawContext);

                account.Withdraw(r.Amount);

                await _accountRepo.UpdateAsync(account);

                withdrawalTransaction.SetStatus(TransactionStatus.Success);
                await _transactionRepo.AddAsync(withdrawalTransaction);

                return new TransactionResponse
                {
                    TransactionId = withdrawalTransaction.TransactionId,
                    Status = TransactionStatus.Success,
                    Balance = account.Balance,
                    Type = TransactionType.Withdrawal,
                    Timestamp = withdrawalTransaction.Timestamp,
                };
            }
            catch (Exception ex)
            {
                withdrawalTransaction.SetStatus(TransactionStatus.Failed);
                await _transactionRepo.AddAsync(withdrawalTransaction);
                throw new Exception($"Withdrawal Failed: {ex.Message}");
            }
        }
    }
}
