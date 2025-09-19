using MediatR;
using TransferService.Application.DTO;
using TransferService.Domain.Enums;

namespace TransferService.Application.Features.Accounts.Commands.CreateAccount
{
    public record CreateAccountCommand : IRequest<AccountDetailsResponse>
    {
        public AccountRequest Account { get; init; } = null!;
        public string OwnerName { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
        public Guid CustomerId { get; init; }
    }
}
