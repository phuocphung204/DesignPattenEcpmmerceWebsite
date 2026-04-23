using FluentValidation;

namespace DesignPattern.Application.Features.Users.Commands.UpdateUserAddress;

public sealed class UpdateUserAddressCommandValidator : AbstractValidator<UpdateUserAddressCommand>
{
  public UpdateUserAddressCommandValidator()
  {
    RuleFor(x => x.UserId)
      .NotEmpty()
      .WithMessage("Id người dùng là bắt buộc.");

    // RuleFor(x => x.AddressId)
    //   .NotEmpty()
    //   .WithMessage("Id địa chỉ là bắt buộc.");

    RuleFor(x => x.dto)
      .Must(dto => dto is not null && HasAnyUpdate(dto))
      .WithMessage("Phải có ít nhất một trường để cập nhật.");

    When(x => x.dto is not null, () =>
    {
      RuleFor(x => x.dto.ReceiverName)
        .NotEmpty().WithMessage("Người nhận không được để trống.")
        .When(x => x.dto.ReceiverName is not null);

      RuleFor(x => x.dto.PhoneNumber)
        .NotEmpty().WithMessage("Số điện thoại người nhận không được để trống.")
        .When(x => x.dto.PhoneNumber is not null);

      RuleFor(x => x.dto.Country)
        .NotEmpty().WithMessage("Quốc gia không được để trống.")
        .When(x => x.dto.Country is not null);

      RuleFor(x => x.dto.Province)
        .NotEmpty().WithMessage("Tỉnh/Thành phố không được để trống.")
        .When(x => x.dto.Province is not null);

      RuleFor(x => x.dto.District)
        .NotEmpty().WithMessage("Quận/Huyện không được để trống.")
        .When(x => x.dto.District is not null);

      RuleFor(x => x.dto.Ward)
        .NotEmpty().WithMessage("Phường/Xã không được để trống.")
        .When(x => x.dto.Ward is not null);

      RuleFor(x => x.dto.Street)
        .NotEmpty().WithMessage("Số nhà, tên đường không được để trống.")
        .When(x => x.dto.Street is not null);
    });
  }

  private static bool HasAnyUpdate(UpdateUserAddressDTO dto)
  {
    return dto.ReceiverName is not null
      || dto.PhoneNumber is not null
      || dto.Country is not null
      || dto.Province is not null
      || dto.District is not null
      || dto.Ward is not null
      || dto.Street is not null
      || dto.ProvinceCode is not null
      || dto.DistrictCode is not null
      || dto.WardCode is not null;
  }
}
