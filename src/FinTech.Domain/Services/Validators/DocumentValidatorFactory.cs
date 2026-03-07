using FinTech.Domain.Interfaces;

namespace FinTech.Domain.Services.Validators;

public class DocumentValidatorFactory
{
    private readonly IEnumerable<IDocumentValidator> _validators;

    public DocumentValidatorFactory(IEnumerable<IDocumentValidator> validators)
    {
        var duplicates = validators
            .SelectMany(v => v.SupportedTypes)
            .Select(t => t.ToUpper())
            .GroupBy(t => t)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Count != 0)
        {
            throw new InvalidOperationException(
                $"Error: Multiple validators registered for: {string.Join(", ", duplicates)}");
        }

        _validators = validators;
    }

    public IDocumentValidator GetValidator(string type)
    {
        return _validators.FirstOrDefault(v => v.SupportedTypes.Contains(type.ToUpper()))
               ?? throw new NotSupportedException($"Document type {type} is not supported.");
    }
}
