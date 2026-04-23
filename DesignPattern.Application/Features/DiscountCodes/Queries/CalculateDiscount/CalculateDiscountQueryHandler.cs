using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.DiscountCodes;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.ValueObjects;
using MediatR;

namespace DesignPattern.Application.Features.DiscountCodes.Queries.CalculateDiscount;

public class CalculateDiscountQueryHandler : IRequestHandler<CalculateDiscountQuery, Result<CalculateDiscountResponse>>
{
  private readonly IUnitOfWork _unitOfWork;

  public CalculateDiscountQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<CalculateDiscountResponse>> Handle(CalculateDiscountQuery request, CancellationToken cancellationToken)
  {
    var priceResult = Price.Create(request.Dto.SubTotal);
    if (priceResult.IsFailure)
      return priceResult.Error;

    var subTotal = priceResult.Value;

    var discountCode = await _unitOfWork.DiscountCodeRepository.GetByCodeAsync(request.Dto.Code, cancellationToken);
    if (discountCode is null)
      return DiscountCodeErrors.NotFound;

    var discountAmountResult = discountCode.CheckAppliableDiscount(subTotal);
    if (discountAmountResult.IsFailure)
      return discountAmountResult.Error;

    var discountAmount = discountAmountResult.Value;

    // Note: To calculate final amount, we create a new Price or just do decimal math.
    // Since Price overloads operator - :
    var finalAmountPrice = subTotal - discountAmount;

    decimal? percent = null;
    decimal? amount = null;
    if (discountCode is PercentageDiscountCode percentageDiscount)
    {
      percent = percentageDiscount.Percent.Value;
    }
    else if (discountCode is FixedDiscountCode fixedDiscount)
    {
      amount = fixedDiscount.FixedAmount.Amount;
    }

    var response = new CalculateDiscountResponse
    {
      OriginalAmount = subTotal.Amount,
      DiscountAmount = discountAmount.Amount,
      FinalAmount = finalAmountPrice.Amount,
      DiscountType = discountCode.Type.ToString(),
      Percent = discountCode.Type == DiscountType.Percentage ? ((PercentageDiscountCode)discountCode).Percent?.Value : null,
      Amount = discountCode.Type == DiscountType.FixedAmount ? ((FixedDiscountCode)discountCode).FixedAmount?.Amount : null
    };

    return response;
  }
}
