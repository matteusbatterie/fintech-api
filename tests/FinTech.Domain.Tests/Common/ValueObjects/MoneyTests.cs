using FinTech.Domain.Common.ValueObjects;
using FluentAssertions;

namespace FinTech.Domain.Tests.Common.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Addition_ShouldWork_WhenCurrenciesMatch()
    {
        var m1 = new Money(100, "BRL");
        var m2 = new Money(50, "BRL");

        var result = m1 + m2;

        result.Amount.Should().Be(150);
    }

    [Fact]
    public void Addition_ShouldThrow_WhenCurrenciesMismatch()
    {
        var m1 = new Money(100, "BRL");
        var m2 = new Money(100, "USD");

        var act = () => m1 + m2;

        act.Should().Throw<InvalidOperationException>();
    }
}
