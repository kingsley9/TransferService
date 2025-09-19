using FluentValidation;

namespace TransferService.Application.Features.Accounts.Commands.CreateAccount
{
    public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountCommandValidator()
        {
            RuleFor(x => x.Account).NotNull().WithMessage("Account details are required");
            RuleFor(x => x.Account.BankCode).NotEmpty().WithMessage("Bank code is required");
            RuleFor(x => x.Account.SchemeCode).NotEmpty().WithMessage("Scheme code is required");
            RuleFor(x => x.Account.Type).IsInEnum().WithMessage("Invalid account type");
            RuleFor(x => x.Account.Tier).IsInEnum().WithMessage("Invalid account tier");
            RuleFor(x => x.Account.Currency).NotEmpty().WithMessage("Currency is required");
            RuleFor(x => x.Account.Pin)
                .NotEmpty()
                .MinimumLength(4)
                .WithMessage("PIN must be at least 4 characters");
            RuleFor(x => x.OwnerName).NotEmpty().WithMessage("Owner name is required");
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
            RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Customer ID is required");
        }
    }
}
