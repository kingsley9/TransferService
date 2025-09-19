using FluentValidation;

namespace TransferService.Application.Features.Accounts.Queries.GetAccountBalance
{
    public class GetAccountBalanceQueryValidator : AbstractValidator<GetAccountBalanceQuery>
    {
        public GetAccountBalanceQueryValidator()
        {
            RuleFor(x => x.AccountId).GreaterThan(0).WithMessage("Account ID must be positive");
        }
    }
}
