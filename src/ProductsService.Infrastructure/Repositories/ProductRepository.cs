using Microsoft.EntityFrameworkCore;
using ProductsService.Application.Interfaces;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Enums;
using ProductsService.Infrastructure.Persistence;

namespace ProductsService.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductsDbContext _context;

    public ProductRepository(ProductsDbContext context)
    {
        _context = context;
    }

    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(ProductColor? color = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsNoTracking().AsQueryable();

        if (color.HasValue)
            query = query.Where(p => p.Color == color.Value);

        return await query
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsWithSkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        var normalized = sku.Trim().ToUpperInvariant();
        return _context.Products.AsNoTracking().AnyAsync(p => p.Sku == normalized, cancellationToken);
    }
}
