using DesignPattern.Application.Features.Seos.Shared;
using DesignPattern.Domain.Abstractions;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.ProductCategory;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Extensions;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Commands.UpdateProductCategory;

public class UpdateProductCategoryHandler : IRequestHandler<UpdateProductCategoryCommand, Result<ProductCategoryResponse>>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ISlugChecker _slugChecker;

  public UpdateProductCategoryHandler(IUnitOfWork unitOfWork, ISlugChecker slugChecker)
  {
    _unitOfWork = unitOfWork;
    _slugChecker = slugChecker;
  }

  public async Task<Result<ProductCategoryResponse>> Handle(UpdateProductCategoryCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;

    var updateResult = await CheckExistAsync(request.Id, cancellationToken)
      .BindAsync(category => GetParentCategoryAsync(dto.ParentCategoryId, cancellationToken)
        .BindAsync(async parentCategory =>
        {
          // Tạo slug nếu có name
          var slugResult = dto.Name is not null ?
            await _slugChecker.GenerateUniqueSlugAsync(dto.Name, cancellationToken)
            : Result<string>.Success(null!);
          return slugResult.Bind(slug => UpdateCategory(category!, slug, parentCategory, dto));
        }));

    var persistedResult = await updateResult
      .TapAsync(category => PersistCategoryAsync(category, cancellationToken));

    return persistedResult.Map(ProductCategoryResponse.FromDomain);
  }

  /// <summary>
  /// Kiểm tra tồn tại của category
  /// </summary>
  /// <param name="id"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  private async Task<Result<ProductCategory?>> CheckExistAsync(Guid id, CancellationToken cancellationToken)
  {
    var category = await _unitOfWork.ProductCategoryRepository.GetByIdAsync(id, cancellationToken);
    if (category == null)
    {
      return Result<ProductCategory?>.Failure(ProductCategoryErrors.NotFound);
    }
    return Result<ProductCategory?>.Success(category);
  }

  /// <summary>
  /// Lấy parent category
  /// </summary>
  /// <param name="parentCategoryId"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  private async Task<Result<ProductCategory?>> GetParentCategoryAsync(string? parentCategoryId, CancellationToken cancellationToken)
  {
    if (parentCategoryId == null) return Result<ProductCategory?>.Success(null);

    var parentId = Guid.Parse(parentCategoryId);
    var category = await _unitOfWork.ProductCategoryRepository.GetByIdAsync(parentId, cancellationToken);
    if (category == null)
    {
      return Result<ProductCategory?>.Failure(ProductCategoryErrors.InvalidParentCategoryId);
    }
    return Result<ProductCategory?>.Success(category);
  }

  /// <summary>
  /// Cập nhật category
  /// </summary>
  /// <param name="category"></param>
  /// <param name="slug"></param>
  /// <param name="parentCategory"></param>
  /// <param name="dto"></param>
  /// <returns></returns>
  private static Result<ProductCategory> UpdateCategory(ProductCategory category, string? slug, ProductCategory? parentCategory, UpdateProductCategoryDTO dto)
  {
    var updateResult = category.Update(
      name: dto.Name,
      metaTitle: dto.Title,
      metaDescription: dto.MetaDescription,
      slug: slug
    );
    if (updateResult.IsFailure)
    {
      return Result<ProductCategory>.Failure(updateResult.Error);
    }

    updateResult = category.ChangeParentCategory(parentCategory);
    if (updateResult.IsFailure)
    {
      return Result<ProductCategory>.Failure(updateResult.Error);
    }

    return Result<ProductCategory>.Success(category);
  }

  private async Task PersistCategoryAsync(ProductCategory category, CancellationToken cancellationToken)
  {
    _unitOfWork.ProductCategoryRepository.Update(category);
    await _unitOfWork.SaveChangesAsync(cancellationToken);
  }

}
