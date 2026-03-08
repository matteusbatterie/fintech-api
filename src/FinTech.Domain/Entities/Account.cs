using FinTech.Domain.Common;
using FinTech.Domain.Common.ValueObjects;

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

    public void UpdateBalance(Money amount)
    {
        Balance += amount;
    }
}
