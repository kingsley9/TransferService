using FluentValidation;

namespace TransferService.Application.Features.Accounts.Queries.GetAccountsByCustomer
{
    public class GetAccountsByCustomerQueryValidator : AbstractValidator<GetAccountsByCustomerQuery>
    {
        public GetAccountsByCustomerQueryValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Customer ID is required");
        }
    }
}
