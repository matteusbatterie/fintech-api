using FinTech.Domain.Common.ValueObjects;
using FinTech.Domain.Entities;
using FinTech.Domain.Events;
using FinTech.Domain.Exceptions;

namespace FinTech.Domain.Tests.Entities;

public class AccountEventsTests
{
    [Fact]
    public void Constructor_RaisesAccountOpened()
    {
        var account = new Account("Jane Doe", new Document("12345678909", "CPF"), "BRL");

        var openedEvent = Assert.Single(account.DomainEvents);
        Assert.IsType<AccountOpened>(openedEvent);
    }

    [Fact]
    public void Deposit_RaisesFundsDeposited_WithCorrectAmount()
    {
        var account = new Account("Jane Doe", new Document("12345678909", "CPF"), "BRL");
        account.ClearDomainEvents(); // discard "AccountOpened" event from setup

        account.Deposit(new Money(100, "BRL"));

        var depositedEvent = Assert.Single(account.DomainEvents);
        var typed = Assert.IsType<FundsDeposited>(depositedEvent);
        Assert.Equal(100, typed.Amount.Amount);
    }

    [Fact]
    public void Withdraw_InsufficientFunds_DoesNotRaiseEvent()
    {
        var account = new Account("Jane Doe", new Document("12345678909", "CPF"), "BRL");
        account.ClearDomainEvents(); // discard "AccountOpened" event from setup

        Assert.Throws<InsufficientFundsException>(() => account.Withdraw(new Money(100, "BRL")));
        Assert.Empty(account.DomainEvents);
    }
}
