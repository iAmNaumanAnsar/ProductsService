using FluentValidation.TestHelper;
using ProductsService.Application.Dtos;
using ProductsService.Application.Validators;
using ProductsService.Domain.Enums;
using Xunit;

namespace ProductsService.UnitTests.Application;

public class CreateProductRequestValidatorTests
{
    private readonly CreateProductRequestValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        var request = new CreateProductRequest("Mouse", "SKU-1", ProductColor.Black, 19.99m, 100);

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyName_HasErrorForName()
    {
        var request = new CreateProductRequest("", "SKU-1", ProductColor.Black, 19.99m, 100);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("SKU 1")]
    [InlineData("SKU_1")]
    [InlineData("SKU#1")]
    public void Validate_WithInvalidSkuCharacters_HasErrorForSku(string sku)
    {
        var request = new CreateProductRequest("Mouse", sku, ProductColor.Black, 19.99m, 100);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Sku);
    }

    [Fact]
    public void Validate_WithNegativePrice_HasErrorForPrice()
    {
        var request = new CreateProductRequest("Mouse", "SKU-1", ProductColor.Black, -1m, 100);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_WithNegativeStockQuantity_HasErrorForStockQuantity()
    {
        var request = new CreateProductRequest("Mouse", "SKU-1", ProductColor.Black, 19.99m, -5);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.StockQuantity);
    }
}
