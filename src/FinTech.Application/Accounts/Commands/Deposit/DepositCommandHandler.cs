using FinTech.Domain.Common.ValueObjects;
using FinTech.Domain.Interfaces;
using MediatR;

namespace FinTech.Application.Accounts.Commands.Deposit;

public class DepositCommandHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DepositCommand>
{
    public async Task Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId)
            ?? throw new KeyNotFoundException($"Account with ID {request.AccountId} not found.");

        account.Deposit(new Money(request.Amount, request.Currency));

        accountRepository.Update(account);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
