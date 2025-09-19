using MediatR;
using TransferService.Application.DTO;
using TransferService.Domain.Entities;

namespace TransferService.Application.Features.Customers.Commands.CreateCustomer
{
    public record CreateCustomerCommand(CreateCustomerRequest Request) : IRequest<Customer>;
}
