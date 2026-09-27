using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductsService.Application.Interfaces;
using ProductsService.Application.Services;

namespace ProductsService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));

        return services;
    }
}
