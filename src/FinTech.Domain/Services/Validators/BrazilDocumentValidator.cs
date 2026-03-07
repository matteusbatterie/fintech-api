using FinTech.Domain.Interfaces;

namespace FinTech.Domain.Services.Validators;

/// <summary>
/// Provides validation for Brazilian document numbers, specifically CPF (Cadastro de Pessoas Físicas) and CNPJ
/// (Cadastro Nacional da Pessoa Jurídica).
/// </summary>
/// <remarks>This class supports validation of CPF and CNPJ formats, ensuring that the input adheres to Brazilian
/// standards for these identification numbers. It exposes a list of supported document types and offers methods to
/// check the validity of provided document strings. Non-digit characters are automatically ignored during validation,
/// allowing for flexible input formats.</remarks>
public class BrazilDocumentValidator : IDocumentValidator
{
    public IEnumerable<string> SupportedTypes => ["CPF", "CNPJ"];

    /// <summary>
    /// Determines whether the specified document string represents a valid CPF or CNPJ number.
    /// </summary>
    /// <remarks>A valid CPF consists of 11 digits, and a valid CNPJ consists of 14 digits. The method
    /// automatically removes any non-digit characters from the input before performing validation.</remarks>
    /// <param name="document">The document string to validate. The string may contain non-digit characters, which will be ignored during
    /// validation.</param>
    /// <returns>true if the document is a valid CPF or CNPJ number; otherwise, false.</returns>
    public bool IsValid(string document)
    {
        var sanitized = new string([.. document.Where(char.IsDigit)]);

        return sanitized.Length switch
        {
            11 => IsValidCpf(sanitized),
            14 => IsValidCnpj(sanitized),
            _ => false
        };
    }

    /// <summary>
    /// Determines whether the specified CPF (Cadastro de Pessoas Físicas) number is valid according to Brazilian
    /// validation rules.
    /// </summary>
    /// <remarks>This method checks the CPF number against standard validation rules, including the
    /// calculation of check digits. The input should be a string containing only digits, without formatting characters
    /// such as periods or hyphens.</remarks>
    /// <param name="cpf">The CPF number to validate. This value must be a string of 11 digits and cannot consist of the same digit
    /// repeated.</param>
    /// <returns>true if the CPF number is valid; otherwise, false.</returns>
    private static bool IsValidCpf(string cpf)
    {
        if (cpf.All(c => c == cpf[0])) return false;

        int[] multiplier1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] multiplier2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

        return ValidateCheckDigits(cpf, multiplier1, multiplier2);
    }

    /// <summary>
    /// Determines whether the specified CNPJ (Cadastro Nacional da Pessoa Jurídica) number is valid according to its
    /// format and check digits.
    /// </summary>
    /// <remarks>This method checks for invalid CNPJs consisting of repeated digits and validates the check
    /// digits using standard CNPJ algorithms. Ensure that the input string is properly formatted and contains only
    /// digits before calling this method.</remarks>
    /// <param name="cnpj">The CNPJ number to validate. This value must be a string containing exactly 14 numeric digits.</param>
    /// <returns>true if the CNPJ is valid; otherwise, false.</returns>
    private static bool IsValidCnpj(string cnpj)
    {
        if (cnpj.All(c => c == cnpj[0])) return false;

        int[] multiplier1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] multiplier2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        return ValidateCheckDigits(cnpj, multiplier1, multiplier2);
    }

    /// <summary>
    /// Validates whether the specified string of digits ends with check digits calculated using the provided
    /// multipliers.
    /// </summary>
    /// <remarks>This method is typically used to validate identification numbers that use a two-stage check
    /// digit algorithm, such as certain Brazilian document numbers. The validation process involves calculating two
    /// check digits sequentially using weighted sums and modulus operations.</remarks>
    /// <param name="digits">A string containing the digits to validate. The string must be at least as long as the total number of
    /// multipliers required for validation.</param>
    /// <param name="m1">An array of integers representing the multipliers used to calculate the first check digit.</param>
    /// <param name="m2">An array of integers representing the multipliers used to calculate the second check digit.</param>
    /// <returns>true if the input string ends with the calculated check digits; otherwise, false.</returns>
    private static bool ValidateCheckDigits(string digits, int[] m1, int[] m2)
    {
        string temp = digits[..m1.Length];
        int sum = 0;

        for (int i = 0; i < m1.Length; i++)
            sum += (temp[i] - '0') * m1[i];

        int remainder = sum % 11;
        remainder = remainder < 2 ? 0 : 11 - remainder;

        string digit = remainder.ToString();
        temp += digit;
        sum = 0;

        for (int i = 0; i < m2.Length; i++)
            sum += (temp[i] - '0') * m2[i];

        remainder = sum % 11;
        remainder = remainder < 2 ? 0 : 11 - remainder;
        digit += remainder.ToString();

        return digits.EndsWith(digit);
    }
}
