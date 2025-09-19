using MediatR;
using TransferService.Application.DTO;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;

namespace TransferService.Application.Features.Accounts.Commands.UpdateAccount
{
    public class UpdateAccountCommandHandler
        : IRequestHandler<UpdateAccountCommand, AccountDetailsResponse?>
    {
        private readonly IAccountRepository _accountRepository;

        public UpdateAccountCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<AccountDetailsResponse?> Handle(
            UpdateAccountCommand request,
            CancellationToken cancellationToken
        )
        {
            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            if (account == null)
                return null;

            account.Update(request.AccountName, request.Type, request.Tier);
            await _accountRepository.UpdateAsync(account);

            return new AccountDetailsResponse
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                Username = account.Username,
                Type = account.Type,
                Balance = account.Balance,
                Tier = account.Tier,
                Currency = account.Currency,
                CustomerId = account.CustomerId,
                AccountNumber = account.AccountNumber,
            };
        }
    }
}
