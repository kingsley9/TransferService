using MediatR;
using TransferService.Application.Features.Customers.Commands.DeleteCustomer;
using TransferService.Application.Interfaces;

namespace TransferService.Application.Features.Customers.Commands.DeleteCustomer
{
    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;

        public DeleteCustomerCommandHandler(
            ICustomerRepository customerRepository,
            IAccountRepository accountRepository
        )
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
        }

        public async Task<bool> Handle(
            DeleteCustomerCommand request,
            CancellationToken cancellationToken
        )
        {
            var customer = await _customerRepository.GetCustomerWithAccountsByIdAsync(request.Id);
            if (customer == null)
                return false;

            await _accountRepository.DeleteRangeAsync(customer.Accounts!);
            await _customerRepository.DeleteAsync(customer);
            return true;
        }
    }
}
