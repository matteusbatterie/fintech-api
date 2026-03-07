using FinTech.Domain.Services.Validators;
using FluentAssertions;

namespace FinTech.Domain.Tests.Services.Validators;

public class BrazilDocumentValidatorTests
{
    private readonly BrazilDocumentValidator _validator = new();

    [Theory]
    [InlineData("84047044067", true)]  // Valid CPF
    [InlineData("11111111111", false)] // Same digits
    [InlineData("12345678901", false)] // Invalid math
    [InlineData("840.470.440-67", true)] // Masked but valid
    public void IsValid_Cpf_ShouldReturnExpected(string input, bool expected)
    {
        _validator.IsValid(input).Should().Be(expected);
    }

    [Theory]
    [InlineData("57630547000198", true)]  // Valid CNPJ
    [InlineData("00000000000000", false)] // Same digits
    [InlineData("12345678000199", false)] // Invalid math
    [InlineData("57.630.547/0001-98", true)] // Masked but valid
    public void IsValid_Cnpj_ShouldReturnExpected(string input, bool expected)
    {
        _validator.IsValid(input).Should().Be(expected);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenLengthIsInvalid()
    {
        _validator.IsValid("12345").Should().BeFalse();
    }
}
