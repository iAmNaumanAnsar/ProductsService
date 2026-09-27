using FluentValidation;
using ProductsService.Application.Dtos;

namespace ProductsService.Application.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Sku)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[A-Za-z0-9-]+$")
            .WithMessage("Sku may only contain letters, numbers and hyphens.");

        RuleFor(x => x.Color)
            .IsInEnum();

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0);
    }
}
