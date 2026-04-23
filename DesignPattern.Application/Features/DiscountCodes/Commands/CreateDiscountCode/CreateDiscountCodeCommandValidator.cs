using DesignPattern.Domain.Entities.DiscountCodes;
using DesignPattern.Domain.Enums;
using FluentValidation;

namespace DesignPattern.Application.Features.DiscountCodes.Commands.CreateDiscountCode;

public sealed class CreateDiscountCodeCommandValidator : AbstractValidator<CreateDiscountCodeCommand>
{
  public CreateDiscountCodeCommandValidator()
  {
    RuleFor(x => x.dto)
      .NotNull()
      .WithMessage("Thông tin mã giảm giá là bắt buộc.");

    RuleFor(x => x.dto)
      .SetValidator(new CreateDiscountCodeDtoValidator())
      .When(x => x.dto is not null);
  }
}

public sealed class CreateDiscountCodeDtoValidator : AbstractValidator<CreateDiscountCodeDTO>
{
  public CreateDiscountCodeDtoValidator()
  {
    RuleFor(x => x.Code)
      .NotEmpty().WithMessage("Code là bắt buộc.")
      .Length(5).WithMessage("Code phải có đúng 5 ký tự.")
      .Matches("^[a-zA-Z0-9]+$").WithMessage("Code chỉ được chứa chữ hoặc số.");

    RuleFor(x => x.MinimumOrderAmount)
      .GreaterThanOrEqualTo(0)
      .WithMessage("Giá trị đơn hàng tối thiểu phải lớn hơn hoặc bằng 0.");

    RuleFor(x => x.UsageLimit)
      .GreaterThan(0)
      .WithMessage("Giới hạn sử dụng phải lớn hơn 0.");

    RuleFor(x => x.ExpirationDate)
      .Must(expirationDate => expirationDate >= DateTime.UtcNow.AddHours(DiscountCode.MINIMUM_EXPIRATION_TIME))
      .WithMessage($"Ngày hết hạn phải cách thời điểm hiện tại ít nhất {DiscountCode.MINIMUM_EXPIRATION_TIME} giờ.");

    RuleFor(x => x.Type)
      .IsInEnum()
      .WithMessage("Loại mã giảm giá không hợp lệ.");

    When(x => x.Type == DiscountType.FixedAmount, () =>
    {
      RuleFor(x => x.FixedAmount)
        .NotNull().WithMessage("FixedAmount là bắt buộc với loại giảm giá cố định.")
        .GreaterThanOrEqualTo(0).WithMessage("FixedAmount phải lớn hơn hoặc bằng 0.");
    });

    When(x => x.Type == DiscountType.Percentage, () =>
    {
      RuleFor(x => x.Percent)
        .NotNull().WithMessage("Percent là bắt buộc với loại giảm giá theo phần trăm.")
        .InclusiveBetween(0, 100).WithMessage("Percent phải nằm trong khoảng từ 0 đến 100.");

      RuleFor(x => x.MaximumDiscountAmount)
        .NotNull().WithMessage("MaximumDiscountAmount là bắt buộc với loại giảm giá theo phần trăm.")
        .GreaterThanOrEqualTo(0).WithMessage("MaximumDiscountAmount phải lớn hơn hoặc bằng 0.");
    });
  }
}
