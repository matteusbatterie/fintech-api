namespace FinTech.Domain.Interfaces;

public interface IDocumentValidator
{
    IEnumerable<string> SupportedTypes { get; }
    bool IsValid(string document);
}
