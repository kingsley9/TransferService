using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransferService.Domain.Enums;

namespace TransferService.Domain.Entities
{
    public class AccountRule
    {
        public AccountType Type { get; }
        public AccountTier Tier { get; }
        public string Currency { get; }
        public decimal? MinBalance { get; }
        public decimal? MaxBalance { get; }
        public decimal? MaxSingleDeposit { get; }
        public decimal? MaxWithdrawal { get; }
        public int? MaxMonthlyTransactions { get; }

        public AccountRule(
            AccountType type,
            AccountTier tier,
            string currency,
            decimal? minBalance = null,
            decimal? maxBalance = null,
            decimal? maxSingleDeposit = null,
            decimal? maxWithdrawal = null,
            int? maxMonthlyTransactions = null
        )
        {
            Tier = tier;
            Type = type;
            Currency = currency;
            MinBalance = minBalance;
            MaxBalance = maxBalance;
            MaxSingleDeposit = maxSingleDeposit;
            MaxWithdrawal = maxWithdrawal;
            MaxMonthlyTransactions = maxMonthlyTransactions;
        }
    }
}
