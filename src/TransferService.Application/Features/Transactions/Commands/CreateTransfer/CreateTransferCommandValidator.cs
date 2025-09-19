using FluentValidation;
using TransferService.Application.DTO;

namespace TransferService.Application.Features.Transactions.Commands.CreateTransfer
{
    public class CreateTransferCommandValidator : AbstractValidator<CreateTransferCommand>
    {
        public CreateTransferCommandValidator()
        {
            RuleFor(x => x.Request.AccountId)
                .GreaterThan(0)
                .WithMessage("Account ID must be positive");
            RuleFor(x => x.Request.TargetAccountId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Target account ID must be provided and positive");
            RuleFor(x => x.Request.Amount).GreaterThan(0).WithMessage("Amount must be positive");
            RuleFor(x => x.Request.Pin).NotEmpty().WithMessage("PIN is required");
        }
    }
}
