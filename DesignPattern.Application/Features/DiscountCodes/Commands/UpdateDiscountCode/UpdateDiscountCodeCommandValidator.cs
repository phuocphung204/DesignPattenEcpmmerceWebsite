using DesignPattern.Domain.Entities.DiscountCodes;
using FluentValidation;

namespace DesignPattern.Application.Features.DiscountCodes.Commands.UpdateDiscountCode;

public sealed class UpdateDiscountCodeCommandValidator : AbstractValidator<UpdateDiscountCodeCommand>
{
  public UpdateDiscountCodeCommandValidator()
  {
    RuleFor(x => x.Id)
      .NotEmpty()
      .WithMessage("Id mã giảm giá là bắt buộc.");

    RuleFor(x => x.dto)
      .NotNull()
      .WithMessage("Thông tin cập nhật mã giảm giá là bắt buộc.");

    RuleFor(x => x.dto)
      .SetValidator(new UpdateDiscountCodeDtoValidator())
      .When(x => x.dto is not null);
  }
}

public sealed class UpdateDiscountCodeDtoValidator : AbstractValidator<UpdateDiscountCodeDto>
{
  public UpdateDiscountCodeDtoValidator()
  {
    RuleFor(x => x.ExpirationDate)
      .Must(expirationDate => expirationDate is null || expirationDate.Value >= DateTime.UtcNow.AddHours(DiscountCode.MINIMUM_EXPIRATION_TIME))
      .WithMessage($"Ngày hết hạn phải cách thời điểm hiện tại ít nhất {DiscountCode.MINIMUM_EXPIRATION_TIME} giờ.");

    RuleFor(x => x.UsageLimit)
      .GreaterThan(0)
      .When(x => x.UsageLimit.HasValue)
      .WithMessage("Giới hạn sử dụng phải lớn hơn 0.");

    RuleFor(x => x.MinimumOrderAmount)
      .GreaterThanOrEqualTo(0)
      .WithMessage("Giá trị đơn hàng tối thiểu phải lớn hơn hoặc bằng 0.");

    RuleFor(x => x.FixedAmount)
      .GreaterThanOrEqualTo(0)
      .When(x => x.FixedAmount.HasValue)
      .WithMessage("FixedAmount phải lớn hơn hoặc bằng 0.");

    RuleFor(x => x.Percent)
      .InclusiveBetween(0, 100)
      .When(x => x.Percent.HasValue)
      .WithMessage("Percent phải nằm trong khoảng từ 0 đến 100.");

    RuleFor(x => x.MaximumDiscountAmount)
      .GreaterThanOrEqualTo(0)
      .When(x => x.MaximumDiscountAmount.HasValue)
      .WithMessage("MaximumDiscountAmount phải lớn hơn hoặc bằng 0.");
  }
}
