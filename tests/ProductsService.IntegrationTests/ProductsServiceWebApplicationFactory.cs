using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductsService.Infrastructure.Persistence;

namespace ProductsService.IntegrationTests;

/// <summary>
/// Boots the real Api pipeline (auth, middleware, DI) against a throwaway
/// SQLite file per test run instead of the dev database, so tests can run
/// in parallel and never touch developer data.
/// </summary>
public class ProductsServiceWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"productsservice-tests-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ProductsDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<ProductsDbContext>(options =>
                options.UseSqlite($"Data Source={_dbPath}"));
        });
    }

    public new void Dispose()
    {
        base.Dispose();
        SqliteConnection.ClearAllPools();
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }
}
