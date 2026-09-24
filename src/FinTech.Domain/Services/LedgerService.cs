using FinTech.Domain.Common.ValueObjects;
using FinTech.Domain.Entities;
using FinTech.Domain.Enums;

namespace FinTech.Domain.Services;

/// <summary>
/// Represents a service responsible for handling ledger operations, including the creation of transfer transactions
/// </summary>
public class LedgerService
{
    /// <summary>
    /// Creates a transfer transaction between two accounts, ensuring that the origin and destination accounts
    /// are different and that the transaction is balanced according to double-entry bookkeeping principles.
    ///
    /// Since it mutates the state of the involved accounts, it should not be static and should be used within
    /// a transactional context to ensure atomicity.
    /// </summary>
    /// <returns>The created transaction.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the origin and destination accounts are the same.</exception>
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
