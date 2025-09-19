using MediatR;

namespace TransferService.Application.Features.Customers.Commands.DeleteCustomer
{
    public record DeleteCustomerCommand(Guid Id) : IRequest<bool>;
}
