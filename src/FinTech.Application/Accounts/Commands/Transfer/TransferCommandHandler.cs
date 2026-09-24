using FinTech.Domain.Common.ValueObjects;
using FinTech.Domain.Interfaces;
using FinTech.Domain.Services;
using MediatR;

namespace FinTech.Application.Accounts.Commands.Transfer;

public class TransferCommandHandler(
    IAccountRepository accountRepository,
    ITransactionRepository transactionRepository,
    LedgerService ledgerService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<TransferCommand, Guid>
{
    public async Task<Guid> Handle(TransferCommand request, CancellationToken cancellationToken)
    {
        var origin = await accountRepository.GetByIdAsync(request.OriginAccountId)
            ?? throw new KeyNotFoundException($"Origin account with ID {request.OriginAccountId} not found.");
        var destination = await accountRepository.GetByIdAsync(request.DestinationAccountId)
            ?? throw new KeyNotFoundException($"Destination account with ID {request.DestinationAccountId} not found.");

        var transaction = ledgerService.CreateTransfer(
            origin, destination, new Money(request.Amount, request.Currency), request.Reference);

        accountRepository.Update(origin);
        accountRepository.Update(destination);
        await transactionRepository.AddAsync(transaction);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return transaction.Id;
    }
}
