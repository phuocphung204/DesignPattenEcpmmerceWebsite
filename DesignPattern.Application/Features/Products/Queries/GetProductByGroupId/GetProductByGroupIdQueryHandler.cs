using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Products.Shared;
using DesignPattern.Domain.Repositories;

namespace DesignPattern.Application.Features.Products.Queries.GetProductByGroupId;

public class GetProductByGroupIdQueryHandler : IRequestHandler<GetProductByGroupIdQuery, Result<List<ProductOnGroupResponse>>>
{
  private readonly IUnitOfWork _unitOfWork;
  public GetProductByGroupIdQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<List<ProductOnGroupResponse>>> Handle(GetProductByGroupIdQuery request, CancellationToken cancellationToken)
  {
    var products = await _unitOfWork.ProductRepository.GetProductsByVariantGroupIdAsync(request.VariantGroupId, cancellationToken);

    foreach (var product in products)
    {
      var seo = await _unitOfWork.SeoRepository.GetByReferenceIdAsync(product.Id, cancellationToken);
      product.SetSeo(seo);
    }

    var response = products.Select(p => new ProductOnGroupResponse(
      Id: p.Id,
      Name: p.Name.Value,
      VariantGroupId: p.VariantGroupId.Value,
      Image: p.Images.FirstOrDefault() ?? string.Empty,
      SellingPrice: p.SellingPrice.Amount,
      SoldQuantity: p.SoldQuantity.Value,
      Rating: p.Rating,
      ShortDescription: p.ShortDescription,
      Seo: new SeoResponse(p.Seo.Title?.Value, p.Seo.Description?.Value, p.Seo.Slug?.Value)
    )).ToList();
    return response;
  }
}