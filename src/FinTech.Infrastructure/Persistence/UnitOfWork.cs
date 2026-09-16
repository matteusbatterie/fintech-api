using FinTech.Domain.Common;
using FinTech.Domain.Interfaces;

namespace FinTech.Infrastructure.Persistence;

public class UnitOfWork(FinTechDbContext context, IDomainEventDispatcher dispatcher) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Collect events BEFORE saving
        var aggregatesWithEvents = context.ChangeTracker    // ChangeTracker still has everything staged
            .Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(a => a.DomainEvents.Count > 0)
            .ToList();

        var result = await context.SaveChangesAsync(cancellationToken);

        // Only dispatch if the save actually succeeded (an exception above skips this entirely)
        var allEvents = aggregatesWithEvents.SelectMany(a => a.DomainEvents).ToList();
        await dispatcher.DispatchAsync(allEvents, cancellationToken);

        foreach (var aggregate in aggregatesWithEvents)
            aggregate.ClearDomainEvents();

        return result;
    }
}
