using FluentAssertions;
using MediatR;
using NSubstitute;
using TransferService.Application.DTO;
using TransferService.Application.Features.Transactions.Commands.CreateDeposit;
using TransferService.Application.Features.Transactions.Commands.CreateTransfer;
using TransferService.Application.Features.Transactions.Commands.CreateWithdrawal;
using TransferService.Application.Features.Transactions.Queries.GetTransactionDetails;
using TransferService.Application.Features.Transactions.Queries.GetTransactionsForAccount;
using TransferService.Application.Services;
using TransferService.Domain.Enums;

namespace TransferService.Tests.Application.Transactions
{
    public class TransactionServiceTests
    {
        private readonly IMediator _mediator = Substitute.For<IMediator>();
        private readonly TransactionService _transactionService;

        public TransactionServiceTests()
        {
            _transactionService = new TransactionService(_mediator);
        }

        private TransactionRequest CreateTransactionRequest(
            int accountId = 1,
            int? targetAccountId = null,
            decimal amount = 1000,
            string pin = "1234"
        )
        {
            return new TransactionRequest
            {
                AccountId = accountId,
                TargetAccountId = targetAccountId,
                Amount = amount,
                Pin = pin,
            };
        }

        private TransactionResponse CreateTransactionResponse(
            int transactionId = 1,
            TransactionStatus status = TransactionStatus.Success,
            decimal balance = 4000,
            TransactionType type = TransactionType.Transfer,
            DateTime? timestamp = null
        )
        {
            return new TransactionResponse
            {
                TransactionId = transactionId,
                Status = status,
                Balance = balance,
                Type = type,
                Timestamp = timestamp ?? DateTime.UtcNow,
            };
        }

        [Fact]
        public async Task TransferAsync_ShouldTransferAndReturnResponse_WhenValidInput()
        {
            var request = CreateTransactionRequest(
                accountId: 1,
                targetAccountId: 2,
                amount: 1000,
                pin: "1234"
            );
            var response = CreateTransactionResponse(
                transactionId: 1,
                status: TransactionStatus.Success,
                balance: 4000,
                type: TransactionType.Transfer
            );
            _mediator
                .Send(
                    Arg.Is<CreateTransferCommand>(c => c.Request == request),
                    Arg.Any<CancellationToken>()
                )
                .Returns(response);

            var result = await _transactionService.TransferAsync(request);

            result.Should().NotBeNull();
            result.TransactionId.Should().Be(1);
            result.Status.Should().Be(TransactionStatus.Success);
            result.Type.Should().Be(TransactionType.Transfer);
            result.Balance.Should().Be(4000);
            result.Timestamp.Should().NotBe(default);
            await _mediator
                .Received(1)
                .Send(Arg.Any<CreateTransferCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task TransferAsync_ShouldThrow_WhenFromAccountNotFound()
        {
            var request = CreateTransactionRequest(accountId: 1, targetAccountId: 2);
            _mediator
                .Send(Arg.Any<CreateTransferCommand>(), Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromException<TransactionResponse>(
                        new ArgumentException("One or both accounts not found")
                    )
                );

            Func<Task> action = async () => await _transactionService.TransferAsync(request);

            await action
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("One or both accounts not found");
            await _mediator
                .Received(1)
                .Send(Arg.Any<CreateTransferCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task TransferAsync_ShouldThrow_WhenToAccountNotFound()
        {
            var request = CreateTransactionRequest(accountId: 1, targetAccountId: 2);
            _mediator
                .Send(Arg.Any<CreateTransferCommand>(), Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromException<TransactionResponse>(
                        new ArgumentException("One or both accounts not found")
                    )
                );

            Func<Task> action = async () => await _transactionService.TransferAsync(request);

            await action
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("One or both accounts not found");
            await _mediator
                .Received(1)
                .Send(Arg.Any<CreateTransferCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task TransferAsync_ShouldThrow_WhenSameAccount()
        {
            var request = CreateTransactionRequest(accountId: 1, targetAccountId: 1);
            _mediator
                .Send(Arg.Any<CreateTransferCommand>(), Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromException<TransactionResponse>(
                        new InvalidOperationException("Cannot transfer to the same account")
                    )
                );

            Func<Task> action = async () => await _transactionService.TransferAsync(request);

            await action
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Cannot transfer to the same account");
            await _mediator
                .Received(1)
                .Send(Arg.Any<CreateTransferCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task TransferAsync_ShouldThrow_WhenPinIsInvalid()
        {
            var request = CreateTransactionRequest(accountId: 1, targetAccountId: 2, pin: "9999");
            _mediator
                .Send(Arg.Any<CreateTransferCommand>(), Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromException<TransactionResponse>(
                        new UnauthorizedAccessException("Invalid PIN")
                    )
                );

            Func<Task> action = async () => await _transactionService.TransferAsync(request);

            await action
                .Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid PIN");
            await _mediator
                .Received(1)
                .Send(Arg.Any<CreateTransferCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task TransferAsync_ShouldThrowAndLogFailedTransaction_WhenValidationFails()
        {
            var request = CreateTransactionRequest(accountId: 1, targetAccountId: 2, amount: 1000);
            _mediator
                .Send(Arg.Any<CreateTransferCommand>(), Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromException<TransactionResponse>(
                        new Exception("Transfer Failed: Insufficient balance")
                    )
                );

            Func<Task> action = async () => await _transactionService.TransferAsync(request);

            await action
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage("Transfer Failed: Insufficient balance");
            await _mediator
                .Received(1)
                .Send(Arg.Any<CreateTransferCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Deposit_ShouldDepositAndReturnResponse_WhenValidInput()
        {
            var request = CreateTransactionRequest(
                accountId: 1,
                targetAccountId: null,
                amount: 1000,
                pin: "1234"
            );
            var response = CreateTransactionResponse(
                transactionId: 1,
                status: TransactionStatus.Success,
                balance: 6000,
                type: TransactionType.Deposit
            );
            _mediator
                .Send(
                    Arg.Is<CreateDepositCommand>(c => c.Request == request),
                    Arg.Any<CancellationToken>()
                )
                .Returns(response);

            var result = await _transactionService.Deposit(request);

            result.Should().NotBeNull();
            result.TransactionId.Should().Be(1);
            result.Status.Should().Be(TransactionStatus.Success);
            result.Type.Should().Be(TransactionType.Deposit);
            result.Balance.Should().Be(6000);
            result.Timestamp.Should().NotBe(default);
            await _mediator
                .Received(1)
                .Send(Arg.Any<CreateDepositCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Deposit_ShouldThrow_WhenAccountNotFound()
        {
            var request = CreateTransactionRequest(accountId: 1, targetAccountId: null);
            _mediator
                .Send(Arg.Any<CreateDepositCommand>(), Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromException<TransactionResponse>(
                        new Exception("Account 1 does not exist")
                    )
                );

            Func<Task> action = async () => await _transactionService.Deposit(request);

            await action.Should().ThrowAsync<Exception>().WithMessage("Account 1 does not exist");
            await _mediator
                .Received(1)
                .Send(Arg.Any<CreateDepositCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Deposit_ShouldThrow_WhenPinIsInvalid()
        {
            var request = CreateTransactionRequest(
                accountId: 1,
                targetAccountId: null,
                pin: "9999"
            );
            _mediator
                .Send(Arg.Any<CreateDepositCommand>(), Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromException<TransactionResponse>(
                        new UnauthorizedAccessException("Invalid PIN")
                    )
                );

            Func<Task> action = async () => await _transactionService.Deposit(request);

            await action
                .Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid PIN");
            await _mediator
                .Received(1)
                .Send(Arg.Any<CreateDepositCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Deposit_ShouldThrow_WhenValidationFails()
        {
            var request = CreateTransactionRequest(accountId: 1, targetAccountId: null);
            _mediator
                .Send(Arg.Any<CreateDepositCommand>(), Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromException<TransactionResponse>(
                        new Exception("Deposit Failed: Invalid deposit amount")
                    )
                );

            Func<Task> action = async () => await _transactionService.Deposit(request);

            await action
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage("Deposit Failed: Invalid deposit amount");
            await _mediator
                .Received(1)
                .Send(Arg.Any<CreateDepositCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Withdraw_ShouldWithdrawAndReturnResponse_WhenValidInput()
        {
            var request = CreateTransactionRequest(
                accountId: 1,
                targetAccountId: null,
                amount: 1000,
                pin: "1234"
            );
            var response = CreateTransactionResponse(
                transactionId: 1,
                status: TransactionStatus.Success,
                balance: 4000,
                type: TransactionType.Withdrawal
            );
            _mediator
                .Send(
                    Arg.Is<CreateWithdrawalCommand>(c => c.Request == request),
                    Arg.Any<CancellationToken>()
                )
                .Returns(response);

            var result = await _transactionService.Withdraw(request);

            result.Should().NotBeNull();
            result.TransactionId.Should().Be(1);
            result.Status.Should().Be(TransactionStatus.Success);
            result.Type.Should().Be(TransactionType.Withdrawal);
            result.Balance.Should().Be(4000);
            result.Timestamp.Should().NotBe(default);
            await _mediator
                .Received(1)
                .Send(Arg.Any<CreateWithdrawalCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Withdraw_ShouldThrow_WhenAccountNotFound()
        {
            var request = CreateTransactionRequest(accountId: 1, targetAccountId: null);
            _mediator
                .Send(Arg.Any<CreateWithdrawalCommand>(), Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromException<TransactionResponse>(
                        new Exception("Account 1 does not exist")
                    )
                );

            Func<Task> action = async () => await _transactionService.Withdraw(request);

            await action.Should().ThrowAsync<Exception>().WithMessage("Account 1 does not exist");
            await _mediator
                .Received(1)
                .Send(Arg.Any<CreateWithdrawalCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Withdraw_ShouldThrow_WhenPinIsInvalid()
        {
            var request = CreateTransactionRequest(
                accountId: 1,
                targetAccountId: null,
                pin: "9999"
            );
            _mediator
                .Send(Arg.Any<CreateWithdrawalCommand>(), Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromException<TransactionResponse>(
                        new UnauthorizedAccessException("Invalid PIN")
                    )
                );

            Func<Task> action = async () => await _transactionService.Withdraw(request);

            await action
                .Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid PIN");
            await _mediator
                .Received(1)
                .Send(Arg.Any<CreateWithdrawalCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Withdraw_ShouldThrow_WhenValidationFails()
        {
            var request = CreateTransactionRequest(accountId: 1, targetAccountId: null);
            _mediator
                .Send(Arg.Any<CreateWithdrawalCommand>(), Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromException<TransactionResponse>(
                        new Exception("Withdrawal failed: Insufficient balance")
                    )
                );

            Func<Task> action = async () => await _transactionService.Withdraw(request);

            await action
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage("Withdrawal failed: Insufficient balance");
            await _mediator
                .Received(1)
                .Send(Arg.Any<CreateWithdrawalCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetTransactionDetailsAsync_ReturnsTransactionResponse_WhenTransactionExists()
        {
            var accountId = 1;
            var transactionId = 1;
            var response = CreateTransactionResponse(
                transactionId: 1,
                status: TransactionStatus.Success,
                balance: 5000,
                type: TransactionType.Deposit
            );
            _mediator
                .Send(
                    Arg.Is<GetTransactionDetailsQuery>(q =>
                        q.AccountId == accountId && q.TransactionId == transactionId
                    ),
                    Arg.Any<CancellationToken>()
                )
                .Returns(response);

            var result = await _transactionService.GetTransactionDetailsAsync(
                accountId,
                transactionId
            );

            result.Should().NotBeNull();
            result.TransactionId.Should().Be(1);
            result.Status.Should().Be(TransactionStatus.Success);
            result.Type.Should().Be(TransactionType.Deposit);
            result.Balance.Should().Be(5000);
            result.Timestamp.Should().NotBe(default);
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetTransactionDetailsQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetTransactionDetailsAsync_ReturnsNull_WhenTransactionDoesNotExist()
        {
            var accountId = 1;
            var transactionId = 1;
            _mediator
                .Send(
                    Arg.Is<GetTransactionDetailsQuery>(q =>
                        q.AccountId == accountId && q.TransactionId == transactionId
                    ),
                    Arg.Any<CancellationToken>()
                )
                .Returns((TransactionResponse?)null);

            var result = await _transactionService.GetTransactionDetailsAsync(
                accountId,
                transactionId
            );

            result.Should().BeNull();
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetTransactionDetailsQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetTransactionsForAccountAsync_ReturnsTransactions_WhenTransactionsExist()
        {
            var accountId = 1;
            var responses = new List<TransactionResponse>
            {
                CreateTransactionResponse(
                    transactionId: 1,
                    status: TransactionStatus.Success,
                    balance: 5000,
                    type: TransactionType.Deposit
                ),
                CreateTransactionResponse(
                    transactionId: 2,
                    status: TransactionStatus.Success,
                    balance: 4500,
                    type: TransactionType.Withdrawal
                ),
            };
            _mediator
                .Send(
                    Arg.Is<GetTransactionsForAccountQuery>(q => q.AccountId == accountId),
                    Arg.Any<CancellationToken>()
                )
                .Returns(responses);

            var result = await _transactionService.GetTransactionsForAccountAsync(accountId);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(t => t.Type == TransactionType.Deposit && t.Balance == 5000);
            result.Should().Contain(t => t.Type == TransactionType.Withdrawal && t.Balance == 4500);
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetTransactionsForAccountQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetTransactionsForAccountAsync_ReturnsEmptyList_WhenNoTransactionsExist()
        {
            var accountId = 1;
            _mediator
                .Send(
                    Arg.Is<GetTransactionsForAccountQuery>(q => q.AccountId == accountId),
                    Arg.Any<CancellationToken>()
                )
                .Returns(new List<TransactionResponse>());

            var result = await _transactionService.GetTransactionsForAccountAsync(accountId);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetTransactionsForAccountQuery>(), Arg.Any<CancellationToken>());
        }
    }
}
