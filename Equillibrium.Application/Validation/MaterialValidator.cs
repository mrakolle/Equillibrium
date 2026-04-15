using FluentValidation;
using Equillibrium.Core.Entities;


namespace Equillibrium.Application.Validation;

public class MaterialValidator : AbstractValidator<Material>
{
    public MaterialValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);
        RuleFor(x => x.BaseUnit).NotEmpty();
        
        // REMOVED RuleFor(x => x.TenantId) because it's no longer in the Entity
    }
}
