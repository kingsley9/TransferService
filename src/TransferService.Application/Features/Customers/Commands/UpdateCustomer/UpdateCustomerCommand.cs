using MediatR;
using TransferService.Application.DTO;
using TransferService.Domain.Entities;

namespace TransferService.Application.Features.Customers.Commands.UpdateCustomer
{
    public record UpdateCustomerCommand(Guid Id, UpdateCustomerRequest Request)
        : IRequest<Customer>;
}
