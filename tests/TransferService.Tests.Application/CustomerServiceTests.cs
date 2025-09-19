using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using NSubstitute;
using TransferService.Application.DTO;
using TransferService.Application.Features.Customers.Commands.CreateCustomer;
using TransferService.Application.Features.Customers.Commands.DeleteCustomer;
using TransferService.Application.Features.Customers.Commands.UpdateCustomer;
using TransferService.Application.Features.Customers.Queries.GetAllCustomers;
using TransferService.Application.Features.Customers.Queries.GetCustomerByCredentials;
using TransferService.Application.Features.Customers.Queries.GetCustomerById;
using TransferService.Application.Features.Customers.Queries.GetCustomerByUsername;
using TransferService.Application.Services;
using TransferService.Domain.Entities;
using TransferService.Domain.Enums;
using Xunit;

namespace TransferService.Tests.Application.Customers
{
    public class CustomerServiceTests
    {
        private readonly IMediator _mediator = Substitute.For<IMediator>();
        private readonly CustomerService _customerService;

        public CustomerServiceTests()
        {
            _customerService = new CustomerService(_mediator);
        }

        private CreateCustomerRequest CreateCustomerRequest(
            string firstName = "John",
            string middleName = "A",
            string lastName = "Doe",
            string email = "john.doe@example.com",
            string username = "johndoe",
            string password = "Password123",
            string phoneNumber = "1234567890",
            DateTime? dateOfBirth = null,
            string street = "123 Main St",
            string city = "Lagos",
            string state = "Lagos",
            string postalCode = "100001",
            string country = "Nigeria"
        )
        {
            return new CreateCustomerRequest
            {
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                Email = email,
                Username = username,
                Password = password,
                PhoneNumber = phoneNumber,
                DateOfBirth = dateOfBirth ?? new DateTime(1990, 1, 1),
                Street = street,
                City = city,
                State = state,
                PostalCode = postalCode,
                Country = country,
            };
        }

        private UpdateCustomerRequest CreateUpdateCustomerRequest(
            string firstName = null,
            string middleName = null,
            string lastName = null,
            string username = null,
            string password = null,
            string street = null,
            string city = null,
            string state = null,
            string postalCode = null,
            string country = null
        )
        {
            return new UpdateCustomerRequest
            {
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                Username = username,
                Password = password,
                Street = street,
                City = city,
                State = state,
                PostalCode = postalCode,
                Country = country,
            };
        }

        private CustomerDto CreateCustomerDto(
            Guid id,
            string username = "johndoe",
            string fullName = "John A Doe",
            string email = "john.doe@example.com"
        )
        {
            return new CustomerDto
            {
                Id = id,
                Username = username,
                FullName = fullName,
                Email = email,
            };
        }

        private Customer CreateCustomer(
            Guid id = default,
            string firstName = "John",
            string middleName = "A",
            string lastName = "Doe",
            string email = "john.doe@example.com",
            string username = "johndoe",
            string passwordHash = "hashedPassword",
            string phoneNumber = "1234567890",
            DateTime? dateOfBirth = null,
            string street = "123 Main St",
            string city = "Lagos",
            string state = "Lagos",
            string postalCode = "100001",
            string country = "Nigeria",
            IReadOnlyCollection<Account> accounts = null
        )
        {
            var customer = new Customer(
                new CustomerName(firstName, middleName, lastName),
                dateOfBirth ?? new DateTime(1990, 1, 1),
                phoneNumber,
                email,
                new Address(street, city, state, postalCode, country),
                username,
                passwordHash
            );
            if (id != default)
            {
                customer.Id = id;
            }
            if (accounts != null)
            {
                foreach (Account account in accounts)
                {
                    customer.AddAccount(account);
                }
            }
            return customer;
        }

        [Fact]
        public async Task RegisterCustomerAsync_CreatesCustomer_WhenEmailIsUnique()
        {
            var request = CreateCustomerRequest();
            var response = CreateCustomer(
                id: Guid.NewGuid(),
                username: request.Username,
                email: request.Email
            );
            _mediator
                .Send(
                    Arg.Is<CreateCustomerCommand>(c => c.Request == request),
                    Arg.Any<CancellationToken>()
                )
                .Returns(response);

            var result = await _customerService.RegisterCustomerAsync(request);

            result.Should().NotBeNull();
            result.Username.Should().Be(request.Username);
            result.Email.Should().Be(request.Email);
            result.Name.FirstName.Should().Be(request.FirstName);
            result.Name.MiddleName.Should().Be(request.MiddleName);
            result.Name.LastName.Should().Be(request.LastName);
            result.PhoneNumber.Should().Be(request.PhoneNumber);
            result.DateOfBirth.Should().Be(request.DateOfBirth);
            result.Address.Street.Should().Be(request.Street);
            result.Address.City.Should().Be(request.City);
            result.Address.State.Should().Be(request.State);
            result.Address.PostalCode.Should().Be(request.PostalCode);
            result.Address.Country.Should().Be(request.Country);
            await _mediator
                .Received(1)
                .Send(Arg.Any<CreateCustomerCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task RegisterCustomerAsync_ThrowsInvalidOperationException_WhenEmailExists()
        {
            var request = CreateCustomerRequest();
            _mediator
                .Send(Arg.Any<CreateCustomerCommand>(), Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromException<Customer>(
                        new InvalidOperationException("A customer with this email already exists")
                    )
                );

            Func<Task> action = async () => await _customerService.RegisterCustomerAsync(request);

            await action
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("A customer with this email already exists");
            await _mediator
                .Received(1)
                .Send(Arg.Any<CreateCustomerCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ReturnsCustomerDto_WhenCustomerExists()
        {
            var customerId = Guid.NewGuid();
            var response = CreateCustomerDto(customerId);
            _mediator
                .Send(
                    Arg.Is<GetCustomerByIdQuery>(q => q.Id == customerId),
                    Arg.Any<CancellationToken>()
                )
                .Returns(response);

            var result = await _customerService.GetCustomerByIdAsync(customerId);

            result.Should().NotBeNull();
            result.Id.Should().Be(customerId);
            result.Username.Should().Be(response.Username);
            result.FullName.Should().Be(response.FullName);
            result.Email.Should().Be(response.Email);
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetCustomerByIdQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ReturnsNull_WhenCustomerDoesNotExist()
        {
            var customerId = Guid.NewGuid();
            _mediator
                .Send(
                    Arg.Is<GetCustomerByIdQuery>(q => q.Id == customerId),
                    Arg.Any<CancellationToken>()
                )
                .Returns((CustomerDto?)null);

            var result = await _customerService.GetCustomerByIdAsync(customerId);

            result.Should().BeNull();
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetCustomerByIdQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAllCustomersAsync_ReturnsAllCustomerDtos_WhenCustomersExist()
        {
            var responses = new List<CustomerDto>
            {
                CreateCustomerDto(Guid.NewGuid(), username: "user1", email: "user1@example.com"),
                CreateCustomerDto(
                    Guid.NewGuid(),
                    username: "user2",
                    email: "user2@example.com",
                    fullName: "Jane Smith"
                ),
            };
            _mediator
                .Send(Arg.Any<GetAllCustomersQuery>(), Arg.Any<CancellationToken>())
                .Returns(responses);

            var result = await _customerService.GetAllCustomersAsync();

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(c => c.Username == "user1" && c.Email == "user1@example.com");
            result
                .Should()
                .Contain(c =>
                    c.Username == "user2"
                    && c.Email == "user2@example.com"
                    && c.FullName == "Jane Smith"
                );
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetAllCustomersQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UpdateCustomerAsync_UpdatesFields_WhenCustomerExists()
        {
            var customerId = Guid.NewGuid();
            var request = CreateUpdateCustomerRequest(
                firstName: "Jane",
                username: "janedoe",
                password: "NewPassword123",
                street: "456 New St"
            );
            var response = CreateCustomer(
                id: customerId,
                firstName: "Jane",
                username: "janedoe",
                passwordHash: "newhashedpassword",
                street: "456 New St"
            );
            _mediator
                .Send(
                    Arg.Is<UpdateCustomerCommand>(c => c.Id == customerId && c.Request == request),
                    Arg.Any<CancellationToken>()
                )
                .Returns(response);

            var result = await _customerService.UpdateCustomerAsync(customerId, request);

            result.Should().BeTrue();
            response.Name.FirstName.Should().Be("Jane");
            response.Username.Should().Be("janedoe");
            response.Address.Street.Should().Be("456 New St");
            response.PasswordHash.Should().NotBeNull();
            await _mediator
                .Received(1)
                .Send(Arg.Any<UpdateCustomerCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UpdateCustomerAsync_ReturnsFalse_WhenCustomerDoesNotExist()
        {
            var customerId = Guid.NewGuid();
            var request = CreateUpdateCustomerRequest();
            _mediator
                .Send(
                    Arg.Is<UpdateCustomerCommand>(c => c.Id == customerId && c.Request == request),
                    Arg.Any<CancellationToken>()
                )
                .Returns((Customer?)null);

            var result = await _customerService.UpdateCustomerAsync(customerId, request);

            result.Should().BeFalse();
            await _mediator
                .Received(1)
                .Send(Arg.Any<UpdateCustomerCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task DeleteCustomerAsync_ReturnsTrue_WhenCustomerExists()
        {
            var customerId = Guid.NewGuid();
            var accounts = new List<Account>
            {
                new Account(
                    "Account1",
                    "user1",
                    AccountType.Current,
                    3000,
                    AccountTier.TierThree,
                    "NGN",
                    customerId,
                    "1234567890",
                    "1234"
                )
                {
                    AccountId = 1,
                },
                new Account(
                    "Account2",
                    "user1",
                    AccountType.Current,
                    4000,
                    AccountTier.TierThree,
                    "NGN",
                    customerId,
                    "0987654321",
                    "4321"
                )
                {
                    AccountId = 2,
                },
            }.AsReadOnly();
            _mediator
                .Send(
                    Arg.Is<DeleteCustomerCommand>(c => c.Id == customerId),
                    Arg.Any<CancellationToken>()
                )
                .Returns(true);

            var result = await _customerService.DeleteCustomerAsync(customerId);

            result.Should().BeTrue();
            await _mediator
                .Received(1)
                .Send(Arg.Any<DeleteCustomerCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task DeleteCustomerAsync_ReturnsFalse_WhenCustomerDoesNotExist()
        {
            var customerId = Guid.NewGuid();
            _mediator
                .Send(
                    Arg.Is<DeleteCustomerCommand>(c => c.Id == customerId),
                    Arg.Any<CancellationToken>()
                )
                .Returns(false);

            var result = await _customerService.DeleteCustomerAsync(customerId);

            result.Should().BeFalse();
            await _mediator
                .Received(1)
                .Send(Arg.Any<DeleteCustomerCommand>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsTrue_WhenCredentialsValid()
        {
            var username = "johndoe";
            var password = "Password123";
            _mediator
                .Send(
                    Arg.Is<GetCustomerByCredentialsQuery>(q =>
                        q.Username == username && q.Password == password
                    ),
                    Arg.Any<CancellationToken>()
                )
                .Returns(true);

            var result = await _customerService.AuthenticateAsync(username, password);

            result.Should().BeTrue();
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetCustomerByCredentialsQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsFalse_WhenCredentialsInvalid()
        {
            var username = "johndoe";
            var password = "WrongPassword";
            _mediator
                .Send(
                    Arg.Is<GetCustomerByCredentialsQuery>(q =>
                        q.Username == username && q.Password == password
                    ),
                    Arg.Any<CancellationToken>()
                )
                .Returns(false);

            var result = await _customerService.AuthenticateAsync(username, password);

            result.Should().BeFalse();
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetCustomerByCredentialsQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetCustomerByUsernameAsync_ReturnsCustomerDto_WhenCustomerExists()
        {
            var username = "johndoe";
            var response = CreateCustomerDto(
                Guid.NewGuid(),
                username,
                "John A Doe",
                "john.doe@example.com"
            );
            _mediator
                .Send(
                    Arg.Is<GetCustomerByUsernameQuery>(q => q.Username == username),
                    Arg.Any<CancellationToken>()
                )
                .Returns(response);

            var result = await _customerService.GetCustomerByUsernameAsync(username);

            result.Should().NotBeNull();
            result.Id.Should().Be(response.Id);
            result.Username.Should().Be(response.Username);
            result.FullName.Should().Be(response.FullName);
            result.Email.Should().Be(response.Email);
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetCustomerByUsernameQuery>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetCustomerByUsernameAsync_ReturnsNull_WhenCustomerDoesNotExist()
        {
            var username = "johndoe";
            _mediator
                .Send(
                    Arg.Is<GetCustomerByUsernameQuery>(q => q.Username == username),
                    Arg.Any<CancellationToken>()
                )
                .Returns((CustomerDto?)null);

            var result = await _customerService.GetCustomerByUsernameAsync(username);

            result.Should().BeNull();
            await _mediator
                .Received(1)
                .Send(Arg.Any<GetCustomerByUsernameQuery>(), Arg.Any<CancellationToken>());
        }
    }
}
