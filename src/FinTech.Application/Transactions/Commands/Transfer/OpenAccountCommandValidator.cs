using FinTech.Application.Accounts.Commands.OpenAccount;
using FluentValidation;

namespace FinTech.Application.Transactions.Commands.Transfer;

public class OpenAccountCommandValidator : AbstractValidator<OpenAccountCommand>
{
    public OpenAccountCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DocumentNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.DocumentType).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}
