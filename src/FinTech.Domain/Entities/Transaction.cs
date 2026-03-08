using FinTech.Domain.Common;
using FinTech.Domain.Common.ValueObjects;
using FinTech.Domain.Enums;

namespace FinTech.Domain.Entities;

public class Transaction(string reference) : BaseEntity
{
    private readonly List<Entry> _entries = [];
    public IReadOnlyCollection<Entry> Entries => _entries.AsReadOnly();

    public DateTime PostedAt { get; private set; } = DateTime.UtcNow;
    public string Reference { get; private set; } = reference;


    // Required by EF Core for materialization
    private Transaction() : this(string.Empty) { }


    public void AddEntry(Guid accountId, Money amount, EntryType type, string description)
    {
        var entry = new Entry(accountId, amount, type, description);
        _entries.Add(entry);
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
