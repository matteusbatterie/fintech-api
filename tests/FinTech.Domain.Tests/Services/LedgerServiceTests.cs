using FinTech.Domain.Common.ValueObjects;
using FinTech.Domain.Entities;
using FinTech.Domain.Services;
using FluentAssertions;

namespace FinTech.Domain.Tests.Services;

public class LedgerServiceTests
{
    [Fact]
    public void CreateTransfer_ShouldUpdateBalances_WhenFundsAreSufficient()
    {
        // Arrange
        var service = new LedgerService();
        var origin = new Account("John Doe", new Document("123", "TEST"), "BRL");
        var destination = new Account("Jane Doe", new Document("456", "TEST"), "BRL");

        origin.UpdateBalance(new Money(1000, "BRL"));

        // Act
        var transaction = service.CreateTransfer(origin, destination, new Money(400, "BRL"), "Rent");

        // Assert
        origin.Balance.Amount.Should().Be(600);
        destination.Balance.Amount.Should().Be(400);
        transaction.IsBalanced().Should().BeTrue();
    }

    [Fact]
    public void CreateTransfer_ShouldAllow_WhenAmountEqualsTotalBalance()
    {
        // Arrange
        var service = new LedgerService();
        var origin = new Account("Battery", new Document("1", "TEST"), "BRL");
        var destination = new Account("Thais", new Document("2", "TEST"), "BRL");

        origin.UpdateBalance(new Money(500, "BRL"));

        // Act
        service.CreateTransfer(origin, destination, new Money(500, "BRL"), "Empty out"); // Transferring everything

        // Assert
        origin.Balance.Amount.Should().Be(0);
        destination.Balance.Amount.Should().Be(500);
    }

    [Fact]
    public void CreateTransfer_ShouldThrow_WhenInsufficientFunds()
    {
        // Arrange
        var service = new LedgerService();
        var origin = new Account("John Doe", new Document("1", "TEST"), "BRL");
        var destination = new Account("Jane Doe", new Document("2", "TEST"), "BRL");

        // Act
        var act = () => service.CreateTransfer(origin, destination, new Money(100, "BRL"), "Fail");

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Insufficient funds*");
    }

    [Fact]
    public void CreateTransfer_ShouldNotChangeBalance_WhenOriginAndDestinationAreSame()
    {
        // Arrange
        var service = new LedgerService();
        var acc = new Account("Battery", new Document("123", "TEST"), "BRL");
        var initialBalance = new Money(500, "BRL");
        acc.UpdateBalance(initialBalance);

        // Act
        var act = () => service.CreateTransfer(acc, acc, new Money(100, "BRL"), "Self");

        // Assert
        act.Should().Throw<InvalidOperationException>();
        acc.Balance.Amount.Should().Be(500); // Verify state didn't change
    }
}
