using FluentAssertions;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Enums;
using Xunit;

namespace ProductsService.UnitTests.Domain;

public class ProductTests
{
    [Fact]
    public void Constructor_WithValidArguments_SetsPropertiesAndGeneratesId()
    {
        var product = new Product("Desk Lamp", "dl-005", ProductColor.Yellow, 14.75m, 200);

        product.Id.Should().NotBeEmpty();
        product.Name.Should().Be("Desk Lamp");
        product.Sku.Should().Be("DL-005");
        product.Color.Should().Be(ProductColor.Yellow);
        product.Price.Should().Be(14.75m);
        product.StockQuantity.Should().Be(200);
        product.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithMissingName_Throws(string? name)
    {
        var act = () => new Product(name!, "SKU-1", ProductColor.Black, 10m, 1);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithNegativePrice_Throws()
    {
        var act = () => new Product("Name", "SKU-1", ProductColor.Black, -1m, 1);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_WithNegativeStockQuantity_Throws()
    {
        var act = () => new Product("Name", "SKU-1", ProductColor.Black, 10m, -1);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
