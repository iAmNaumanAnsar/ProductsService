using ProductsService.Domain.Enums;

namespace ProductsService.Application.Dtos;

public record ProductDto(
    Guid Id,
    string Name,
    string Sku,
    ProductColor Color,
    decimal Price,
    int StockQuantity,
    DateTime CreatedAtUtc);
