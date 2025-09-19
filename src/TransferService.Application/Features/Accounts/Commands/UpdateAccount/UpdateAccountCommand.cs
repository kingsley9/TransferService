using MediatR;
using TransferService.Application.DTO;
using TransferService.Domain.Enums;

namespace TransferService.Application.Features.Accounts.Commands.UpdateAccount
{
    public record UpdateAccountCommand : IRequest<AccountDetailsResponse>
    {
        public int AccountId { get; init; }
        public string? AccountName { get; init; }
        public AccountType? Type { get; init; }
        public AccountTier? Tier { get; init; }
    }
}
