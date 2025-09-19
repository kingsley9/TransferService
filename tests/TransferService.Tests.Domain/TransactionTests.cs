using System;
using System.Reflection;
using FluentAssertions;
using TransferService.Domain.Entities;
using TransferService.Domain.Enums;
using Xunit;

namespace TransferService.Tests.Domain
{
    public class TransactionTests
    {
        private Transaction CreateDepositTransaction(int accountId = 1, decimal amount = 500)
        {
            return Transaction.CreateDeposit(accountId, amount);
        }

        private Transaction CreateWithdrawalTransaction(int accountId = 1, decimal amount = 300)
        {
            return Transaction.CreateWithdrawal(accountId, amount);
        }

        private Transaction CreateTransferTransaction(
            int accountId = 1,
            decimal amount = 1000,
            int targetAccountId = 2
        )
        {
            return Transaction.CreateTransfer(accountId, amount, targetAccountId);
        }

        private Account CreateAccount(
            int accountId,
            decimal balance = 5000,
            string accountNumber = "1234567890",
            string pin = "1234"
        )
        {
            var account = new Account(
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

            typeof(Account).GetProperty("AccountId")!.SetValue(account, accountId);
            return account;
        }

        [Fact]
        public void CreateDeposit_ShouldSetCorrectProperties()
        {
            var depositTx = CreateDepositTransaction(accountId: 1, amount: 500);

            depositTx.Type.Should().Be(TransactionType.Deposit);
            depositTx.Amount.Should().Be(500);
            depositTx.AccountId.Should().Be(1);
            depositTx.TargetAccountId.Should().BeNull();
            depositTx.Status.Should().Be(TransactionStatus.Pending);
            depositTx.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void CreateWithdrawal_ShouldSetCorrectProperties()
        {
            var withdrawalTx = CreateWithdrawalTransaction(accountId: 2, amount: 300);

            withdrawalTx.Type.Should().Be(TransactionType.Withdrawal);
            withdrawalTx.Amount.Should().Be(300);
            withdrawalTx.AccountId.Should().Be(2);
            withdrawalTx.TargetAccountId.Should().BeNull();
            withdrawalTx.Status.Should().Be(TransactionStatus.Pending);
            withdrawalTx.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void CreateTransfer_ShouldSetCorrectProperties()
        {
            var transferTx = CreateTransferTransaction(
                accountId: 3,
                amount: 1000,
                targetAccountId: 4
            );

            transferTx.Type.Should().Be(TransactionType.Transfer);
            transferTx.Amount.Should().Be(1000);
            transferTx.AccountId.Should().Be(3);
            transferTx.TargetAccountId.Should().Be(4);
            transferTx.Status.Should().Be(TransactionStatus.Pending);
            transferTx.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void CreateTransaction_ShouldThrow_WhenAmountIsInvalid()
        {
            Action createDepositWithZero = () => CreateDepositTransaction(amount: 0);
            Action createDepositWithNegative = () => CreateDepositTransaction(amount: -100);
            Action createWithdrawalWithZero = () => CreateWithdrawalTransaction(amount: 0);
            Action createWithdrawalWithNegative = () => CreateWithdrawalTransaction(amount: -100);
            Action createTransferWithZero = () => CreateTransferTransaction(amount: 0);
            Action createTransferWithNegative = () => CreateTransferTransaction(amount: -100);

            createDepositWithZero.Should().Throw<ArgumentException>();
            createDepositWithNegative.Should().Throw<ArgumentException>();
            createWithdrawalWithZero.Should().Throw<ArgumentException>();
            createWithdrawalWithNegative.Should().Throw<ArgumentException>();
            createTransferWithZero.Should().Throw<ArgumentException>();
            createTransferWithNegative.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ApplyDeposit_ShouldUpdateAccountBalance()
        {
            var account = CreateAccount(accountId: 1, balance: 5000);
            var depositTx = CreateDepositTransaction(accountId: 1, amount: 2000);

            var resultTx = account.Deposit(depositTx.Amount);

            account.Balance.Should().Be(7000);
            account.TotalLodgements.Should().Be(2000);
            resultTx.Type.Should().Be(TransactionType.Deposit);
            resultTx.Amount.Should().Be(2000);
            resultTx.AccountId.Should().Be(1);
            resultTx.Status.Should().Be(TransactionStatus.Pending);
        }

        [Fact]
        public void ApplyWithdrawal_ShouldUpdateAccountBalanceOrFail()
        {
            var accountWithFunds = CreateAccount(accountId: 2, balance: 5000);
            var accountWithoutFunds = CreateAccount(accountId: 3, balance: 2000);
            var withdrawalTx = CreateWithdrawalTransaction(accountId: 2, amount: 1000);
            var invalidWithdrawalTx = CreateWithdrawalTransaction(accountId: 3, amount: 3000);

            var resultTx = accountWithFunds.Withdraw(withdrawalTx.Amount);
            Action invalidWithdrawal = () =>
                accountWithoutFunds.Withdraw(invalidWithdrawalTx.Amount);

            accountWithFunds.Balance.Should().Be(4000);
            accountWithFunds.TotalWithdrawals.Should().Be(1000);
            resultTx.Type.Should().Be(TransactionType.Withdrawal);
            resultTx.Amount.Should().Be(1000);
            resultTx.AccountId.Should().Be(2);
            resultTx.Status.Should().Be(TransactionStatus.Pending);
            invalidWithdrawal
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Insuffiecient funds");
        }

        [Fact]
        public void ApplyTransfer_ShouldMoveFundsBetweenAccounts()
        {
            var fromAccount = CreateAccount(accountId: 4, balance: 5000);
            var toAccount = CreateAccount(accountId: 5, balance: 2000);
            var transferTx = CreateTransferTransaction(
                accountId: 4,
                amount: 1000,
                targetAccountId: 5
            );

            var resultTx = fromAccount.TransferTo(transferTx.Amount, toAccount);

            fromAccount.Balance.Should().Be(4000);
            fromAccount.TotalWithdrawals.Should().Be(1000);
            toAccount.Balance.Should().Be(3000);
            toAccount.TotalLodgements.Should().Be(2000);
            resultTx.Type.Should().Be(TransactionType.Transfer);
            resultTx.Amount.Should().Be(1000);
            resultTx.AccountId.Should().Be(4);
            resultTx.TargetAccountId.Should().Be(5);
            resultTx.Status.Should().Be(TransactionStatus.Pending);
        }
    }
}
