using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using MediatR;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Application.Features.DiscountCodes.Shared;
using DesignPattern.Domain.Entities.DiscountCodes;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Features.DiscountCodes.Commands.CreateDiscountCode;

public class CreateDiscountCodeCommandHandler : IRequestHandler<CreateDiscountCodeCommand, Result<DiscountCodeResponse>>
{
  private readonly IUnitOfWork _unitOfWork;

  public CreateDiscountCodeCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<DiscountCodeResponse>> Handle(CreateDiscountCodeCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    // Check if code already exists
    var tempCodeResult = Code.Create(dto.Code);

    if (tempCodeResult.IsSuccess)
    {
      var exists = await _unitOfWork.DiscountCodeRepository.GetByCodeAsync(tempCodeResult.Value.Value, cancellationToken);
      if (exists is not null)
      {
        return Error.Conflict(
          "DiscountCode.DuplicateCode",
          "A discount code with this code already exists."
        );
      }
    }

    var discountCodeResult = CreateDiscountCode(dto);
    if (discountCodeResult.IsFailure)
    {
      return discountCodeResult.Error;
    }

    var discountCode = discountCodeResult.Value;
    _unitOfWork.DiscountCodeRepository.Create(discountCode);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return DiscountCodeResponse.FromDomain(discountCode);
  }

  private static Result<DiscountCode> CreateDiscountCode(CreateDiscountCodeDTO dto)
  {

    Result<DiscountCode> result;

    switch (dto.Type)
    {
      case DiscountType.FixedAmount:
        {
          if (dto.FixedAmount is not decimal fixedAmount)
            return Result<DiscountCode>.Failure(new Error("FixedAmount.Invalid", "Fixed amount must not be null."));

          result = DiscountCodeFactory.Create(new FixedAmountDiscountData(
              dto.Code,
              dto.MinimumOrderAmount,
              dto.UsageLimit,
              dto.ExpirationDate,
              fixedAmount));

          break;
        }

      case DiscountType.Percentage:
        {
          if (dto.Percent is not decimal percent)
            return Result<DiscountCode>.Failure(new Error("Percent.Invalid", "Percent must not be null."));

          if (dto.MaximumDiscountAmount is not decimal max)
            return Result<DiscountCode>.Failure(new Error("MaximumDiscountAmount.Invalid", "Max discount must not be null."));

          result = DiscountCodeFactory.Create(new PercentageDiscountData(
              dto.Code,
              dto.MinimumOrderAmount,
              dto.UsageLimit,
              dto.ExpirationDate,
              percent,
              max));

          break;
        }

      default:
        return Result<DiscountCode>.Failure(new Error("DiscountType.Invalid", "Invalid discount type."));
    }

    if (result.IsFailure)
      return result;

    return Result<DiscountCode>.Success(result.Value);
  }
}
