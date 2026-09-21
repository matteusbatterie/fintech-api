using FinTech.Domain.Common.ValueObjects;
using FinTech.Domain.Entities;
using FinTech.Domain.Exceptions;
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
    public void Deposit_ShouldIncreaseBalance()
    {
        var account = new Account("Battery", new Document("1", "TEST"), "BRL");

        account.Deposit(new Money(100, "BRL"));

        account.Balance.Amount.Should().Be(100);
    }

    [Fact]
    public void Withdraw_ShouldDecreaseBalance_WhenFundsAreSufficient()
    {
        var account = new Account("Battery", new Document("1", "TEST"), "BRL");
        account.Deposit(new Money(100, "BRL"));

        account.Withdraw(new Money(30, "BRL"));

        account.Balance.Amount.Should().Be(70);
    }

    [Fact]
    public void Withdraw_ShouldThrow_WhenFundsAreInsufficient()
    {
        var account = new Account("Battery", new Document("1", "TEST"), "BRL");
        account.Deposit(new Money(50, "BRL"));

        var act = () => account.Withdraw(new Money(100, "BRL"));

        act.Should().Throw<InsufficientFundsException>();
        account.Balance.Amount.Should().Be(50); // state unchanged on failure
    }

    [Fact]
    public void Withdraw_ShouldThrow_WhenCurrencyDoesNotMatch()
    {
        var account = new Account("Battery", new Document("1", "TEST"), "BRL");
        account.Deposit(new Money(100, "BRL"));

        var act = () => account.Withdraw(new Money(10, "USD"));

        act.Should().Throw<CurrencyMismatchException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Deposit_ShouldThrow_WhenAmountIsNotPositive(decimal amount)
    {
        var account = new Account("Battery", new Document("1", "TEST"), "BRL");

        var act = () => account.Deposit(new Money(amount, "BRL"));

        act.Should().Throw<ArgumentException>();
    }
}
