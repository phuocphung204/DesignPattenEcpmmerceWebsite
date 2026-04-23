using FluentValidation;

namespace DesignPattern.Application.Features.WarehouseItems.Commands.UpdateItem;

public sealed class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
{
  public UpdateItemCommandValidator()
  {
    RuleFor(x => x.Id)
      .NotEmpty()
      .WithMessage("Id warehouse item là bắt buộc.");

    RuleFor(x => x.dto)
      .NotNull()
      .WithMessage("Thông tin cập nhật warehouse item là bắt buộc.");

    RuleFor(x => x.dto)
      .Must(dto => dto is not null && HasAnyUpdate(dto))
      .WithMessage("Phải có ít nhất một trường để cập nhật.");

    RuleFor(x => x.dto)
      .SetValidator(new UpdateItemDtoValidator())
      .When(x => x.dto is not null);
  }

  private static bool HasAnyUpdate(UpdateItemDTO dto)
  {
    return dto.Name is not null
      || dto.Sku is not null
      || dto.VariantGroupId is not null
      || dto.Quantity is not null;
  }
}

public sealed class UpdateItemDtoValidator : AbstractValidator<UpdateItemDTO>
{
  public UpdateItemDtoValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty().WithMessage("Tên sản phẩm không được để trống.")
      .MaximumLength(100).WithMessage("Tên sản phẩm không được vượt quá 100 ký tự.")
      .When(x => x.Name is not null);

    RuleFor(x => x.Sku)
      .NotEmpty().WithMessage("Sku không được để trống.")
      .MaximumLength(64).WithMessage("Sku không được vượt quá 64 ký tự.")
      .When(x => x.Sku is not null);

    RuleFor(x => x.VariantGroupId)
      .NotEmpty().WithMessage("VariantGroupId không được để trống.")
      .MaximumLength(64).WithMessage("VariantGroupId không được vượt quá 64 ký tự.")
      .When(x => x.VariantGroupId is not null);

    RuleFor(x => x.Quantity)
      .GreaterThan(0)
      .WithMessage("Số lượng cập nhật phải lớn hơn 0.")
      .When(x => x.Quantity.HasValue);
  }
}
