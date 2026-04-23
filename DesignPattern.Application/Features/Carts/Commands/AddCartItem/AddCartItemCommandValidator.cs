using FluentValidation;

namespace DesignPattern.Application.Features.Carts.Commands.AddCartItem;

public sealed class AddCartItemCommandValidator : AbstractValidator<AddCartItemCommand>
{
  public AddCartItemCommandValidator()
  {
    RuleFor(x => x.dto)
      .NotNull()
      .WithMessage("Thông tin sản phẩm thêm vào giỏ hàng là bắt buộc.");

    RuleFor(x => x.dto)
      .SetValidator(new AddCartItemDtoValidator())
      .When(x => x.dto is not null);
  }
}

public sealed class AddCartItemDtoValidator : AbstractValidator<AddCartItemDto>
{
  public AddCartItemDtoValidator()
  {
    RuleFor(x => x.ProductId)
      .NotEmpty()
      .WithMessage("ProductId là bắt buộc.");

    RuleFor(x => x.Quantity)
      .GreaterThan(0)
      .WithMessage("Số lượng sản phẩm phải lớn hơn 0.");
  }
}
