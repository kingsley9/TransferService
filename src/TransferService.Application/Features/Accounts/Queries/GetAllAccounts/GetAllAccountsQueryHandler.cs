using MediatR;
using TransferService.Application.DTO;
using TransferService.Application.Interfaces;

namespace TransferService.Application.Features.Accounts.Queries.GetAllAccounts
{
    public class GetAllAccountsQueryHandler
        : IRequestHandler<GetAllAccountsQuery, IEnumerable<AccountDetailsResponse>>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAllAccountsQueryHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<IEnumerable<AccountDetailsResponse>> Handle(
            GetAllAccountsQuery request,
            CancellationToken cancellationToken
        )
        {
            var accounts = await _accountRepository.GetAllAsync();
            return accounts.Select(account => new AccountDetailsResponse
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
            });
        }
    }
}
