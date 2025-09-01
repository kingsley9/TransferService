using TransferService.Domain.Entities;
using TransferService.Domain.Enums;

namespace TransferService.Domain.Rules
{
    public interface IAccountRule
    {
        AccountType? AccountType { get; }
        AccountTier? Tier { get; }
        string? Currency { get; }

        void Validate(AccountValidationContext context);
    }
}
