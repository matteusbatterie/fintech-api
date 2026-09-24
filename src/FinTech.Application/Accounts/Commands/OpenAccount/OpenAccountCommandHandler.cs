using FinTech.Domain.Common.ValueObjects;
using FinTech.Domain.Entities;
using FinTech.Domain.Interfaces;
using FinTech.Domain.Services.Validators;
using MediatR;

namespace FinTech.Application.Accounts.Commands.OpenAccount;

public class OpenAccountCommandHandler(
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork,
    DocumentValidatorFactory documentValidatorFactory)
    : IRequestHandler<OpenAccountCommand, Guid>
{
    public async Task<Guid> Handle(OpenAccountCommand request, CancellationToken cancellationToken)
    {
        var validator = documentValidatorFactory.GetValidator(request.DocumentType);
        if (!validator.IsValid(request.DocumentNumber))
            throw new ArgumentException($"Invalid {request.DocumentType} document number.");

        var document = new Document(request.DocumentNumber, request.DocumentType);
        var account = new Account(request.Name, document, request.Currency);

        await accountRepository.AddAsync(account);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return account.Id;
    }
}
