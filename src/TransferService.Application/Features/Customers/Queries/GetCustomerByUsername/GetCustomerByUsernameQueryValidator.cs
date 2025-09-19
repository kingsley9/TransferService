using FluentValidation;

namespace TransferService.Application.Features.Customers.Queries.GetCustomerByUsername
{
    public class GetCustomerByUsernameQueryValidator : AbstractValidator<GetCustomerByUsernameQuery>
    {
        public GetCustomerByUsernameQueryValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
        }
    }
}
