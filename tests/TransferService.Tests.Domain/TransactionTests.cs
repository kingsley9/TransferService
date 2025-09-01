using TransferService.Domain.Entities;
using TransferService.Domain.Enums;
using Xunit;

namespace TransferService.Tests.Domain
{
    public class TransactionTests
    {
        [Fact]
        public void CreateDeposit_ShouldReturnDepositTransaction()
        {
            var tx = Transaction.CreateDeposit(1, 500);

            Assert.Equal(TransactionType.Deposit, tx.Type);
            Assert.Equal(500, tx.Amount);
        }

        [Fact]
        public void CreateWithdrawal_ShouldReturnWithdrawalTransaction()
        {
            var tx = Transaction.CreateWithdrawal(1, 300);

            Assert.Equal(TransactionType.Withdrawal, tx.Type);
            Assert.Equal(300, tx.Amount);
        }

        [Fact]
        public void CreateTransfer_ShouldReturnTransferTransaction()
        {
            var tx = Transaction.CreateTransfer(1, 1000, 2);

            Assert.Equal(TransactionType.Transfer, tx.Type);
            Assert.Equal(1000, tx.Amount);
            Assert.Equal(2, tx.TargetAccountId);
        }

        [Fact]
        public void Transaction_ShouldThrow_WhenAmountIsZero()
        {
            Assert.Throws<ArgumentException>(() => Transaction.CreateDeposit(1, 0));
        }
    }
}
