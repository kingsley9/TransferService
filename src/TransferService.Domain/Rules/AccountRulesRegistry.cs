using TransferService.Domain.Entities;

namespace TransferService.Domain.Rules
{
    using TransferService.Domain.Enums;

    public static class AccountRulesRegistry
    {
        private static readonly List<AccountRule> _rules = new()
        {
            // Savings Tier 1
            new AccountRule(
                AccountType.Savings,
                AccountTier.TierOne,
                "NGN",
                maxSingleDeposit: 50_000m,
                maxBalance: 300_000m
            ),
            // Savings Tier 2
            new AccountRule(
                AccountType.Savings,
                AccountTier.TierTwo,
                "NGN",
                maxSingleDeposit: 100_000m,
                maxBalance: 500_000m
            ),
            // Savings Tier 3 (no limits)
            new AccountRule(AccountType.Savings, AccountTier.TierThree, "NGN"),
            // Current accounts are only Tier 3 (no limits)
            new AccountRule(AccountType.Current, AccountTier.TierThree, "NGN"),
        };

        public static AccountRule? GetRule(
            AccountType type,
            AccountTier tier,
            string currency = "NGN"
        )
        {
            return _rules.FirstOrDefault(r =>
                r.Type == type && r.Tier == tier && r.Currency == currency
            );
        }
    }
}
