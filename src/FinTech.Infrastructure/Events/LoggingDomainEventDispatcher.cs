using FinTech.Domain.Common;
using FinTech.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FinTech.Infrastructure.Events;

public class LoggingDomainEventDispatcher(ILogger<LoggingDomainEventDispatcher> logger)
    : IDomainEventDispatcher
{
    public Task DispatchAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default)
    {
        if (!logger.IsEnabled(LogLevel.Information))
        {
            return Task.CompletedTask;
        }

        foreach (var domainEvent in events)
        {
            logger.LogInformation(
                "Domain event dispatched: {EventType} at {OccurredAt} - {@Event}",
                domainEvent.GetType().Name,
                domainEvent.OccurredAt,
                domainEvent
            );
        }

        return Task.CompletedTask;
    }
}
