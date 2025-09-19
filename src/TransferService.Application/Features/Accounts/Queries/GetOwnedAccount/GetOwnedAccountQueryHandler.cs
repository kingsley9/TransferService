using MediatR;
using TransferService.Application.DTO;
using TransferService.Application.Interfaces;
using TransferService.Domain.Exceptions;

namespace TransferService.Application.Features.Accounts.Queries.GetOwnedAccount
{
    public class GetOwnedAccountQueryHandler
        : IRequestHandler<GetOwnedAccountQuery, AccountDetailsResponse>
    {
        private readonly IAccountRepository _accountRepository;

        public GetOwnedAccountQueryHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<AccountDetailsResponse> Handle(
            GetOwnedAccountQuery request,
            CancellationToken cancellationToken
        )
        {
            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            if (account == null)
                throw new NotFoundException("Account not found");
            if (account.Username != request.Username)
                throw new ForbiddenException("You do not own this account");

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
