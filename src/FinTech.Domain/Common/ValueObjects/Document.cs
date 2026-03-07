namespace FinTech.Domain.Common.ValueObjects;

public record Document
{
    public string Number { get; init; }
    public string Type { get; init; }

    public Document(string number, string type)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Document number is required.", nameof(number));

        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Document type is required.", nameof(type));

        Number = new string([.. number.Where(char.IsLetterOrDigit)]).ToUpper();
        Type = type.ToUpper();
    }

    public override string ToString() => $"{Type}: {Number}";
}
