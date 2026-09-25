using FinTech.Application.Accounts.Commands.Transfer;
using FluentValidation;

namespace FinTech.Application.Transactions.Commands.Transfer;

public class TransferCommandValidator : AbstractValidator<TransferCommand>
{
    public TransferCommandValidator()
    {
        RuleFor(x => x.OriginAccountId).NotEmpty();
        RuleFor(x => x.DestinationAccountId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
        RuleFor(x => x.Reference).NotEmpty().MaximumLength(200);
        RuleFor(x => x.IdempotencyKey).NotEmpty();
    }
}
