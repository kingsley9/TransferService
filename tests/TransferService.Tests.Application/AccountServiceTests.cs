using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using NSubstitute;
using TransferService.Application.DTO;
using TransferService.Application.Features.Accounts.Commands.ChangeAccountPin;
using TransferService.Application.Features.Accounts.Commands.CreateAccount;
using TransferService.Application.Features.Accounts.Commands.DeleteAccount;
using TransferService.Application.Features.Accounts.Commands.UpdateAccount;
using TransferService.Application.Features.Accounts.Queries.GetAccountBalance;
using TransferService.Application.Features.Accounts.Queries.GetAccountById;
using TransferService.Application.Features.Accounts.Queries.GetAllAccounts;
using TransferService.Application.Features.Accounts.Queries.GetOwnedAccount;
using TransferService.Application.Services;
using TransferService.Domain.Entities;
using TransferService.Domain.Enums;
using TransferService.Domain.Exceptions;
using Xunit;

namespace TransferService.Tests.Application.Accounts
{
    public class AccountServiceTests
    {
        private readonly IMediator _mediator = Substitute.For<IMediator>();
        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            _accountService = new AccountService(_mediator);
        }

        private AccountRequest CreateAccountRequest(
            decimal balance = 5000,
            string pin = "1234",
            AccountType type = AccountType.Savings,
            AccountTier tier = AccountTier.TierThree,
            string currency = "NGN",
            string bankCode = "001",
            string schemeCode = "SAV"
        )
        {
            return new AccountRequest
            {
                Balance = balance,
                Pin = pin,
                Type = type,
                Tier = tier,
                Currency = currency,
                BankCode = bankCode,
                SchemeCode = schemeCode,
            };
        }

        private AccountDetailsResponse CreateAccountDetailsResponse(
            int accountId = 1,
            string accountName = "TestAccount",
            string username = "user1",
            Guid customerId = default,
            decimal balance = 5000,
            string accountNumber = "1234567890",
            AccountType type = AccountType.Savings,
            AccountTier tier = AccountTier.TierThree,
            string currency = "NGN"
        )
        {
            return new AccountDetailsResponse
            {
                AccountId = accountId,
                AccountName = accountName,
                Username = username,
                CustomerId = customerId == default ? Guid.NewGuid() : customerId,
                Balance = balance,
                AccountNumber = accountNumber,
                Type = type,
                Tier = tier,
                Currency = currency,
            };
        }

        private Account CreateAccount(
            int accountId = 1,
            string accountName = "TestAccount",
            string username = "user1",
            Guid customerId = default,
            decimal balance = 5000,
            string accountNumber = "1234567890",
            string pinHash = "1234",
            AccountType type = AccountType.Savings,
            AccountTier tier = AccountTier.TierThree,
            string currency = "NGN"
        )
        {
            var account = new Account(
                accountName,
                username,
                type,
                balance,
                tier,
                currency,
                customerId == default ? Guid.NewGuid() : customerId,
                accountNumber,
                pinHash
            )
            {
                AccountId = accountId,
            };
            return account;
        }

        [Fact]
        public async Task GetAccountByIdAsync_ReturnsAccount_WhenAccountExists()
        {
            var accountId = 1;
            var response = CreateAccountDetailsResponse(accountId);
            _mediator
                .Send(
                    Arg.Is<GetAccountByIdQuery>(q => q.AccountId == accountId),
                    Arg.Any<CancellationToken>()
                )
                .Returns(response);

            var result = await _accountService.GetAccountByIdAsync(accountId);

            result.Should().NotBeNull();
            result.AccountId.Should().Be(response.AccountId);
            result.AccountName.Should().Be(response.AccountName);
            result.Username.Should().Be(response.Username);
            result.Balance.Should().Be(response.Balance);
            result.Type.Should().Be(response.Type);
            result.Tier.Should().Be(response.Tier);
            result.Currency.Should().Be(response.Currency);
            result.CustomerId.Should().Be(response.CustomerId);
            result.AccountNumber.Should().Be(response.AccountNumber);
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetAccountByIdQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAccountByIdAsync_ReturnsNull_WhenAccountDoesNotExist()
        {
            var accountId = 1;
            _mediator
                .Send(
                    Arg.Is<GetAccountByIdQuery>(q => q.AccountId == accountId),
                    Arg.Any<CancellationToken>()
                )
                .Returns((AccountDetailsResponse?)null);

            var result = await _accountService.GetAccountByIdAsync(accountId);

            result.Should().BeNull();
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetAccountByIdQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAllAccountsAsync_ReturnsAllAccounts_WhenAccountsExist()
        {
            var responses = new List<AccountDetailsResponse>
            {
                CreateAccountDetailsResponse(1),
                CreateAccountDetailsResponse(2, accountName: "Account2"),
            };
            _mediator
                .Send(Arg.Any<GetAllAccountsQuery>(), Arg.Any<CancellationToken>())
                .Returns(responses);

            var result = await _accountService.GetAllAccountsAsync();

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(a => a.AccountId == 1);
            result.Should().Contain(a => a.AccountId == 2);
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetAllAccountsQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CreateAccountAsync_CreatesAccount_WhenValidRequest()
        {
            var request = CreateAccountRequest();
            var ownerName = "TestAccount";
            var username = "user1";
            var customerId = Guid.NewGuid();
            var response = CreateAccountDetailsResponse(
                accountName: ownerName,
                username: username,
                customerId: customerId,
                accountNumber: "1234567890",
                type: request.Type,
                tier: request.Tier,
                currency: request.Currency
            );
            _mediator
                .Send(
                    Arg.Is<CreateAccountCommand>(c =>
                        c.Account == request
                        && c.OwnerName == ownerName
                        && c.Username == username
                        && c.CustomerId == customerId
                    ),
                    Arg.Any<CancellationToken>()
                )
                .Returns(response);

            var result = await _accountService.CreateAccountAsync(
                request,
                ownerName,
                username,
                customerId
            );

            result.Should().NotBeNull();
            result.AccountName.Should().Be(ownerName);
            result.Username.Should().Be(username);
            result.CustomerId.Should().Be(customerId);
            result.AccountNumber.Should().Be(response.AccountNumber);
            result.Balance.Should().Be(request.Balance);
            result.Type.Should().Be(request.Type);
            result.Tier.Should().Be(request.Tier);
            result.Currency.Should().Be(request.Currency);
            await _mediator
                .Received(1)
                .Send(Arg.Any<CreateAccountCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CreateAccountAsync_ThrowsArgumentNullException_WhenRequestIsNull()
        {
            Func<Task> action = async () =>
                await _accountService.CreateAccountAsync(
                    null,
                    "TestAccount",
                    "user1",
                    Guid.NewGuid()
                );

            await action.Should().ThrowAsync<ArgumentNullException>().WithParameterName("account");
            await _mediator
                .Received(0)
                .Send(Arg.Any<CreateAccountCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CreateAccountAsync_ThrowsException_WhenAccountNumberGenerationFails()
        {
            var request = CreateAccountRequest();
            _mediator
                .Send(Arg.Any<CreateAccountCommand>(), Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromException<AccountDetailsResponse>(new Exception("Generation failed"))
                );

            Func<Task> action = async () =>
                await _accountService.CreateAccountAsync(
                    request,
                    "TestAccount",
                    "user1",
                    Guid.NewGuid()
                );

            await action.Should().ThrowAsync<Exception>().WithMessage("Generation failed");
            await _mediator
                .Received(1)
                .Send(Arg.Any<CreateAccountCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UpdateAccountAsync_UpdatesAccountName_WhenAccountExists()
        {
            var existingAccount = CreateAccount(accountName: "OldName");
            var updatedAccount = CreateAccount(accountName: "NewName");
            var response = CreateAccountDetailsResponse(accountName: "NewName");
            _mediator
                .Send(
                    Arg.Is<UpdateAccountCommand>(c =>
                        c.AccountId == 1
                        && c.AccountName == "NewName"
                        && c.Type == AccountType.Savings
                        && c.Tier == AccountTier.TierThree
                    ),
                    Arg.Any<CancellationToken>()
                )
                .Returns(response);

            var result = await _accountService.UpdateAccountAsync(updatedAccount);

            result.Should().BeTrue();
            await _mediator
                .Received(1)
                .Send(Arg.Any<UpdateAccountCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UpdateAccountAsync_UpdatesAllFields_WhenAllFieldsProvided()
        {
            var existingAccount = CreateAccount(
                accountName: "OldName",
                type: AccountType.Savings,
                tier: AccountTier.TierThree
            );
            var updatedAccount = CreateAccount(
                accountName: "NewName",
                type: AccountType.Current,
                tier: AccountTier.TierTwo
            );
            var response = CreateAccountDetailsResponse(
                accountName: "NewName",
                type: AccountType.Current,
                tier: AccountTier.TierTwo
            );
            _mediator
                .Send(
                    Arg.Is<UpdateAccountCommand>(c =>
                        c.AccountId == 1
                        && c.AccountName == "NewName"
                        && c.Type == AccountType.Current
                        && c.Tier == AccountTier.TierTwo
                    ),
                    Arg.Any<CancellationToken>()
                )
                .Returns(response);

            var result = await _accountService.UpdateAccountAsync(updatedAccount);

            result.Should().BeTrue();
            await _mediator
                .Received(1)
                .Send(Arg.Any<UpdateAccountCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UpdateAccountAsync_ReturnsFalse_WhenAccountDoesNotExist()
        {
            var updatedAccount = CreateAccount();
            _mediator
                .Send(Arg.Any<UpdateAccountCommand>(), Arg.Any<CancellationToken>())
                .Returns((AccountDetailsResponse?)null);

            var result = await _accountService.UpdateAccountAsync(updatedAccount);

            result.Should().BeFalse();
            await _mediator
                .Received(1)
                .Send(Arg.Any<UpdateAccountCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task DeleteAccountAsync_ReturnsTrue_WhenAccountExists()
        {
            var accountId = 1;
            _mediator
                .Send(
                    Arg.Is<DeleteAccountCommand>(c => c.AccountId == accountId),
                    Arg.Any<CancellationToken>()
                )
                .Returns(true);

            var result = await _accountService.DeleteAccountAsync(accountId);

            result.Should().BeTrue();
            await _mediator
                .Received(1)
                .Send(Arg.Any<DeleteAccountCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task DeleteAccountAsync_ReturnsFalse_WhenAccountNotExists()
        {
            var accountId = 1;
            _mediator
                .Send(
                    Arg.Is<DeleteAccountCommand>(c => c.AccountId == accountId),
                    Arg.Any<CancellationToken>()
                )
                .Returns(false);

            var result = await _accountService.DeleteAccountAsync(accountId);

            result.Should().BeFalse();
            await _mediator
                .Received(1)
                .Send(Arg.Any<DeleteAccountCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAccountBalanceAsync_ReturnsBalance_WhenAccountExists()
        {
            var accountId = 1;
            var balance = 5000m;
            _mediator
                .Send(
                    Arg.Is<GetAccountBalanceQuery>(q => q.AccountId == accountId),
                    Arg.Any<CancellationToken>()
                )
                .Returns(balance);

            var result = await _accountService.GetAccountBalanceAsync(accountId);

            result.Should().Be(balance);
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetAccountBalanceQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAccountBalanceAsync_ReturnsNull_WhenAccountDoesNotExist()
        {
            var accountId = 1;
            _mediator
                .Send(
                    Arg.Is<GetAccountBalanceQuery>(q => q.AccountId == accountId),
                    Arg.Any<CancellationToken>()
                )
                .Returns((decimal?)null);

            var result = await _accountService.GetAccountBalanceAsync(accountId);

            result.Should().BeNull();
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetAccountBalanceQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetOwnedAccountAsync_ReturnsAccount_WhenUserOwnsAccount()
        {
            var accountId = 1;
            var username = "user1";
            var response = CreateAccountDetailsResponse(accountId, username: username);
            _mediator
                .Send(
                    Arg.Is<GetOwnedAccountQuery>(q =>
                        q.AccountId == accountId && q.Username == username
                    ),
                    Arg.Any<CancellationToken>()
                )
                .Returns(response);

            var result = await _accountService.GetOwnedAccountAsync(accountId, username);

            result.Should().NotBeNull();
            result.AccountId.Should().Be(response.AccountId);
            result.Username.Should().Be(response.Username);
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetOwnedAccountQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetOwnedAccountAsync_ThrowsNotFoundException_WhenAccountNotFound()
        {
            var accountId = 1;
            var username = "user1";
            _mediator
                .Send(
                    Arg.Is<GetOwnedAccountQuery>(q =>
                        q.AccountId == accountId && q.Username == username
                    ),
                    Arg.Any<CancellationToken>()
                )
                .Returns(
                    Task.FromException<AccountDetailsResponse>(
                        new NotFoundException("Account not found")
                    )
                );

            Func<Task> action = async () =>
                await _accountService.GetOwnedAccountAsync(accountId, username);

            await action.Should().ThrowAsync<NotFoundException>().WithMessage("Account not found");
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetOwnedAccountQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetOwnedAccountAsync_ThrowsForbiddenException_WhenAccountNotOwned()
        {
            var accountId = 1;
            var username = "user2";
            _mediator
                .Send(
                    Arg.Is<GetOwnedAccountQuery>(q =>
                        q.AccountId == accountId && q.Username == username
                    ),
                    Arg.Any<CancellationToken>()
                )
                .Returns(
                    Task.FromException<AccountDetailsResponse>(
                        new ForbiddenException("User does not own account")
                    )
                );

            Func<Task> action = async () =>
                await _accountService.GetOwnedAccountAsync(accountId, username);

            await action
                .Should()
                .ThrowAsync<ForbiddenException>()
                .WithMessage("User does not own account");
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetOwnedAccountQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ChangeAccountPinAsync_ChangesPin_WhenValidRequest()
        {
            var account = CreateAccount();
            var currentPin = "1234";
            var newPin = "5678";
            _mediator
                .Send(
                    Arg.Is<ChangeAccountPinCommand>(c =>
                        c.AccountId == account.AccountId
                        && c.CurrentPin == currentPin
                        && c.NewPin == newPin
                    ),
                    Arg.Any<CancellationToken>()
                )
                .Returns(Task.CompletedTask);

            await _accountService.ChangeAccountPinAsync(account, currentPin, newPin);

            await _mediator
                .Received(1)
                .Send(Arg.Any<ChangeAccountPinCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ChangeAccountPinAsync_ThrowsArgumentException_WhenPinChangeFails()
        {
            var account = CreateAccount();
            var currentPin = "1234";
            var newPin = "5678";
            _mediator
                .Send(Arg.Any<ChangeAccountPinCommand>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromException(new ArgumentException("Current PIN is incorrect")));

            Func<Task> action = async () =>
                await _accountService.ChangeAccountPinAsync(account, currentPin, newPin);

            await action
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Current PIN is incorrect");
            await _mediator
                .Received(1)
                .Send(Arg.Any<ChangeAccountPinCommand>(), Arg.Any<CancellationToken>());
        }
    }
}
