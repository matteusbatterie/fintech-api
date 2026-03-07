using FinTech.Domain.Common.ValueObjects;
using FinTech.Domain.Entities;
using FluentAssertions;

namespace FinTech.Domain.Tests.Entities;

public class AccountTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithZeroBalance()
    {
        var doc = new Document("12345678909", "CPF");
        var account = new Account("Battery", doc, "BRL");

        account.Balance.Amount.Should().Be(0);
        account.Balance.Currency.Should().Be("BRL");
    }

    [Fact]
    public void UpdateBalance_ShouldHandlePositiveAndNegativeMovements()
    {
        var account = new Account("Battery", new Document("1", "TEST"), "BRL");

        // Credit
        account.UpdateBalance(new Money(100, "BRL"));
        // Debit
        account.UpdateBalance(new Money(-30, "BRL"));

        account.Balance.Amount.Should().Be(70);
    }
}
