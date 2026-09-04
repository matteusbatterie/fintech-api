using FinTech.Domain.Common;
using FinTech.Domain.Common.ValueObjects;
using FinTech.Domain.Exceptions;

namespace FinTech.Domain.Entities;

public class Account : BaseEntity
{
    public string Name { get; private set; }
    public Document Document { get; private set; }
    public Money Balance { get; private set; }


    // Required by EF Core for materialization
    private Account() 
    {
        Name = default!;
        Document = default!;
        Balance = default!;
    }

    public Account(string name, Document document, string currency)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name must be provided.", nameof(name));

        Name = name;
        Document = document;
        Balance = new Money(0, currency);
    }

    [Obsolete("Use Deposit and Withdraw methods instead.")]
    public void UpdateBalance(Money amount)
    {
        Balance += amount;
    }

    public void Deposit(Money amount)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException("Deposit amount must be positive.", nameof(amount));

        EnsureSameCurrency(amount);
        Balance += amount;
    }

    public void Withdraw(Money amount)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException("Withdrawal amount must be positive.", nameof(amount));

        EnsureSameCurrency(amount);

        if (Balance.Amount < amount.Amount)
            throw new InsufficientFundsException(Id, amount);

        Balance -= amount;
    }

    private void EnsureSameCurrency(Money amount)
    {
        if (amount.Currency != Balance.Currency)
            throw new CurrencyMismatchException(Balance.Currency, amount.Currency);
    }
}
