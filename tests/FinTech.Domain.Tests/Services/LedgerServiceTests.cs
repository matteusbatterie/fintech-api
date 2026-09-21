using FinTech.Domain.Common.ValueObjects;
using FinTech.Domain.Entities;
using FinTech.Domain.Exceptions;
using FinTech.Domain.Services;
using FluentAssertions;

namespace FinTech.Domain.Tests.Services;

public class LedgerServiceTests
{
    [Fact]
    public void CreateTransfer_ShouldUpdateBalances_WhenFundsAreSufficient()
    {
        var service = new LedgerService();
        var origin = new Account("John Doe", new Document("123", "TEST"), "BRL");
        var destination = new Account("Jane Doe", new Document("456", "TEST"), "BRL");

        origin.Deposit(new Money(1000, "BRL"));

        var transaction = service.CreateTransfer(origin, destination, new Money(400, "BRL"), "Rent");

        origin.Balance.Amount.Should().Be(600);
        destination.Balance.Amount.Should().Be(400);
        transaction.IsPosted.Should().BeTrue();
    }

    [Fact]
    public void CreateTransfer_ShouldAllow_WhenAmountEqualsTotalBalance()
    {
        var service = new LedgerService();
        var origin = new Account("John Doe", new Document("123", "TEST"), "BRL");
        var destination = new Account("Jane Doe", new Document("456", "TEST"), "BRL");

        origin.Deposit(new Money(500, "BRL"));

        service.CreateTransfer(origin, destination, new Money(500, "BRL"), "Empty out");

        origin.Balance.Amount.Should().Be(0);
        destination.Balance.Amount.Should().Be(500);
    }

    [Fact]
    public void CreateTransfer_ShouldThrow_WhenInsufficientFunds()
    {
        var service = new LedgerService();
        var origin = new Account("John Doe", new Document("123", "TEST"), "BRL");
        var destination = new Account("Jane Doe", new Document("456", "TEST"), "BRL");

        var act = () => service.CreateTransfer(origin, destination, new Money(100, "BRL"), "Fail");

        act.Should().Throw<InsufficientFundsException>();
    }

    [Fact]
    public void CreateTransfer_ShouldNotChangeBalance_WhenOriginAndDestinationAreSame()
    {
        var service = new LedgerService();
        var acc = new Account("John Doe", new Document("123", "TEST"), "BRL");
        acc.Deposit(new Money(500, "BRL"));

        var act = () => service.CreateTransfer(acc, acc, new Money(100, "BRL"), "Self");

        act.Should().Throw<InvalidOperationException>();
        acc.Balance.Amount.Should().Be(500);
    }
}
