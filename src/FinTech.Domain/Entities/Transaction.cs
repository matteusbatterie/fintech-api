using FinTech.Domain.Common;
using FinTech.Domain.Common.ValueObjects;
using FinTech.Domain.Enums;
using FinTech.Domain.Events;

namespace FinTech.Domain.Entities;

public class Transaction(string reference) : AggregateRoot
{
    private readonly List<Entry> _entries = [];
    public IReadOnlyCollection<Entry> Entries => _entries.AsReadOnly();

    public DateTime PostedAt { get; private set; } = DateTime.UtcNow;
    public string Reference { get; private set; } = reference;
    public bool IsPosted { get; private set; } = false;


    // Required by EF Core for materialization
    private Transaction() : this(reference: default!) { }


    public void AddEntry(Guid accountId, Money amount, EntryType type, string description)
    {
        if (IsPosted)
            throw new InvalidOperationException("Cannot modify a posted transaction.");

        var entry = new Entry(accountId, amount, type, description);
        _entries.Add(entry);
    }

    public void Post()
    {
        if (IsPosted)
            throw new InvalidOperationException("Transaction already posted.");

        if (!IsBalanced())
            throw new InvalidOperationException("Transaction is not balanced.");

        IsPosted = true;
        PostedAt = DateTime.UtcNow;

        Raise(new TransactionPosted(Id, Reference));
    }

    public bool IsBalanced()
    {
        if (_entries.Count == 0) return false;

        // Guard: Ensure all entries use the same currency
        var currencies = _entries.Select(e => e.Amount.Currency).Distinct();
        if (currencies.Count() > 1)
            throw new InvalidOperationException("A single transaction cannot involve multiple currencies.");

        var total = _entries.Sum(e => e.Type == EntryType.Credit
            ? e.Amount.Amount
            : -e.Amount.Amount);

        return total == 0;
    }
}
