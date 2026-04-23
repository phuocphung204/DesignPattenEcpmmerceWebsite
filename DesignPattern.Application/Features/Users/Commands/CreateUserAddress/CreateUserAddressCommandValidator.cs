using FluentValidation;

namespace DesignPattern.Application.Features.Users.Commands.CreateUserAddress;

public sealed class CreateUserAddressCommandValidator : AbstractValidator<CreateUserAddressCommand>
{
  public CreateUserAddressCommandValidator()
  {
    RuleFor(x => x.UserId)
      .NotEmpty()
      .WithMessage("Id người dùng là bắt buộc.");

    RuleFor(x => x.dto.ReceiverName)
      .NotEmpty().WithMessage("Người nhận không được để trống.");

    RuleFor(x => x.dto.PhoneNumber)
      .NotEmpty().WithMessage("Số điện thoại người nhận không được để trống.");

    RuleFor(x => x.dto.Country)
      .NotEmpty().WithMessage("Quốc gia không được để trống.");

    RuleFor(x => x.dto.Province)
      .NotEmpty().WithMessage("Tỉnh/Thành phố không được để trống.");

    RuleFor(x => x.dto.District)
      .NotEmpty().WithMessage("Quận/Huyện không được để trống.");

    RuleFor(x => x.dto.Ward)
      .NotEmpty().WithMessage("Phường/Xã không được để trống.");

    RuleFor(x => x.dto.Street)
      .NotEmpty().WithMessage("Số nhà, tên đường không được để trống.");
  }
}
