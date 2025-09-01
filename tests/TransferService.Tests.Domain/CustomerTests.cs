using TransferService.Domain.Entities;
using TransferService.Domain.Enums;
using Xunit;

namespace TransferService.Tests.Domain
{
    public class CustomerTests
    {
        [Fact]
        public void AddAccount_ShouldAddAccountToCustomer()
        {
            var customer = new Customer(
                new CustomerName("John", "M", "Doe"),
                DateTime.UtcNow.AddYears(-25),
                "1234567890",
                "test@example.com",
                new Address("Street", "City", "State", "12345", "Country"),
                "johndoe",
                "hash"
            );

            var account = new Account(
                "Savings",
                "johndoe",
                AccountType.Savings,
                5000,
                AccountTier.TierThree,
                "NGN",
                customer.Id,
                "1234567890",
                "pin"
            );

            customer.AddAccount(account);

            Assert.Single(customer.Accounts);
            Assert.Equal(account, customer.Accounts.First());
        }

        [Fact]
        public void UpdateEmail_ShouldChangeEmail()
        {
            var customer = new Customer(
                new CustomerName("John", "M", "Doe"),
                DateTime.UtcNow.AddYears(-25),
                "1234567890",
                "old@example.com",
                new Address("Street", "City", "State", "12345", "Country"),
                "johndoe",
                "hash"
            );

            customer.UpdateEmail("new@example.com");

            Assert.Equal("new@example.com", customer.Email);
        }

        [Fact]
        public void UpdateUsername_ShouldThrow_WhenEmpty()
        {
            var customer = new Customer(
                new CustomerName("John", "M", "Doe"),
                DateTime.UtcNow.AddYears(-25),
                "1234567890",
                "test@example.com",
                new Address("Street", "City", "State", "12345", "Country"),
                "johndoe",
                "hash"
            );

            Assert.Throws<ArgumentException>(() => customer.UpdateUsername(""));
        }
    }
}
