using MediatR;
using TransferService.Application.DTO;
using TransferService.Application.Interfaces;

namespace TransferService.Application.Features.Accounts.Queries.GetAccountsByCustomer
{
    public class GetAccountsByCustomerQueryHandler
        : IRequestHandler<GetAccountsByCustomerQuery, IEnumerable<AccountDetailsResponse>>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountsByCustomerQueryHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<IEnumerable<AccountDetailsResponse>> Handle(
            GetAccountsByCustomerQuery request,
            CancellationToken cancellationToken
        )
        {
            var accounts = await _accountRepository.GetByCustomerIdAsync(request.CustomerId);
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
