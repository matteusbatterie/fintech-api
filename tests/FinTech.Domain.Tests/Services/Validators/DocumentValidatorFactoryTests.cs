using FinTech.Domain.Services.Validators;
using FluentAssertions;

namespace FinTech.Domain.Tests.Services.Validators;

public class DocumentValidatorFactoryTests
{
    [Fact]
    public void GetValidator_ShouldReturnCorrectType()
    {
        // Arrange
        var validator = new BrazilDocumentValidator();
        var factory = new DocumentValidatorFactory([validator]);

        // Act
        var result = factory.GetValidator("CPF");

        // Assert
        result.Should().BeOfType<BrazilDocumentValidator>();
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenDuplicatesExist()
    {
        // Arrange
        var v1 = new BrazilDocumentValidator();
        var v2 = new BrazilDocumentValidator();

        // Ac
        var act = () => new DocumentValidatorFactory([v1, v2]);

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Multiple validators registered*");
    }
}
