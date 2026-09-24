namespace FinTech.Application.Common;

public interface IIdempotentRequest
{
    string IdempotencyKey { get; }
}
