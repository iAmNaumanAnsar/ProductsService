using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using ProductsService.Application.Dtos;
using ProductsService.Domain.Enums;
using ProductsService.IntegrationTests.Helpers;
using Xunit;

namespace ProductsService.IntegrationTests;

public class ProductsControllerTests : IClassFixture<ProductsServiceWebApplicationFactory>
{
    private readonly ProductsServiceWebApplicationFactory _factory;

    public ProductsControllerTests(ProductsServiceWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAll_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateAndGetAll_WithToken_RoundTripsProduct()
    {
        var client = _factory.CreateClient();
        var token = await AuthHelper.GetAccessTokenAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var uniqueSku = $"IT-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var createResponse = await client.PostAsJsonAsync("/api/products", new CreateProductRequest(
            "Integration Test Widget", uniqueSku, ProductColor.Green, 12.34m, 7));

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductDto>(JsonOptions.Default);
        created!.Sku.Should().Be(uniqueSku);
        created.Color.Should().Be(ProductColor.Green);

        var getAllResponse = await client.GetAsync("/api/products");
        getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await getAllResponse.Content.ReadFromJsonAsync<List<ProductDto>>(JsonOptions.Default);
        products.Should().Contain(p => p.Sku == uniqueSku);
    }

    [Fact]
    public async Task Create_WithDuplicateSku_ReturnsConflict()
    {
        var client = _factory.CreateClient();
        var token = await AuthHelper.GetAccessTokenAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var uniqueSku = $"DUP-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var request = new CreateProductRequest("Duplicate Widget", uniqueSku, ProductColor.Blue, 5.00m, 1);

        var firstResponse = await client.PostAsJsonAsync("/api/products", request);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var secondResponse = await client.PostAsJsonAsync("/api/products", request);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_WithInvalidRequest_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();
        var token = await AuthHelper.GetAccessTokenAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var invalidRequest = new CreateProductRequest("", "BAD SKU", ProductColor.Blue, -5.00m, -1);

        var response = await client.PostAsJsonAsync("/api/products", invalidRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAll_FilteredByColor_ReturnsOnlyMatchingProducts()
    {
        var client = _factory.CreateClient();
        var token = await AuthHelper.GetAccessTokenAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var redSku = $"RED-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        await client.PostAsJsonAsync("/api/products", new CreateProductRequest("Red Widget", redSku, ProductColor.Red, 3.00m, 2));

        var response = await client.GetAsync("/api/products?color=Red");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>(JsonOptions.Default);
        products.Should().OnlyContain(p => p.Color == ProductColor.Red);
        products.Should().Contain(p => p.Sku == redSku);
    }
}
