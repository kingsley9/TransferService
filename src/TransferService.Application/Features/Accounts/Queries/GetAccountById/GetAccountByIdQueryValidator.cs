using FluentValidation;

namespace TransferService.Application.Features.Accounts.Queries.GetAccountById
{
    public class GetAccountByIdQueryValidator : AbstractValidator<GetAccountByIdQuery>
    {
        public GetAccountByIdQueryValidator()
        {
            RuleFor(x => x.AccountId).GreaterThan(0).WithMessage("Account ID must be positive");
        }
    }
}
