using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using OrderService.Grpc.Gateway.Models.Products;
using Swashbuckle.AspNetCore.Annotations;
using Pb = Products.CreationService.Contracts;

namespace OrderService.Grpc.Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly Pb.ProductService.ProductServiceClient _productService;

    public ProductController(Pb.ProductService.ProductServiceClient productService)
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
        [FromBody] IEnumerable<ProductDto> products,
        CancellationToken cancellationToken)
    {
        IEnumerable<Pb.ProductDto> pbProducts = products
            .Select(x => new Pb.ProductDto { Name = x.Name, Price = (double)x.Price });

        var request = new Pb.AddProductsRequest { Products = { pbProducts } };
        Pb.AddProductsResponse response = await _productService.AddProductsAsync(request, cancellationToken: cancellationToken);

        return Ok(response.ProductsIds.ToArray());
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
        var q = new Pb.ProductQuery
        {
            Ids = { query.Ids },
            NamePattern = query.NamePattern,
            MinPrice = query.MinPrice,
            MaxPrice = query.MaxPrice,
            Cursor = query.Cursor,
            PageSize = query.PageSize,
        };

        AsyncServerStreamingCall<Pb.ProductDto> response = _productService.QueryProducts(q, cancellationToken: cancellationToken);

        var products = new List<ProductDto>();

        while (await response.ResponseStream.MoveNext(cancellationToken))
        {
            Pb.ProductDto p = response.ResponseStream.Current;
            products.Add(new ProductDto(p.Name, (decimal)p.Price));
        }

        return Ok(products);
    }
}