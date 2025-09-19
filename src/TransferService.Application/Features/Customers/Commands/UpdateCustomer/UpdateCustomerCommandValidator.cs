using FluentValidation;
using TransferService.Application.DTO;

namespace TransferService.Application.Features.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerRequest>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .When(x => !string.IsNullOrWhiteSpace(x.Username))
                .WithMessage("Username is required");
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .When(x => !string.IsNullOrWhiteSpace(x.FirstName))
                .WithMessage("First name is required");
            RuleFor(x => x.LastName)
                .NotEmpty()
                .When(x => !string.IsNullOrWhiteSpace(x.LastName))
                .WithMessage("Last name is required");
            RuleFor(x => x.Password)
                .MinimumLength(6)
                .When(x => !string.IsNullOrWhiteSpace(x.Password))
                .WithMessage("Password must be at least 6 characters");
        }
    }
}
