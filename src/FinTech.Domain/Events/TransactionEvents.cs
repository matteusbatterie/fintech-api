using FinTech.Domain.Common;

namespace FinTech.Domain.Events;

public record TransactionPosted(Guid TransactionId, string Reference) : DomainEvent;
