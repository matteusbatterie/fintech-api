using FinTech.Domain.Common;

namespace FinTech.Domain.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default);
}
