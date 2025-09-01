using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransferService.Domain.Entities;
using TransferService.Domain.Enums;

namespace TransferService.Domain.Rules
{
    public class MaxBalanceTierTwoRule : IAccountRule
    {
        public AccountType? AccountType => Enums.AccountType.Savings;
        public AccountTier? Tier => AccountTier.TierTwo;
        public string? Currency => "NGN";

        public void Validate(AccountValidationContext context)
        {
            if (context.Account.Balance > 500_000m)
                throw new InvalidOperationException(
                    "Account balance exceeds max allowed for Tier Two Savings."
                );
        }
    }
}
