using MediatR;
using TransferService.Application.DTO;
using TransferService.Application.Interfaces;

namespace TransferService.Application.Features.Accounts.Queries.GetAccountById
{
    public class GetAccountByIdQueryHandler
        : IRequestHandler<GetAccountByIdQuery, AccountDetailsResponse?>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountByIdQueryHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<AccountDetailsResponse?> Handle(
            GetAccountByIdQuery request,
            CancellationToken cancellationToken
        )
        {
            Console.WriteLine("Inside mediatR get account by Id");
            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            if (account == null)
                return null;

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
