using FluentValidation;

namespace TransferService.Application.Features.Customers.Queries.GetAllCustomers
{
    public class GetAllCustomersQueryValidator : AbstractValidator<GetAllCustomersQuery>
    {
        public GetAllCustomersQueryValidator() { }
    }
}
