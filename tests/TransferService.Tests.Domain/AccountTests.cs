using TransferService.Domain.Entities;
using TransferService.Domain.Enums;
using Xunit;

namespace TransferService.Tests.Domain
{
    public class AccountTests
    {
        private Account CreateTierThreeAccount(
            decimal balance = 5000,
            string accountNumber = "1234567890",
            string pin = "1234"
        )
        {
            return new Account(
                "TestAccount",
                "user1",
                AccountType.Savings,
                balance,
                AccountTier.TierThree,
                "NGN",
                Guid.NewGuid(),
                accountNumber,
                pin
            );
        }

        private Account CreateTierOneAccount(
            decimal balance = 500,
            string accountNumber = "1234567890",
            string pin = "1234"
        )
        {
            return new Account(
                "TestAccount",
                "user1",
                AccountType.Savings,
                balance,
                AccountTier.TierOne,
                "NGN",
                Guid.NewGuid(),
                accountNumber,
                pin
            );
        }

        [Fact]
        public void Deposit_ShouldIncreaseBalance()
        {
            var account = CreateTierThreeAccount();

            var transaction = account.Deposit(2000);

            Assert.Equal(7000, account.Balance);
            Assert.Equal(TransactionType.Deposit, transaction.Type);
        }

        [Fact]
        public void Withdraw_ShouldDecreaseBalance()
        {
            var account = CreateTierThreeAccount();

            var transaction = account.Withdraw(1000);

            Assert.Equal(4000, account.Balance);
            Assert.Equal(TransactionType.Withdrawal, transaction.Type);
        }

        [Fact]
        public void Withdraw_ShouldThrow_WhenInsufficientFunds()
        {
            var account = CreateTierThreeAccount(2000);

            Assert.Throws<InvalidOperationException>(() => account.Withdraw(3000));
        }

        [Fact]
        public void TransferTo_ShouldMoveFundsBetweenAccounts()
        {
            var fromAccount = CreateTierThreeAccount(5000, "1234567890", "1234");
            var toAccount = CreateTierThreeAccount(2000, "0987654321", "5678");

            var transaction = fromAccount.TransferTo(1000, toAccount);

            Assert.Equal(4000, fromAccount.Balance);
            Assert.Equal(3000, toAccount.Balance);
            Assert.Equal(TransactionType.Transfer, transaction.Type);
        }

        [Fact]
        public void ShouldThrow_WhenOpeningBalanceLessThanMinForTier3Naira()
        {
            Assert.Throws<InvalidOperationException>(() =>
                new Account(
                    "MyAccount",
                    "user1",
                    AccountType.Savings,
                    1000,
                    AccountTier.TierThree,
                    "NGN",
                    Guid.NewGuid(),
                    "1234567890",
                    "1234"
                )
            );
        }

        [Fact]
        public void ShouldThrow_WhenAccountNumberInvalid()
        {
            Assert.Throws<ArgumentException>(() => CreateTierThreeAccount(5000, "12345", "1234"));
        }

        [Fact]
        public void ShouldThrow_WhenPinInvalid()
        {
            Assert.Throws<ArgumentException>(() =>
                CreateTierThreeAccount(5000, "1234567890", "12")
            );
        }

        [Fact]
        public void UpdatePin_ShouldChangePin()
        {
            var account = CreateTierThreeAccount();

            account.UpdatePin("9999");

            Assert.True(account.IsPinMatch("9999"));
            Assert.False(account.IsPinMatch("1234"));
        }

        [Fact]
        public void Withdraw_ShouldThrow_WhenAccountIsPND()
        {
            var account = CreateTierThreeAccount();
            account.IsPND = true;

            Assert.Throws<InvalidOperationException>(() => account.Withdraw(1000));
        }

        [Fact]
        public void Deposit_ShouldRespectLien()
        {
            var account = CreateTierThreeAccount();
            account.ApplyLien(500);

            account.Deposit(1000);

            Assert.Equal(5500, account.Balance);
            Assert.Equal(0, account.LienAmount);
        }

        [Fact]
        public void Deposit_ShouldThrow_WhenExceedsMaxSingleDeposit()
        {
            var account = CreateTierOneAccount();

            Assert.Throws<InvalidOperationException>(() => account.Deposit(1_000_000m));
        }

        [Fact]
        public void Deposit_ShouldThrow_WhenExceedsMaxBalance()
        {
            var account = CreateTierOneAccount(balance: 999_000);

            Assert.Throws<InvalidOperationException>(() => account.Deposit(100_000));
        }

        [Fact]
        public void Withdraw_ShouldThrow_WhenAccountInactive()
        {
            var account = CreateTierThreeAccount();
            account.GetType().GetProperty("Status")!.SetValue(account, AccountStatus.Inactive);

            Assert.Throws<InvalidOperationException>(() => account.Withdraw(100));
        }
    }
}
