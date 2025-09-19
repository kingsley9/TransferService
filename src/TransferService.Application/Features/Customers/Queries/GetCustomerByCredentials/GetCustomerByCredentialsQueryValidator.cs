using FluentValidation;

namespace TransferService.Application.Features.Customers.Queries.GetCustomerByCredentials
{
    public class GetCustomerByCredentialsQueryValidator
        : AbstractValidator<GetCustomerByCredentialsQuery>
    {
        public GetCustomerByCredentialsQueryValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        }
    }
}
