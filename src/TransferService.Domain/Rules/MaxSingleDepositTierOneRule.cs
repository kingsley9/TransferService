using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransferService.Domain.Entities;
using TransferService.Domain.Enums;

namespace TransferService.Domain.Rules
{
    public class MaxSingleDepositTierOneRule : IAccountRule
    {
        public AccountType? AccountType => Enums.AccountType.Savings;
        public AccountTier? Tier => AccountTier.TierOne;
        public string? Currency => "NGN";

        public void Validate(AccountValidationContext context)
        {
            if (context.DepositAmount.HasValue && context.DepositAmount.Value > 50_000m)
                throw new InvalidOperationException(
                    "Deposit exceeds max allowed for Tier One Savings."
                );
        }
    }
}
