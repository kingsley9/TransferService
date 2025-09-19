using FluentValidation;

namespace TransferService.Application.Features.Customers.Commands.DeleteCustomer
{
    public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
    {
        public DeleteCustomerCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Customer ID is required");
        }
    }
}
