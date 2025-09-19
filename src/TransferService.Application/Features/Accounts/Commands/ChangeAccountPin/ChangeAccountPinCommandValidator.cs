using FluentValidation;

namespace TransferService.Application.Features.Accounts.Commands.ChangeAccountPin
{
    public class ChangeAccountPinCommandValidator : AbstractValidator<ChangeAccountPinCommand>
    {
        public ChangeAccountPinCommandValidator()
        {
            RuleFor(x => x.AccountId).GreaterThan(0).WithMessage("Account ID must be positive");
            RuleFor(x => x.CurrentPin).NotEmpty().WithMessage("Current PIN is required");
            RuleFor(x => x.NewPin)
                .NotEmpty()
                .MinimumLength(4)
                .WithMessage("New PIN must be at least 4 characters");
        }
    }
}
