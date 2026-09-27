using ProductsService.Application.Dtos;
using ProductsService.Application.Exceptions;
using ProductsService.Application.Interfaces;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Enums;

namespace ProductsService.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        if (await _repository.ExistsWithSkuAsync(request.Sku, cancellationToken))
            throw new DuplicateSkuException(request.Sku);

        var product = new Product(request.Name, request.Sku, request.Color, request.Price, request.StockQuantity);

        var created = await _repository.AddAsync(product, cancellationToken);

        return ToDto(created);
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(ProductColor? color = null, CancellationToken cancellationToken = default)
    {
        var products = await _repository.GetAllAsync(color, cancellationToken);

        return products.Select(ToDto).ToList();
    }

    private static ProductDto ToDto(Product product) => new(
        product.Id,
        product.Name,
        product.Sku,
        product.Color,
        product.Price,
        product.StockQuantity,
        product.CreatedAtUtc);
}
