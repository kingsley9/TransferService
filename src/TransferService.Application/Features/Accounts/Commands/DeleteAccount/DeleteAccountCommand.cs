using MediatR;

namespace TransferService.Application.Features.Accounts.Commands.DeleteAccount
{
    public record DeleteAccountCommand(int AccountId) : IRequest<bool>;
}
