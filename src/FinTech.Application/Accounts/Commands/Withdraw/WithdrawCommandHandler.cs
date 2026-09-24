using FinTech.Domain.Common.ValueObjects;
using FinTech.Domain.Interfaces;
using MediatR;

namespace FinTech.Application.Accounts.Commands.Withdraw;

public class WithdrawCommandHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<WithdrawCommand>
{
    public async Task Handle(WithdrawCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId)
            ?? throw new KeyNotFoundException($"Account with ID {request.AccountId} not found.");

        account.Withdraw(new Money(request.Amount, request.Currency));

        accountRepository.Update(account);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
