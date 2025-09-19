using MediatR;
using TransferService.Application.DTO;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;

namespace TransferService.Application.Features.Accounts.Commands.CreateAccount
{
    public class CreateAccountCommandHandler
        : IRequestHandler<CreateAccountCommand, AccountDetailsResponse>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountNumberGenerator _accountNumberGenerator;
        private readonly IPinService _pinService;

        public CreateAccountCommandHandler(
            IAccountRepository accountRepository,
            ICustomerRepository customerRepository,
            IAccountNumberGenerator accountNumberGenerator,
            IPinService pinService
        )
        {
            _accountRepository = accountRepository;
            _customerRepository = customerRepository;
            _accountNumberGenerator = accountNumberGenerator;
            _pinService = pinService;
        }

        public async Task<AccountDetailsResponse> Handle(
            CreateAccountCommand request,
            CancellationToken cancellationToken
        )
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer == null)
                throw new ArgumentException("Customer not found");

            var accountNumber = await _accountNumberGenerator.GenerateAsync(
                request.Account.BankCode,
                request.Account.SchemeCode
            );

            var account = new Account(
                accountName: request.OwnerName,
                username: request.Username,
                type: request.Account.Type,
                balance: request.Account.Balance,
                tier: request.Account.Tier,
                currency: request.Account.Currency,
                customerId: request.CustomerId,
                accountNumber: accountNumber,
                pin: request.Account.Pin
            );
            _pinService.SetPin(account, request.Account.Pin);

            await _accountRepository.AddAsync(account);

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
