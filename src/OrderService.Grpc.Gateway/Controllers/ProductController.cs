using Google.Protobuf.Collections;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Products.CreationService.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderService.Grpc.Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductService.ProductServiceClient _productService;

    public ProductController(ProductService.ProductServiceClient productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Add an array of products to the system
    /// </summary>
    [SwaggerResponse(StatusCodes.Status200OK, "Products added", typeof(long[]))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Error while adding products")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
    [HttpPut]
    public async Task<ActionResult> AddProductsAsync(
        [FromBody] RepeatedField<AddProductDto> products,
        CancellationToken cancellationToken)
    {
        var request = new AddProductsRequest { Products = { products } };
        AddProductsResponse response = await _productService.AddProductsAsync(request, cancellationToken: cancellationToken);
        return Ok(response.ProductsIds);
    }

    /// <summary>
    /// Query products with filters
    /// </summary>
    /// <returns>An array of products matching the provided filter</returns>
    [SwaggerResponse(StatusCodes.Status200OK, "Products queried", typeof(ProductDto[]))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Error while querying products")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
    [HttpGet]
    public async Task<ActionResult<ProductDto[]>> QueryProductsAsync(
        [FromQuery] ProductQuery query,
        CancellationToken cancellationToken)
    {
        AsyncServerStreamingCall<ProductDto> response = _productService.QueryProducts(query, cancellationToken: cancellationToken);

        var products = new List<ProductDto>();

        while (await response.ResponseStream.MoveNext(cancellationToken))
        {
            ProductDto product = response.ResponseStream.Current;
            products.Add(product);
        }

        return Ok(products);
    }
}