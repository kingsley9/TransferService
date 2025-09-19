using FluentValidation;

namespace TransferService.Application.Features.Transactions.Queries.GetTransactionsForAccount
{
    public class GetTransactionsForAccountQueryValidator
        : AbstractValidator<GetTransactionsForAccountQuery>
    {
        public GetTransactionsForAccountQueryValidator()
        {
            RuleFor(x => x.AccountId).GreaterThan(0).WithMessage("Account ID must be positive");
        }
    }
}
