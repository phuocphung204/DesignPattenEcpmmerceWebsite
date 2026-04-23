using FluentValidation;

namespace DesignPattern.Application.Features.Carts.Commands.UpdateCartItemQuantity;

public sealed class UpdateCartItemQuantityCommandValidator : AbstractValidator<UpdateCartItemQuantityCommand>
{
  public UpdateCartItemQuantityCommandValidator()
  {
    RuleFor(x => x.ProductId)
      .NotEmpty()
      .WithMessage("ProductId là bắt buộc.");

    RuleFor(x => x.dto)
      .NotNull()
      .WithMessage("Thông tin cập nhật số lượng là bắt buộc.");

    RuleFor(x => x.dto)
      .SetValidator(new UpdateQuantityDtoValidator())
      .When(x => x.dto is not null);
  }
}

public sealed class UpdateQuantityDtoValidator : AbstractValidator<UpdateQuantityDto>
{
  public UpdateQuantityDtoValidator()
  {
    RuleFor(x => x.NewQuantity)
      .GreaterThan(0)
      .WithMessage("Số lượng mới phải lớn hơn 0.");
  }
}
