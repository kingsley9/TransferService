using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;

namespace TransferService.Infrastructure.Services
{
    public class PinService : IPinService
    {
        private readonly PasswordHasher<Account> _passwordHasher = new();

        public void SetPin(Account account, string rawPin)
        {
            if (!Account.IsValidPin(rawPin))
                throw new ArgumentException("Invalid PIN");

            account.UpdatePin(_passwordHasher.HashPassword(account, rawPin));
        }

        public void ChangePin(Account account, string currentPin, string newPin)
        {
            if (account is null)
                throw new ArgumentNullException("Account must not be null");
            var result = _passwordHasher.VerifyHashedPassword(account, account.PinHash, currentPin);
            if (result == PasswordVerificationResult.Failed)
                throw new ArgumentException("Current PIN is incorrect");

            SetPin(account, newPin);
        }

        public bool VerifyPin(Account account, string rawPin)
        {
            var result = _passwordHasher.VerifyHashedPassword(account, account.PinHash, rawPin);
            return result == PasswordVerificationResult.Success;
        }
    }
}
