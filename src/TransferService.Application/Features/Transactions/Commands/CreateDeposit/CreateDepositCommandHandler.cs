using MediatR;
using TransferService.Application.DTO;
using TransferService.Application.Features.Transactions.Commands.CreateDeposit;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;
using TransferService.Domain.Enums;
using TransferService.Domain.Rules;

namespace TransferService.Application.Features.Transactions.Commands.CreateDeposit
{
    public class CreateDepositCommandHandler
        : IRequestHandler<CreateDepositCommand, TransactionResponse>
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IAccountValidator _accountValidator;
        private readonly IPinService _pinService;

        public CreateDepositCommandHandler(
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
            CreateDepositCommand request,
            CancellationToken cancellationToken
        )
        {
            var r = request.Request;
            var account = await _accountRepo.GetByIdAsync(r.AccountId);
            if (account == null)
                throw new ArgumentException($"Account {r.AccountId} does not exist");

            if (!_pinService.VerifyPin(account, r.Pin))
                throw new UnauthorizedAccessException("Invalid PIN");

            var depositTransaction = Transaction.CreateDeposit(account.AccountId, r.Amount);

            try
            {
                var depositContext = new AccountValidationContext
                {
                    Account = account,
                    DepositAmount = r.Amount,
                };
                _accountValidator.Validate(depositContext);

                account.Deposit(r.Amount);

                await _accountRepo.UpdateAsync(account);

                depositTransaction.SetStatus(TransactionStatus.Success);
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
            catch (Exception ex)
            {
                depositTransaction.SetStatus(TransactionStatus.Failed);
                await _transactionRepo.AddAsync(depositTransaction);
                throw new Exception($"Deposit Failed: {ex.Message}");
            }
        }
    }
}
