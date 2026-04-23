using DesignPattern.Application.Features.Seos.Shared;
using DesignPattern.Domain.Abstractions;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Extensions;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Commands.UpdateBrand;

public class UpdateBrandHandler : IRequestHandler<UpdateBrandCommand, Result<BrandResponse>>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ISlugChecker _slugChecker;

  public UpdateBrandHandler(IUnitOfWork unitOfWork, ISlugChecker slugChecker)
  {
    _unitOfWork = unitOfWork;
    _slugChecker = slugChecker;
  }

  public async Task<Result<BrandResponse>> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    var brand = await _unitOfWork.BrandRepository.GetByIdAsync(request.Id, cancellationToken);
    if (brand is null)
    {
      return Result<BrandResponse>.Failure(BrandErrors.BrandNotFound);
    }

    var updateResult = await GetParentBrandAsync(request.dto.ParentBrandId, cancellationToken)
      .BindAsync(async parentBrand =>
      {
        var brandName = dto.Name;
        var slugResult = brandName is not null ?
          await _slugChecker.GenerateUniqueSlugAsync(brandName, cancellationToken) :
            Result<string>.Success(null!);
        return slugResult.Bind(slug => UpdateBrand(brand, slug, parentBrand, dto));
      });

    var persistedResult = await updateResult.TapAsync(brand => PersistBrandAsync(brand, cancellationToken));

    return persistedResult.Map(BrandResponse.FromDomain);
  }

  private async Task<Result<Brand?>> GetParentBrandAsync(string? parentBrandId, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(parentBrandId))
    {
      return Result<Brand?>.Success(null);
    }

    var parsedParentBrandId = Guid.Parse(parentBrandId);

    var parentBrand = await _unitOfWork.BrandRepository.GetByIdAsync(parsedParentBrandId, cancellationToken);
    if (parentBrand is null)
    {
      return Result<Brand?>.Failure(BrandErrors.ParentBrandNotFound);
    }

    return Result<Brand?>.Success(parentBrand);
  }

  private static Result<Brand> UpdateBrand(Brand brand, string? slug, Brand? parentBrand, UpdateBrandDTO dto)
  {
    var updateResult = brand.Update(
      name: dto.Name,
      metaTitle: dto.Title,
      metaDescription: dto.MetaDescription,
      slug: slug
    );
    if (updateResult.IsFailure)
    {
      return Result<Brand>.Failure(updateResult.Error);
    }

    updateResult = brand.ChangeParentBrand(parentBrand);
    if (updateResult.IsFailure)
    {
      return Result<Brand>.Failure(updateResult.Error);
    }

    return Result<Brand>.Success(brand);
  }

  private async Task PersistBrandAsync(Brand brand, CancellationToken cancellationToken)
  {
    _unitOfWork.BrandRepository.Update(brand);
    await _unitOfWork.SaveChangesAsync(cancellationToken);
  }
}
