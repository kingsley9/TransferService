using FluentValidation;

namespace TransferService.Application.Features.Accounts.Commands.UpdateAccount
{
    public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
    {
        public UpdateAccountCommandValidator()
        {
            RuleFor(x => x.AccountId).GreaterThan(0).WithMessage("Account ID must be positive");
            RuleFor(x => x.AccountName)
                .NotEmpty()
                .When(x => x.AccountName != null)
                .WithMessage("Account name cannot be empty");

            RuleFor(x => x.Type)
                .IsInEnum()
                .When(x => x.Type.HasValue)
                .WithMessage("Invalid account type");

            RuleFor(x => x.Tier)
                .IsInEnum()
                .When(x => x.Tier.HasValue)
                .WithMessage("Invalid account tier");

            RuleFor(x => x)
                .Must(x => x.AccountName != null || x.Type.HasValue || x.Tier.HasValue)
                .WithMessage("At least one of AccountName, Type, or Tier is required");
        }
    }
}
