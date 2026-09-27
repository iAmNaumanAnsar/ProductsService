using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService.Application.Dtos;
using ProductsService.Application.Exceptions;
using ProductsService.Application.Interfaces;
using ProductsService.Domain.Enums;

namespace ProductsService.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IValidator<CreateProductRequest> _validator;

    public ProductsController(IProductService productService, IValidator<CreateProductRequest> validator)
    {
        _productService = productService;
        _validator = validator;
    }

    /// <summary>Creates a new product.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            return ValidationProblem(ModelState);
        }

        try
        {
            var created = await _productService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetAll), new { }, created);
        }
        catch (DuplicateSkuException ex)
        {
            return Conflict(new ProblemDetails { Title = ex.Message, Status = StatusCodes.Status409Conflict });
        }
    }

    /// <summary>
    /// Returns all products, optionally filtered to a single colour
    /// (e.g. <c>GET /api/products?color=Red</c>).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] ProductColor? color, CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllAsync(color, cancellationToken);
        return Ok(products);
    }
}
