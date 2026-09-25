using FinTech.Application.Accounts.Commands.Deposit;
using FluentValidation;

namespace FinTech.Application.Transactions.Commands.Transfer;

public class DepositCommandValidator : AbstractValidator<DepositCommand>
{
    public DepositCommandValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
        RuleFor(x => x.IdempotencyKey).NotEmpty();
    }
}
