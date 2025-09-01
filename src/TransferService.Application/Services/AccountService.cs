using Microsoft.AspNetCore.Identity;
using TransferService.Application.DTO;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;
using TransferService.Domain.Exceptions;

namespace TransferService.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly IAccountNumberGenerator _accountNumberGenerator;
        private readonly PasswordHasher<Account> _passwordHasher = new();

        public AccountService(
            IAccountRepository accountRepository,
            IAccountNumberGenerator accountNumberGenerator
        )
        {
            _accountRepo = accountRepository;
            _accountNumberGenerator = accountNumberGenerator;
        }

        public async Task<Account?> GetAccountByIdAsync(int accountId) =>
            await _accountRepo.GetByIdAsync(accountId);

        public async Task<IEnumerable<Account>> GetAllAccountsAsync() =>
            await _accountRepo.GetAllAsync();

        public async Task<Account> CreateAccountAsync(
            AccountRequest account,
            string ownerName,
            string username,
            Guid customerId
        )
        {
            var accountNumber = await _accountNumberGenerator.GenerateAsync(
                account.BankCode,
                account.SchemeCode
            );

            var newAccount = new Account(
                ownerName,
                username,
                account.Type,
                account.Balance,
                account.Tier,
                account.Currency,
                customerId,
                accountNumber,
                account.Pin
            );
            SetPin(newAccount, account.Pin);

            await _accountRepo.AddAsync(newAccount);
            return newAccount;
        }

        public async Task<bool> UpdateAccountAsync(Account updatedAccount)
        {
            var existingAccount = await _accountRepo.GetByIdAsync(updatedAccount.AccountId);
            if (existingAccount is null)
                return false;

            existingAccount.AccountName = updatedAccount.AccountName;

            await _accountRepo.UpdateAsync(existingAccount);
            return true;
        }

        public async Task<bool> DeleteAccountAsync(int accountId)
        {
            var account = await _accountRepo.GetByIdAsync(accountId);
            if (account is null)
                return false;

            await _accountRepo.DeleteAsync(accountId);
            return true;
        }

        public async Task<decimal?> GetAccountBalanceAsync(int accountId)
        {
            var account = await _accountRepo.GetByIdAsync(accountId);
            return account?.Balance;
        }

        public async Task<Account> GetOwnedAccountAsync(int accountId, string username)
        {
            var account = await _accountRepo.GetByIdAsync(accountId);
            if (account == null)
                throw new NotFoundException("Account not found");
            if (account.Username != username)
                throw new ForbiddenException("You do not own this account");

            return account;
        }

        public void SetPin(Account account, string rawPin)
        {
            var hashedPin = _passwordHasher.HashPassword(account, rawPin);
            account.UpdatePin(hashedPin);
        }
    }
}
