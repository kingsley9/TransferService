using System;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using TransferService.Domain.Entities;
using TransferService.Domain.Enums;
using Xunit;

namespace TransferService.Tests.Domain
{
    public class CustomerTests
    {
        private Customer CreateCustomer(
            string firstName = "John",
            string middleName = "M",
            string lastName = "Doe",
            string phoneNumber = "1234567890",
            string email = "test@example.com",
            string username = "johndoe",
            string passwordHash = "passwordHash"
        )
        {
            return new Customer(
                new CustomerName(firstName, middleName, lastName),
                DateTime.UtcNow.AddYears(-25),
                phoneNumber,
                email,
                new Address("Street", "City", "State", "12345", "Country"),
                username,
                passwordHash
            );
        }

        private Account CreateAccount(
            Guid customerId,
            string name = "Savings",
            decimal balance = 5000,
            string accountNumber = "1234567890",
            string pin = "1234"
        )
        {
            return new Account(
                name,
                "johndoe",
                AccountType.Savings,
                balance,
                AccountTier.TierThree,
                "NGN",
                customerId,
                accountNumber,
                pin
            );
        }

        [Fact]
        public void CreateCustomer_ShouldSetDefaultValuesAndValidateParameters()
        {
            var customer = CreateCustomer();

            Action createWithNullName = () => CreateCustomer(firstName: null);

            customer.Id.Should().NotBe(Guid.Empty);
            customer.IsActive.Should().BeTrue();
            customer.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            customer.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            createWithNullName
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("First name cannot be empty.*");
        }

        [Fact]
        public void AddAccount_ShouldManageAccountsCorrectly()
        {
            var customer = CreateCustomer();
            var account1 = CreateAccount(
                customer.Id,
                name: "Savings",
                accountNumber: "1234567890",
                pin: "1234"
            );
            var account2 = CreateAccount(
                customer.Id,
                name: "Checking",
                balance: 10000,
                accountNumber: "0987654321",
                pin: "4321"
            );
            var beforeUpdate = DateTime.UtcNow;

            customer.AddAccount(account1);
            customer.AddAccount(account2);
            Action addNullAccount = () => customer.AddAccount(null);

            customer.Accounts.Should().HaveCount(2);
            customer.Accounts.Should().Contain(account1);
            customer.Accounts.Should().Contain(account2);
            customer.Accounts.Should().BeAssignableTo<ReadOnlyCollection<Account>>();
            customer.UpdatedAt.Should().BeOnOrAfter(beforeUpdate);
            addNullAccount.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateCustomerDetails_ShouldChangeValuesAndUpdateTime()
        {
            var customer = CreateCustomer(email: "old@example.com", username: "johndoe");
            var newName = new CustomerName("Jane", "A", "Smith");
            var newAddress = new Address(
                "New Street",
                "New City",
                "New State",
                "54321",
                "New Country"
            );
            var newDob = DateTime.UtcNow.AddYears(-30);
            var beforeUpdate = DateTime.UtcNow;

            customer.UpdateEmail("new@example.com");
            customer.UpdateUsername("janedoe");
            customer.UpdateName(newName);
            customer.UpdateAddress(newAddress);
            customer.UpdatePhone("0987654321");
            customer.UpdateDateOfBirth(newDob);
            customer.UpdatePasswordHash("newHash");
            var originalUpdatedAt = customer.UpdatedAt;
            customer.UpdateUsername("janedoe");
            Action updateWithEmptyUsername = () => customer.UpdateUsername("");

            customer.Email.Should().Be("new@example.com");
            customer.Username.Should().Be("janedoe");
            customer.Name.Should().Be(newName);
            customer.Address.Should().Be(newAddress);
            customer.PhoneNumber.Should().Be("0987654321");
            customer.DateOfBirth.Should().Be(newDob);
            customer.PasswordHash.Should().Be("newHash");
            customer.UpdatedAt.Should().BeOnOrAfter(beforeUpdate);
            customer
                .UpdatedAt.Should()
                .Be(
                    originalUpdatedAt,
                    "because updating to the same username should not change UpdatedAt"
                );
            updateWithEmptyUsername.Should().Throw<ArgumentException>();
        }
    }
}
