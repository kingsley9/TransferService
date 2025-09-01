using System;
using TransferService.Domain.Entities;
using TransferService.Domain.Enums;

namespace TransferService.Domain.Rules
{
    public class MaxBalanceTierOneRule : IAccountRule
    {
        public AccountType? AccountType => Enums.AccountType.Savings;
        public AccountTier? Tier => AccountTier.TierOne;
        public string? Currency => "NGN";

        public void Validate(AccountValidationContext context)
        {
            if (context.Account.Balance > 300_000m)
                throw new InvalidOperationException(
                    "Account balance exceeds max allowed for Tier One Savings."
                );
        }
    }
}
