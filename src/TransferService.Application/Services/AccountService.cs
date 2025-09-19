using MediatR;
using TransferService.Application.DTO;
using TransferService.Application.Features.Accounts.Commands.ChangeAccountPin;
using TransferService.Application.Features.Accounts.Commands.CreateAccount;
using TransferService.Application.Features.Accounts.Commands.DeleteAccount;
using TransferService.Application.Features.Accounts.Commands.UpdateAccount;
using TransferService.Application.Features.Accounts.Queries.GetAccountBalance;
using TransferService.Application.Features.Accounts.Queries.GetAccountById;
using TransferService.Application.Features.Accounts.Queries.GetAllAccounts;
using TransferService.Application.Features.Accounts.Queries.GetOwnedAccount;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;

namespace TransferService.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IMediator _mediator;

        public AccountService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<AccountDetailsResponse?> GetAccountByIdAsync(int accountId)
        {
            return await _mediator.Send(new GetAccountByIdQuery(accountId));
        }

        public async Task<IEnumerable<AccountDetailsResponse>> GetAllAccountsAsync()
        {
            return await _mediator.Send(new GetAllAccountsQuery());
        }

        public async Task<AccountDetailsResponse> CreateAccountAsync(
            AccountRequest account,
            string ownerName,
            string username,
            Guid customerId
        )
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            var command = new CreateAccountCommand
            {
                Account = account,
                OwnerName = ownerName,
                Username = username,
                CustomerId = customerId,
            };
            return await _mediator.Send(command);
        }

        public async Task<bool> UpdateAccountAsync(Account updatedAccount)
        {
            var command = new UpdateAccountCommand
            {
                AccountId = updatedAccount.AccountId,
                AccountName = updatedAccount.AccountName,
                Type = updatedAccount.Type,
                Tier = updatedAccount.Tier,
            };
            var result = await _mediator.Send(command);
            return result != null;
        }

        public async Task<bool> DeleteAccountAsync(int accountId)
        {
            return await _mediator.Send(new DeleteAccountCommand(accountId));
        }

        public async Task<decimal?> GetAccountBalanceAsync(int accountId)
        {
            return await _mediator.Send(new GetAccountBalanceQuery(accountId));
        }

        public async Task<AccountDetailsResponse> GetOwnedAccountAsync(
            int accountId,
            string username
        )
        {
            return await _mediator.Send(new GetOwnedAccountQuery(accountId, username));
        }

        public async Task ChangeAccountPinAsync(Account account, string currentPin, string newPin)
        {
            var command = new ChangeAccountPinCommand
            {
                AccountId = account.AccountId,
                CurrentPin = currentPin,
                NewPin = newPin,
            };
            await _mediator.Send(command);
        }
    }
}
