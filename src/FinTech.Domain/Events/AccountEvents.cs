using FinTech.Domain.Common;
using FinTech.Domain.Common.ValueObjects;

namespace FinTech.Domain.Events;

public record AccountOpened(Guid AccountId, string Currency) : DomainEvent;
public record FundsDeposited(Guid AccountId, Money Amount) : DomainEvent;
public record FundsWithdrawn(Guid AccountId, Money Amount) : DomainEvent;
