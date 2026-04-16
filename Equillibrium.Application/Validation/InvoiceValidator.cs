using FluentValidation;
using Equillibrium.Sales.Core.Entities;

namespace Equillibrium.Application.Validation;

public class InvoiceValidator : AbstractValidator<Invoice>
{
    public InvoiceValidator()
    {
        RuleFor(x => x.InvoiceNumber)
            .NotEmpty().WithMessage("Invoice number is required.");

        RuleFor(x => x.IssueDate)
            .NotEmpty();

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(x => x.IssueDate)
            .WithMessage("Due date cannot be earlier than issue date.");

        RuleFor(x => x.ContactId)
            .NotEmpty().WithMessage("A customer or supplier must be selected.");

        RuleFor(x => x.TenantId)
            .NotEmpty();
            
        // Validate nested items
        RuleForEach(x => x.Items).SetValidator(new InvoiceItemValidator());
    }
}

public class InvoiceItemValidator : AbstractValidator<InvoiceItem>
{
    public InvoiceItemValidator()
    {
        RuleFor(x => x.MaterialId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
    }
}
