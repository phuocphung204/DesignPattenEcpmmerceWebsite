using FluentValidation;

namespace DesignPattern.Application.Features.WarehouseItems.Commands.CreateItem;

public sealed class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
{
  public CreateItemCommandValidator()
  {
    RuleFor(x => x.dto)
      .NotNull()
      .WithMessage("Thông tin warehouse item là bắt buộc.");

    RuleFor(x => x.dto)
      .SetValidator(new CreateItemDtoValidator())
      .When(x => x.dto is not null);
  }
}

public sealed class CreateItemDtoValidator : AbstractValidator<CreateItemDTO>
{
  public CreateItemDtoValidator()
  {
    RuleFor(x => x.WarehouseId)
      .NotEmpty()
      .WithMessage("WarehouseId là bắt buộc.");

    RuleFor(x => x.WarehouseName)
      .NotEmpty().WithMessage("Tên kho là bắt buộc.")
      .MaximumLength(100).WithMessage("Tên kho không được vượt quá 100 ký tự.");

    RuleFor(x => x.ProductId)
      .NotEmpty()
      .WithMessage("ProductId là bắt buộc.");

    RuleFor(x => x.ProductName)
      .NotEmpty().WithMessage("Tên sản phẩm là bắt buộc.")
      .MaximumLength(100).WithMessage("Tên sản phẩm không được vượt quá 100 ký tự.");

    RuleFor(x => x.Sku)
      .NotEmpty().WithMessage("Sku là bắt buộc.")
      .MaximumLength(64).WithMessage("Sku không được vượt quá 64 ký tự.");

    RuleFor(x => x.VariantGroupId)
      .NotEmpty().WithMessage("VariantGroupId là bắt buộc.")
      .MaximumLength(64).WithMessage("VariantGroupId không được vượt quá 64 ký tự.");

    RuleFor(x => x.Quantity)
      .GreaterThan(0)
      .WithMessage("Số lượng phải lớn hơn 0.");
  }
}
