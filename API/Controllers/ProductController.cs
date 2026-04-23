using DesignPattern.Application.Features.Products.Commands.CreateProduct;
using DesignPattern.Application.Features.Products.Commands.DeleteProduct;
using DesignPattern.Application.Features.Products.Commands.UpdateProduct;
using DesignPattern.Application.Features.Products.Queries.GetProductById;
using DesignPattern.Application.Features.Products.Queries.GetProductByGroupId;
using DesignPattern.Application.Features.Products.Queries.GetProducts;
using DesignPattern.Application.Helpers;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductController : ControllerBase
{
  private readonly IMediator _mediator;

  public ProductController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpGet("/api/products")]
  public async Task<IActionResult> GetProducts(
    [FromQuery] string? name,
    [FromQuery] Guid? brandId,
    [FromQuery] Guid? categoryId,
    [FromQuery] string? variantGroupId,
    [FromQuery] string? sku,
    [FromQuery] decimal? minRating,
    [FromQuery] decimal? maxRating,
    [FromQuery] decimal? minPrice,
    [FromQuery] decimal? maxPrice,
    [FromQuery] Field? sortBy,
    [FromQuery] Direction? sortDirection,
    [FromQuery] int pageIndex = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
  {
    // var selectedField = field ?? file ?? sortBy;
    // var selectedDirection = direction ?? sortDirection;

    var criteria = new ProductSearchCriteria
    {
      Name = name,
      BrandId = brandId,
      CategoryId = categoryId,
      VariantGroupId = variantGroupId,
      Sku = sku,
      MinRating = minRating,
      MaxRating = maxRating,
      MinPrice = minPrice,
      MaxPrice = maxPrice,
      SortOption = (sortBy.HasValue && sortDirection.HasValue) ? new SortOption
      {
        sortBy = sortBy.Value,
        sortDirection = sortDirection.Value
      } : null,
      PageIndex = pageIndex,
      PageSize = pageSize
    };

    var query = new GetProductsQuery(criteria);
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Lấy danh sách sản phẩm thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(result.Value, "Lấy danh sách sản phẩm thành công"));
  }

  [HttpGet("/api/products/{id:guid}")]
  public async Task<IActionResult> GetProductById([FromRoute] Guid id, CancellationToken cancellationToken)
  {
    var query = new GetProductByIdCommand(id);
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure && result.Error is NotFoundError)
    {
      return NotFound(ApiResponse.FailureResult(
        "Không tìm thấy sản phẩm",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Lấy thông tin sản phẩm thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(result.Value, "Lấy thông tin sản phẩm thành công"));
  }

  [HttpGet("/api/products/by-group")]
  public async Task<IActionResult> GetProductsByGroupId([FromQuery] string variantGroupId, CancellationToken cancellationToken)
  {
    var query = new GetProductByGroupIdQuery(variantGroupId);
    var result = await _mediator.Send(query, cancellationToken);

    if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Lấy danh sách sản phẩm theo nhóm biến thể thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(result.Value, "Lấy danh sách sản phẩm theo nhóm biến thể thành công"));
  }

  [HttpPost("/api/admin/products")]
  public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO request, CancellationToken cancellationToken)
  {
    var command = new CreateProductCommand(request);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure && result.Error is NotFoundError)
    {
      return NotFound(ApiResponse.FailureResult(
        "Không tìm thấy thương hiệu hoặc danh mục sản phẩm",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure && result.Error is ConflictError)
    {
      return Conflict(ApiResponse.FailureResult(
        "Tạo sản phẩm thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Tạo sản phẩm thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return CreatedAtAction(
      nameof(GetProductById),
      new { id = result.Value.Id },
      ApiResponse.SuccessResult(result.Value, "Tạo sản phẩm thành công"));
  }

  [HttpPatch("/api/admin/products/{id:guid}")]
  public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductDto request, CancellationToken cancellationToken)
  {
    var command = new UpdateProductCommand(id, request);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure && result.Error is NotFoundError)
    {
      return NotFound(ApiResponse.FailureResult(
        "Không tìm thấy sản phẩm để cập nhật",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure && result.Error is ConflictError)
    {
      return Conflict(ApiResponse.FailureResult(
        "Cập nhật sản phẩm thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Cập nhật sản phẩm thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(result.Value, "Cập nhật sản phẩm thành công"));
  }

  [HttpDelete("/api/admin/products/{id:guid}")]
  public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, CancellationToken cancellationToken)
  {
    var command = new DeleteProductCommand(id);
    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsFailure && result.Error is NotFoundError)
    {
      return NotFound(ApiResponse.FailureResult(
        "Không tìm thấy sản phẩm để xóa",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure && result.Error is ConflictError)
    {
      return Conflict(ApiResponse.FailureResult(
        "Xóa sản phẩm thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }
    else if (result.IsFailure)
    {
      return BadRequest(ApiResponse.FailureResult(
        "Xóa sản phẩm thất bại",
        result.Error.Code,
        SemicolonSeparatedListParser.Parse(result.Error.Message)));
    }

    return Ok(ApiResponse.SuccessResult(new { }, "Xóa sản phẩm thành công"));
  }
}
