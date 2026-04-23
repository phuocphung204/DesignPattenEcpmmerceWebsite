using FluentValidation;

namespace DesignPattern.Application.Features.Users.Commands.UpdateUserProfile;

public sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
  public UpdateUserProfileCommandValidator()
  {
    RuleFor(x => x.UserId)
      .NotEmpty()
      .WithMessage("Id người dùng là bắt buộc.");

    When(x => x.dto is not null, () =>
    {
      RuleFor(x => x.dto.FullName)
        .NotEmpty()
        .WithMessage("Họ tên không được để trống.")
        .MaximumLength(200)
        .WithMessage("Họ tên không được vượt quá 200 ký tự.")
        .When(x => x.dto.FullName is not null);

      // RuleFor(x => x.dto.PhoneNumber)
      //   .NotEmpty()
      //   .WithMessage("Số điện thoại không được để trống.")
      //   .When(x => x.dto.PhoneNumber is not null);

      RuleFor(x => x.dto.AvatarLink)
        .NotEmpty()
        .WithMessage("Link ảnh đại diện không được để trống.")
        .MaximumLength(2048)
        .WithMessage("Link ảnh đại diện không được vượt quá 2048 ký tự.")
        .When(x => x.dto.AvatarLink is not null);
    });
  }
}
