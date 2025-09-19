using FluentValidation;

namespace TransferService.Application.Features.Accounts.Queries.GetOwnedAccount
{
    public class GetOwnedAccountQueryValidator : AbstractValidator<GetOwnedAccountQuery>
    {
        public GetOwnedAccountQueryValidator()
        {
            RuleFor(x => x.AccountId).GreaterThan(0).WithMessage("Account ID must be positive");
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
        }
    }
}
