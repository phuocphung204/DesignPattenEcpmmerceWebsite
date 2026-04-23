using DesignPattern.Application.Features.Seos.Shared;
using DesignPattern.Domain.Abstractions;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.ProductCategory;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Extensions;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.ValueObjects.Seo;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Commands.CreateCategory;

public class CreateProductCategoryHandler : IRequestHandler<CreateProductCategoryCommand, Result<ProductCategoryResponse>>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ISlugChecker _slugChecker;

  public CreateProductCategoryHandler(IUnitOfWork unitOfWork, ISlugChecker slugChecker)
  {
    _unitOfWork = unitOfWork;
    _slugChecker = slugChecker;
  }

  // public async Task<Result<ProductCategoryResponse>> Handle(CreateProductCategoryCommand request, CancellationToken cancellationToken)
  // {
  //   var dto = request.dto;

  //   var slugResult = await GenerateUniqueSlugAsync(dto.Name, cancellationToken);
  //   if (slugResult.IsFailure)
  //   {
  //     return Result<ProductCategoryResponse>.Failure(slugResult.Error);
  //   }

  //   var parentCategoryResult = await GetParentCategoryAsync(dto.ParentCategoryId, cancellationToken);
  //   if (parentCategoryResult.IsFailure)
  //   {
  //     return Result<ProductCategoryResponse>.Failure(parentCategoryResult.Error);
  //   }

  //   var createResult = CreateCategory(dto, slugResult.Value, parentCategoryResult.Value);
  //   if (createResult.IsFailure)
  //   {
  //     return Result<ProductCategoryResponse>.Failure(createResult.Error);
  //   }

  //   await PersistCategoryAsync(createResult.Value, cancellationToken);

  //   return Result<ProductCategoryResponse>.Success(ToResponse(createResult.Value));
  // }

  public async Task<Result<ProductCategoryResponse>> Handle(CreateProductCategoryCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;

    var generateSlugTask = _slugChecker.GenerateUniqueSlugAsync(dto.Name, cancellationToken);
    var parentCategoryTask = GetParentCategoryAsync(dto.ParentCategoryId, cancellationToken);
    await Task.WhenAll(generateSlugTask, parentCategoryTask);

    var slugResult = await generateSlugTask;
    var parentCategoryResult = await parentCategoryTask;

    var createResult = slugResult
      .Bind(slug => parentCategoryResult
        .Bind(parentCategory => CreateCategory(dto, slug, parentCategory)));

    var persistedResult = await createResult
      .TapAsync(category => PersistCategoryAsync(category, cancellationToken));

    return persistedResult.Map(ProductCategoryResponse.FromDomain);

  }

  private async Task<Result<ProductCategory?>> GetParentCategoryAsync(string? parentCategoryId, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(parentCategoryId))
    {
      return Result<ProductCategory?>.Success(null);
    }

    // đã validate ở validator nên chắc chắn parse được, nếu không sẽ là lỗi server
    var parsedParentCategoryId = Guid.Parse(parentCategoryId);

    var parentCategory = await _unitOfWork.ProductCategoryRepository.GetByIdAsync(parsedParentCategoryId, cancellationToken);
    if (parentCategory is null)
    {
      return Result<ProductCategory?>.Failure(ProductCategoryErrors.InvalidParentCategoryId);
    }

    return Result<ProductCategory?>.Success(parentCategory);
  }

  private static Result<ProductCategory> CreateCategory(CreateProductCategoryDTO dto, string slug, ProductCategory? parentCategory)
  {
    return ProductCategory.Create(
      name: dto.Name,
      level: parentCategory?.Level?.Value + 1 ?? 0,
      metaTitle: dto.Title,
      slug: slug,
      metaDescription: dto.MetaDescription,
      parentCategory: parentCategory);
  }

  private async Task PersistCategoryAsync(ProductCategory category, CancellationToken cancellationToken)
  {
    _unitOfWork.ProductCategoryRepository.Create(category);
    await _unitOfWork.SaveChangesAsync(cancellationToken);
  }

  // private static ProductCategoryResponse ToResponse(ProductCategory category)
  // {
  //   return ProductCategoryResponse.FromDomain(category);
  // }



}
