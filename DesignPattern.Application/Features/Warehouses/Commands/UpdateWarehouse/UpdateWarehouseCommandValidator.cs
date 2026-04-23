using FluentValidation;

namespace DesignPattern.Application.Features.Warehouses.Commands.UpdateWarehouse;

public sealed class UpdateWarehouseCommandValidator : AbstractValidator<UpdateWarehouseCommand>
{
  public UpdateWarehouseCommandValidator()
  {
    RuleFor(x => x.Id)
      .NotEmpty()
      .WithMessage("Id kho là bắt buộc.");

    RuleFor(x => x.dto)
      .NotNull()
      .WithMessage("Thông tin cập nhật kho là bắt buộc.");

    RuleFor(x => x.dto)
      .SetValidator(new UpdateWarehouseDtoValidator())
      .When(x => x.dto is not null);
  }
}

public sealed class UpdateWarehouseDtoValidator : AbstractValidator<UpdateWarehouseDTO>
{
  public UpdateWarehouseDtoValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty().WithMessage("Tên kho là bắt buộc.")
      .MaximumLength(100).WithMessage("Tên kho không được vượt quá 100 ký tự.");

    RuleFor(x => x.Address)
      .NotEmpty().WithMessage("Địa chỉ kho là bắt buộc.")
      .MaximumLength(255).WithMessage("Địa chỉ kho không được vượt quá 255 ký tự.");
  }
}
