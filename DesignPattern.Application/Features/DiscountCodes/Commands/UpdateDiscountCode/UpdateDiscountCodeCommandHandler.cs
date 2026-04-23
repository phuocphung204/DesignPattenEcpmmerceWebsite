using DesignPattern.Application.Features.DiscountCodes.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.DiscountCodes.Commands.UpdateDiscountCode;

public class UpdateDiscountCodeCommandHandler : IRequestHandler<UpdateDiscountCodeCommand, Result<DiscountCodeResponse>>
{
  private readonly IUnitOfWork _unitOfWork;

  public UpdateDiscountCodeCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<DiscountCodeResponse>> Handle(UpdateDiscountCodeCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    var discountCode = await _unitOfWork.DiscountCodeRepository.GetByIdAsync(request.Id, cancellationToken);
    if (discountCode is null)
      return DiscountCodeErrors.NotFound;

    var result = discountCode.Update(dto.ExpirationDate, dto.IsActive, dto.UsageLimit,
                  dto.MinimumOrderAmount, dto.FixedAmount, dto.Percent, dto.MaximumDiscountAmount);
    if (result.IsFailure)
      return result.Error;

    _unitOfWork.DiscountCodeRepository.Update(discountCode);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return DiscountCodeResponse.FromDomain(discountCode);
  }
}
