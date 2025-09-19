using MediatR;
using TransferService.Application.Interfaces;

namespace TransferService.Application.Features.Accounts.Commands.DeleteAccount
{
    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, bool>
    {
        private readonly IAccountRepository _accountRepository;

        public DeleteAccountCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<bool> Handle(
            DeleteAccountCommand request,
            CancellationToken cancellationToken
        )
        {
            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            if (account == null)
                return false;

            await _accountRepository.DeleteAsync(request.AccountId);
            return true;
        }
    }
}
