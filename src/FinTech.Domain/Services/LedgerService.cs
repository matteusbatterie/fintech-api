using FinTech.Domain.Common.ValueObjects;
using FinTech.Domain.Entities;
using FinTech.Domain.Enums;

namespace FinTech.Domain.Services;

public class LedgerService
{
    public Transaction CreateTransfer(Account origin, Account destination, Money amount, string reference)
    {
        if (origin.Id == destination.Id)
            throw new InvalidOperationException("Origin and destination accounts must be different.");

        var transaction = new Transaction(reference);

        // Double entry bookkeeping rule: For every Debit, there must be a Credit
        transaction.AddEntry(origin.Id, amount, EntryType.Debit, $"Transfer to {destination.Name}");
        transaction.AddEntry(destination.Id, amount, EntryType.Credit, $"Transfer from {origin.Name}");

        transaction.Post();

        // Update the actual account entities
        origin.Withdraw(amount);
        destination.Deposit(amount);

        return transaction;
    }
}
