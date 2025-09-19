using TransferService.Domain.Enums;
using TransferService.Domain.Rules;

namespace TransferService.Domain.Entities
{
    public class Account
    {
        public int AccountId { get; internal set; }
        public string AccountName { get; set; } = String.Empty;
        public decimal Balance { get; private set; }
        public string Username { get; private set; } = String.Empty;
        public AccountType Type { get; private set; }
        public string AccountNumber { get; private set; } = String.Empty;
        public AccountTier Tier { get; private set; }
        public AccountStatus Status { get; private set; }
        public bool IsPND { get; set; }
        public Guid CustomerId { get; private set; }
        public Customer Customer { get; private set; } = null!;

        public string BankCode { get; set; } = "035";
        public string SchemeCode { get; set; } = String.Empty;
        public decimal TotalWithdrawals { get; private set; }
        public decimal TotalLodgements { get; private set; }
        public string Currency { get; private set; } = "NGN";
        public decimal LienAmount { get; private set; }
        public decimal AvailableBalance => Balance - LienAmount;

        public string PinHash { get; private set; } = String.Empty;
        private readonly Dictionary<(AccountTier, string), decimal> MinimumBalances = new()
        {
            { (AccountTier.TierThree, "NGN"), 2000m },
        };

        public Account() { }

        public Account(
            string accountName,
            string username,
            AccountType type,
            decimal balance,
            AccountTier tier,
            string currency,
            Guid customerId,
            string accountNumber,
            string pin
        )
        {
            Status = AccountStatus.Active;
            if (
                MinimumBalances.TryGetValue((tier, currency), out var minBalance)
                && balance < minBalance
            )
                throw new InvalidOperationException(
                    "Opening Balance for Tier 3 Naira account must be greater than NGN 2,000.00"
                );
            if (!IsValidAccountNumber(accountNumber))
                throw new ArgumentException("Account number is invalid");
            if (!IsValidPin(pin))
                throw new ArgumentException("Your pin must be 4 digits");

            AccountName = accountName;
            Username = username;
            Type = type;
            Balance = balance;
            Tier = tier;
            CustomerId = customerId;
            Currency = currency;
            AccountNumber = accountNumber;
            PinHash = pin;
        }

        public Transaction Deposit(decimal amount, bool isTransfer = false)
        {
            IsAccountActive();
            var lienDeduction = DeductLien(amount);
            Balance += amount - lienDeduction;

            TotalLodgements += amount;
            return Transaction.CreateDeposit(AccountId, amount);
        }

        public Transaction Withdraw(decimal amount)
        {
            IsAccountActive();
            IsAccountPND();
            if (AvailableBalance < amount)
                throw new InvalidOperationException("Insuffiecient funds");

            Balance -= amount;
            TotalWithdrawals += amount;
            return Transaction.CreateWithdrawal(AccountId, amount);
        }

        public Transaction TransferTo(decimal amount, Account targetAccount)
        {
            IsAccountActive();
            IsAccountPND();
            if (targetAccount == null)
                throw new ArgumentNullException(nameof(targetAccount));
            if (Balance < amount)
                throw new InvalidOperationException("Insufficient funds");

            Withdraw(amount);
            targetAccount.Deposit(amount, true);
            targetAccount.TotalLodgements += amount;
            return Transaction.CreateTransfer(AccountId, amount, targetAccount.AccountId);
        }

        private static bool IsValidAccountNumber(string accountNumber)
        {
            return !string.IsNullOrWhiteSpace(accountNumber) && accountNumber.Length == 10;
        }

        public void UpdatePin(string pinHash)
        {
            PinHash = pinHash;
        }

        public bool IsPinMatch(string pinHash)
        {
            return PinHash == pinHash;
        }

        private void IsAccountActive()
        {
            if (Status == AccountStatus.Inactive || Status == AccountStatus.Closed)
                throw new InvalidOperationException("Account is not active.");
        }

        private void IsAccountPND()
        {
            if (IsPND)
                throw new InvalidOperationException(
                    "Account is restricted from debit transactions."
                );
        }

        public static bool IsValidPin(string pin)
        {
            return !String.IsNullOrEmpty(pin) && pin.Length == 4 && pin.All(char.IsDigit);
        }

        public void ApplyLien(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Lien amount cannot be negative");

            LienAmount += amount;
        }

        public decimal DeductLien(decimal amount)
        {
            var lienDeduction = Math.Min(LienAmount, amount);
            LienAmount -= lienDeduction;
            return lienDeduction;
        }

        public void Update(string? accountName, AccountType? type, AccountTier? tier)
        {
            if (accountName != null)
                AccountName = accountName;
            if (type.HasValue)
                Type = type.Value;
            if (tier.HasValue)
                Tier = tier.Value;
        }
    }
}
