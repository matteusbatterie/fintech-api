namespace FinTech.Domain.Common;

public abstract record DomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
