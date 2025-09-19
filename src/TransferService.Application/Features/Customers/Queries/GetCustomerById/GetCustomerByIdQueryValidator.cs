using FluentValidation;

namespace TransferService.Application.Features.Customers.Queries.GetCustomerById
{
    public class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
    {
        public GetCustomerByIdQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Customer ID is required");
        }
    }
}
