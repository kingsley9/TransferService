using FluentAssertions;
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
                AccountTier.TierOne,
                "NGN",
                Guid.NewGuid(),
                accountNumber,
                pin
            );
        }

        [Fact]
        public void Deposit_ShouldUpdateBalanceAndReturnTransaction()
        {
            var account = CreateTierOneAccount(balance: 5000);

            var transaction = account.Deposit(2000);

            account.Balance.Should().Be(7000);
            transaction.Type.Should().Be(TransactionType.Deposit);
        }

        [Fact]
        public void Withdraw_ShouldUpdateBalanceAndReturnTransaction_WhenSufficientFunds()
        {
            var account = CreateTierOneAccount(balance: 5000);

            var transaction = account.Withdraw(1000);

            account.Balance.Should().Be(4000);
            transaction.Type.Should().Be(TransactionType.Withdrawal);
        }

        [Fact]
        public void Withdraw_ShouldThrow_WhenInsufficientFundsOrAccountRestricted()
        {
            var account = CreateTierThreeAccount(balance: 2000);
            var restrictedAccount = CreateTierThreeAccount();
            restrictedAccount.IsPND = true;
            restrictedAccount
                .GetType()
                .GetProperty("Status")!
                .SetValue(restrictedAccount, AccountStatus.Inactive);

            Action withdrawWithInsufficientFunds = () => account.Withdraw(3000);
            Action withdrawOnRestrictedAccount = () => restrictedAccount.Withdraw(1000);

            withdrawWithInsufficientFunds.Should().Throw<InvalidOperationException>();
            withdrawOnRestrictedAccount.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void TransferTo_ShouldMoveFundsBetweenAccounts()
        {
            var fromAccount = CreateTierThreeAccount(
                balance: 5000,
                accountNumber: "1234567890",
                pin: "1234"
            );
            var toAccount = CreateTierThreeAccount(
                balance: 2000,
                accountNumber: "0987654321",
                pin: "5678"
            );

            var transaction = fromAccount.TransferTo(1000, toAccount);

            fromAccount.Balance.Should().Be(4000);
            toAccount.Balance.Should().Be(3000);
            transaction.Type.Should().Be(TransactionType.Transfer);
        }

        [Fact]
        public void AccountCreation_ShouldThrow_WhenInvalidParameters()
        {
            Action createWithLowBalance = () =>
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
                );
            Action createWithInvalidAccountNumber = () =>
                CreateTierThreeAccount(5000, "12345", "1234");
            Action createWithInvalidPin = () => CreateTierThreeAccount(5000, "1234567890", "12");

            createWithLowBalance.Should().Throw<InvalidOperationException>();
            createWithInvalidAccountNumber.Should().Throw<ArgumentException>();
            createWithInvalidPin.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdatePin_ShouldChangePinAndValidateCorrectly()
        {
            var account = CreateTierThreeAccount(pin: "1234");

            account.UpdatePin("9999");

            account.IsPinMatch("9999").Should().BeTrue();
            account.IsPinMatch("1234").Should().BeFalse();
        }

        [Fact]
        public void Deposit_ShouldRespectLienAndUpdateBalance()
        {
            var account = CreateTierThreeAccount(balance: 5000);
            account.ApplyLien(500);

            account.Deposit(1000);

            account.Balance.Should().Be(5500);
            account.LienAmount.Should().Be(0);
        }
    }
}
