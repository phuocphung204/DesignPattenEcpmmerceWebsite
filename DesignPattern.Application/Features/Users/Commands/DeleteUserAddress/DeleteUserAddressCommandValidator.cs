using FluentValidation;

namespace DesignPattern.Application.Features.Users.Commands.DeleteUserAddress;

public sealed class DeleteUserAddressCommandValidator : AbstractValidator<DeleteUserAddressCommand>
{
  public DeleteUserAddressCommandValidator()
  {
    RuleFor(x => x.UserId)
      .NotEmpty()
      .WithMessage("Id người dùng là bắt buộc.");

    RuleFor(x => x.AddressId)
      .NotEmpty()
      .WithMessage("Id địa chỉ là bắt buộc.");
  }
}
