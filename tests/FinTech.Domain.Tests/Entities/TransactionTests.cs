using FinTech.Domain.Common.ValueObjects;
using FinTech.Domain.Entities;
using FinTech.Domain.Enums;
using FluentAssertions;

namespace FinTech.Domain.Tests.Entities;

public class TransactionTests
{
    [Fact]
    public void IsBalanced_ShouldReturnFalse_WhenSumIsNotZero()
    {
        var tx = new Transaction("Unbalanced test");
        tx.AddEntry(Guid.NewGuid(), new Money(100, "BRL"), EntryType.Debit, "Out");
        tx.AddEntry(Guid.NewGuid(), new Money(50, "BRL"), EntryType.Credit, "Incomplete back");

        tx.IsBalanced().Should().BeFalse();
    }

    [Fact]
    public void IsBalanced_ShouldThrow_WhenCurrenciesAreMixed()
    {   
        var tx = new Transaction("Mixed currency error");
        tx.AddEntry(Guid.NewGuid(), new Money(100, "BRL"), EntryType.Debit, "BRL side");
        tx.AddEntry(Guid.NewGuid(), new Money(100, "USD"), EntryType.Credit, "USD side");

        var act = () => tx.IsBalanced();

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*multiple currencies*");
    }
}
