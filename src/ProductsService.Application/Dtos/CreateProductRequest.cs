using ProductsService.Domain.Enums;

namespace ProductsService.Application.Dtos;

public record CreateProductRequest(
    string Name,
    string Sku,
    ProductColor Color,
    decimal Price,
    int StockQuantity);
