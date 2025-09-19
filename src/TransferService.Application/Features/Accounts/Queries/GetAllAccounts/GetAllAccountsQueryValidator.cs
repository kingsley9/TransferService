using FluentValidation;

namespace TransferService.Application.Features.Accounts.Queries.GetAllAccounts
{
    public class GetAllAccountsQueryValidator : AbstractValidator<GetAllAccountsQuery>
    {
        public GetAllAccountsQueryValidator() { }
    }
}
