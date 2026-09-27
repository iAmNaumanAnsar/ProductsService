using FluentAssertions;
using Moq;
using ProductsService.Application.Dtos;
using ProductsService.Application.Exceptions;
using ProductsService.Application.Interfaces;
using ProductsService.Application.Services;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Enums;
using Xunit;

namespace ProductsService.UnitTests.Application;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repository = new();
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _sut = new ProductService(_repository.Object);
    }

    [Fact]
    public async Task CreateAsync_WithNewSku_AddsProductAndReturnsDto()
    {
        _repository.Setup(r => r.ExistsWithSkuAsync("SKU-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repository.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => p);

        var request = new CreateProductRequest("Mouse", "SKU-1", ProductColor.Black, 19.99m, 100);

        var result = await _sut.CreateAsync(request);

        result.Name.Should().Be("Mouse");
        result.Sku.Should().Be("SKU-1");
        result.Color.Should().Be(ProductColor.Black);
        _repository.Verify(r => r.AddAsync(It.Is<Product>(p => p.Sku == "SKU-1"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateSku_ThrowsAndDoesNotAdd()
    {
        _repository.Setup(r => r.ExistsWithSkuAsync("SKU-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new CreateProductRequest("Mouse", "SKU-1", ProductColor.Black, 19.99m, 100);

        var act = async () => await _sut.CreateAsync(request);

        await act.Should().ThrowAsync<DuplicateSkuException>();
        _repository.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_WithNoColorFilter_ReturnsAllMappedProducts()
    {
        var products = new List<Product>
        {
            new("Mouse", "SKU-1", ProductColor.Black, 19.99m, 100),
            new("Keyboard", "SKU-2", ProductColor.White, 49.99m, 40)
        };
        _repository.Setup(r => r.GetAllAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
        result.Select(p => p.Sku).Should().Contain(new[] { "SKU-1", "SKU-2" });
    }

    [Fact]
    public async Task GetAllAsync_WithColorFilter_PassesFilterThroughToRepository()
    {
        _repository.Setup(r => r.GetAllAsync(ProductColor.Red, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { new("Red Mug", "SKU-3", ProductColor.Red, 9.99m, 10) });

        var result = await _sut.GetAllAsync(ProductColor.Red);

        result.Should().ContainSingle();
        result[0].Color.Should().Be(ProductColor.Red);
        _repository.Verify(r => r.GetAllAsync(ProductColor.Red, It.IsAny<CancellationToken>()), Times.Once);
    }
}
