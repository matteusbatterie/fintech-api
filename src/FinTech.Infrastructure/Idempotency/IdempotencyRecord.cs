namespace FinTech.Infrastructure.Idempotency;

public class IdempotencyRecord
{
    public string Key { get; set; } = default!;
    public string RequestType { get; set; } = default!;
    public string RequestHash { get; set; } = default!;
    public string? ResponsePayload { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
}
