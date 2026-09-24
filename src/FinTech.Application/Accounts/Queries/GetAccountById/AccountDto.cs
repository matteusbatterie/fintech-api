namespace FinTech.Application.Accounts.Queries.GetAccountById;

public record AccountDto(Guid Id, string Name, decimal Balance, string Currency, DateTime CreatedAt);
