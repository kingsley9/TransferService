using MediatR;
using TransferService.Application.Interfaces;

namespace TransferService.Application.Features.Accounts.Commands.ChangeAccountPin
{
    public class ChangeAccountPinCommandHandler : IRequestHandler<ChangeAccountPinCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPinService _pinService;

        public ChangeAccountPinCommandHandler(
            IAccountRepository accountRepository,
            IPinService pinService
        )
        {
            _accountRepository = accountRepository;
            _pinService = pinService;
        }

        public async Task Handle(
            ChangeAccountPinCommand request,
            CancellationToken cancellationToken
        )
        {
            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            if (account == null)
                throw new ArgumentException("Account not found");

            _pinService.ChangePin(account, request.CurrentPin, request.NewPin);
            await _accountRepository.UpdateAsync(account);
        }
    }
}
