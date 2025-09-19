using FluentValidation;
using TransferService.Application.DTO;

namespace TransferService.Application.Features.Transactions.Commands.CreateDeposit
{
    public class CreateDepositCommandValidator : AbstractValidator<CreateDepositCommand>
    {
        public CreateDepositCommandValidator()
        {
            RuleFor(x => x.Request.AccountId)
                .GreaterThan(0)
                .WithMessage("Account ID must be positive");
            RuleFor(x => x.Request.Amount).GreaterThan(0).WithMessage("Amount must be positive");
            RuleFor(x => x.Request.Pin).NotEmpty().WithMessage("PIN is required");
            RuleFor(x => x.Request.TargetAccountId)
                .Null()
                .WithMessage("Target account ID should not be provided for deposits");
        }
    }
}
