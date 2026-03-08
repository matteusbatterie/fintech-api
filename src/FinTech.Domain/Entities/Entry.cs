using FinTech.Domain.Common;
using FinTech.Domain.Common.ValueObjects;
using FinTech.Domain.Enums;

namespace FinTech.Domain.Entities;

public class Entry : BaseEntity
{
    public Guid AccountId { get; private set; }
    public Money Amount { get; private set; }
    public EntryType Type { get; private set; }
    public string Description { get; private set; }


    // Required by EF Core for materialization
    private Entry() 
    {
        AccountId = Guid.Empty;
        Amount = default!;
        Type = default!;
        Description = string.Empty;
    }

    // Internal constructor: Entries should only be created by a Transaction
    internal Entry(Guid accountId, Money amount, EntryType type, string description)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        AccountId = accountId;
        Amount = amount;
        Type = type;
        Description = description;
    }
}
