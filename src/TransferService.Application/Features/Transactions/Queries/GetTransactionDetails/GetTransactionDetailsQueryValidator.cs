using FluentValidation;

namespace TransferService.Application.Features.Transactions.Queries.GetTransactionDetails
{
    public class GetTransactionDetailsQueryValidator : AbstractValidator<GetTransactionDetailsQuery>
    {
        public GetTransactionDetailsQueryValidator()
        {
            RuleFor(x => x.AccountId).GreaterThan(0).WithMessage("Account ID must be positive");
            RuleFor(x => x.TransactionId)
                .GreaterThan(0)
                .WithMessage("Transaction ID must be positive");
        }
    }
}
