using ProductsService.Domain.Enums;

namespace ProductsService.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Sku { get; private set; } = default!;
    public ProductColor Color { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Product() { }

    public Product(string name, string sku, ProductColor color, decimal price, int stockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("Product SKU is required.", nameof(sku));

        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");

        if (stockQuantity < 0)
            throw new ArgumentOutOfRangeException(nameof(stockQuantity), "Stock quantity cannot be negative.");

        Id = Guid.NewGuid();
        Name = name.Trim();
        Sku = sku.Trim().ToUpperInvariant();
        Color = color;
        Price = price;
        StockQuantity = stockQuantity;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
