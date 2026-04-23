using DesignPattern.Application.Features.Seos.Shared;
using DesignPattern.Domain.Abstractions;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Extensions;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.Seos.Commands.CreateBrand;

public class CreateBrandHandler : IRequestHandler<CreateBrandCommand, Result<BrandResponse>>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ISlugChecker _slugChecker;

  public CreateBrandHandler(IUnitOfWork unitOfWork, ISlugChecker slugChecker)
  {
    _unitOfWork = unitOfWork;
    _slugChecker = slugChecker;
  }

  public async Task<Result<BrandResponse>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;

    var generateSlugTask = _slugChecker.GenerateUniqueSlugAsync(dto.Name, cancellationToken);
    var parentBrandTask = GetParentBrandAsync(dto.ParentBrandId, cancellationToken);
    await Task.WhenAll(generateSlugTask, parentBrandTask);

    var slugResult = await generateSlugTask;
    var parentBrandResult = await parentBrandTask;

    var createResult = slugResult
      .Bind(slug => parentBrandResult
        .Bind(parentBrand => CreateBrand(dto, slug, parentBrand)));

    var persistedResult = await createResult
      .TapAsync(brand => PersistBrandAsync(brand, cancellationToken));

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

  private static Result<Brand> CreateBrand(CreateBrandDTO dto, string slug, Brand? parentBrand)
  {
    return Brand.Create(
        name: dto.Name,
        level: parentBrand?.Level?.Value + 1 ?? 0,
        metaTitle: dto.Title,
        slug: slug,
        metaDescription: dto.MetaDescription,
        parentBrand: parentBrand);
  }

  private async Task PersistBrandAsync(Brand brand, CancellationToken cancellationToken)
  {
    _unitOfWork.BrandRepository.Create(brand);
    await _unitOfWork.SaveChangesAsync(cancellationToken);
  }
}
