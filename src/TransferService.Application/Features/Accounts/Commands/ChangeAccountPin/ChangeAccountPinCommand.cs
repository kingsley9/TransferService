using MediatR;

namespace TransferService.Application.Features.Accounts.Commands.ChangeAccountPin
{
    public record ChangeAccountPinCommand : IRequest
    {
        public int AccountId { get; init; }
        public string CurrentPin { get; init; } = string.Empty;
        public string NewPin { get; init; } = string.Empty;
    }
}
