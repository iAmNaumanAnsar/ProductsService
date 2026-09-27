using ProductsService.Domain.Entities;
using ProductsService.Domain.Enums;

namespace ProductsService.Application.Interfaces;

public interface IProductRepository
{
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetAllAsync(ProductColor? color = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithSkuAsync(string sku, CancellationToken cancellationToken = default);
}
