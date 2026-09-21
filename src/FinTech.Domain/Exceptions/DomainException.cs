using FinTech.Domain.Common.ValueObjects;

namespace FinTech.Domain.Exceptions;

public abstract class DomainException(string message) : Exception(message);

public class InsufficientFundsException(Guid accountId, Money attempted)
    : DomainException($"Account {accountId} has insufficient funds for the attempted transaction of {attempted.Amount} {attempted.Currency}.");

public class CurrencyMismatchException(string expected, string actual)
    : DomainException($"Currency mismatch: expected {expected}, but received {actual}.");

public class ConcurrencyConflictException(Guid accountId)
    : DomainException($"Account {accountId} has been modified by another operation. Please retry.");
