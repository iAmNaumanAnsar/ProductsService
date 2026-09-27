using Microsoft.EntityFrameworkCore;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Enums;

namespace ProductsService.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(ProductsDbContext context)
    {
        await context.Database.MigrateAsync();

        if (context.Products.Any())
            return;

        context.Products.AddRange(
            new Product("Wireless Mouse", "WM-001", ProductColor.Black, 19.99m, 150),
            new Product("Mechanical Keyboard", "MK-002", ProductColor.White, 79.99m, 60),
            new Product("USB-C Hub", "UH-003", ProductColor.Silver, 34.50m, 90),
            new Product("Gaming Monitor 27\"", "GM-004", ProductColor.Black, 249.00m, 25),
            new Product("Desk Lamp", "DL-005", ProductColor.Yellow, 14.75m, 200));

        await context.SaveChangesAsync();
    }
}
