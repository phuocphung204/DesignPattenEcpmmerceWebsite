using FluentValidation;

namespace DesignPattern.Application.Features.Users.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
  public RegisterUserCommandValidator()
  {
    RuleFor(x => x.dto.FullName)
        .NotEmpty().WithMessage("Họ tên không được để trống.");

    RuleFor(x => x.dto.Email)
      .NotEmpty().WithMessage("Email không được để trống.")
      .EmailAddress().WithMessage("Email không hợp lệ.");

    // RuleFor(x => x.dto.PhoneNumber)
    //   .NotEmpty().WithMessage("Số điện thoại không được để trống.");

    RuleFor(x => x.dto.Password)
      .NotEmpty().WithMessage("Password không được để trống.")
      .MinimumLength(6).WithMessage("Password phải có ít nhất 6 ký tự.");

    RuleFor(x => x.dto.Address.ReceiverName)
      .NotEmpty().WithMessage("Người nhận không được để trống.");

    RuleFor(x => x.dto.Address.PhoneNumber)
      .NotEmpty().WithMessage("Số điện thoại người nhận không được để trống.");

    RuleFor(x => x.dto.Address.Country)
      .NotEmpty().WithMessage("Quốc gia không được để trống.");

    RuleFor(x => x.dto.Address.Province)
      .NotEmpty().WithMessage("Tỉnh/Thành phố không được để trống.");

    RuleFor(x => x.dto.Address.District)
      .NotEmpty().WithMessage("Quận/Huyện không được để trống.");

    RuleFor(x => x.dto.Address.Ward)
      .NotEmpty().WithMessage("Phường/Xã không được để trống.");

    RuleFor(x => x.dto.Address.Street)
      .NotEmpty().WithMessage("Số nhà, tên đường không được để trống.");
  }
}
