using MediatR;
using TransferService.Application.DTO;
using TransferService.Application.Features.Transactions.Commands.CreateTransfer;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;
using TransferService.Domain.Enums;
using TransferService.Domain.Rules;

namespace TransferService.Application.Features.Transactions.Commands.CreateTransfer
{
    public class CreateTransferCommandHandler
        : IRequestHandler<CreateTransferCommand, TransactionResponse>
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IAccountValidator _accountValidator;
        private readonly IPinService _pinService;

        public CreateTransferCommandHandler(
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
            CreateTransferCommand request,
            CancellationToken cancellationToken
        )
        {
            var r = request.Request;
            var fromAccount = await _accountRepo.GetByIdAsync(r.AccountId);
            var toAccount = r.TargetAccountId.HasValue
                ? await _accountRepo.GetByIdAsync(r.TargetAccountId.Value)
                : null;
            if (fromAccount == null || toAccount == null)
                throw new ArgumentException("One or both accounts not found");

            if (r.AccountId == r.TargetAccountId)
                throw new InvalidOperationException("Cannot transfer to the same account");

            if (!_pinService.VerifyPin(fromAccount, r.Pin))
                throw new UnauthorizedAccessException("Invalid PIN");

            var transferTransaction = Transaction.CreateTransfer(
                fromAccount.AccountId,
                r.Amount,
                toAccount.AccountId
            );

            try
            {
                var fromContext = new AccountValidationContext
                {
                    Account = fromAccount,
                    WithdrawalAmount = r.Amount,
                };

                _accountValidator.Validate(fromContext);

                fromAccount.TransferTo(r.Amount, toAccount);

                await _accountRepo.UpdateAsync(fromAccount);
                await _accountRepo.UpdateAsync(toAccount);

                transferTransaction.SetStatus(TransactionStatus.Success);
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
            catch (Exception ex)
            {
                transferTransaction.SetStatus(TransactionStatus.Failed);
                await _transactionRepo.AddAsync(transferTransaction);
                throw new Exception($"Transfer Failed: {ex.Message}");
            }
        }
    }
}
