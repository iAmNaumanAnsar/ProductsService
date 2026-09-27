using ProductsService.Application.Dtos;
using ProductsService.Domain.Enums;

namespace ProductsService.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductDto>> GetAllAsync(ProductColor? color = null, CancellationToken cancellationToken = default);
}
