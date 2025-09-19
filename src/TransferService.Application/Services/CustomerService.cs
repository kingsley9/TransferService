using MediatR;
using TransferService.Application.DTO;
using TransferService.Application.Features.Customers.Commands.CreateCustomer;
using TransferService.Application.Features.Customers.Commands.DeleteCustomer;
using TransferService.Application.Features.Customers.Commands.UpdateCustomer;
using TransferService.Application.Features.Customers.Queries.GetAllCustomers;
using TransferService.Application.Features.Customers.Queries.GetCustomerByCredentials;
using TransferService.Application.Features.Customers.Queries.GetCustomerById;
using TransferService.Application.Features.Customers.Queries.GetCustomerByUsername;
using TransferService.Application.Interfaces;
using TransferService.Domain.Entities;

namespace TransferService.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IMediator _mediator;

        public CustomerService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Customer> RegisterCustomerAsync(CreateCustomerRequest request)
        {
            var command = new CreateCustomerCommand(request);
            return await _mediator.Send(command);
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(Guid id)
        {
            return await _mediator.Send(new GetCustomerByIdQuery(id));
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            return await _mediator.Send(new GetAllCustomersQuery());
        }

        public async Task<bool> UpdateCustomerAsync(Guid id, UpdateCustomerRequest request)
        {
            var command = new UpdateCustomerCommand(id, request);
            var customer = await _mediator.Send(command);
            return customer != null;
        }

        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            return await _mediator.Send(new DeleteCustomerCommand(id));
        }

        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            return await _mediator.Send(new GetCustomerByCredentialsQuery(username, password));
        }

        public async Task<CustomerDto?> GetCustomerByUsernameAsync(string username)
        {
            return await _mediator.Send(new GetCustomerByUsernameQuery(username));
        }
    }
}
