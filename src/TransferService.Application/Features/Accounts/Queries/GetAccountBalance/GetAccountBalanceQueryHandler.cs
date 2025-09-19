using MediatR;
using TransferService.Application.Interfaces;

namespace TransferService.Application.Features.Accounts.Queries.GetAccountBalance
{
    public class GetAccountBalanceQueryHandler : IRequestHandler<GetAccountBalanceQuery, decimal?>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountBalanceQueryHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<decimal?> Handle(
            GetAccountBalanceQuery request,
            CancellationToken cancellationToken
        )
        {
            Console.WriteLine("Inside mediatR get account by Id");

            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            return account?.Balance;
        }
    }
}
