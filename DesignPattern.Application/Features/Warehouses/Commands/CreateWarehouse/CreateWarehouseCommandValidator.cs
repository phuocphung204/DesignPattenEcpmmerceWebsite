using FluentValidation;

namespace DesignPattern.Application.Features.Warehouses.Commands.CreateWarehouse;

public sealed class CreateWarehouseCommandValidator : AbstractValidator<CreateWarehouseCommand>
{
  public CreateWarehouseCommandValidator()
  {
    RuleFor(x => x.dto)
      .NotNull()
      .WithMessage("Thông tin kho là bắt buộc.");

    RuleFor(x => x.dto)
      .SetValidator(new CreateWarehouseDtoValidator())
      .When(x => x.dto is not null);
  }
}

public sealed class CreateWarehouseDtoValidator : AbstractValidator<CreateWarehouseDTO>
{
  public CreateWarehouseDtoValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty().WithMessage("Tên kho là bắt buộc.")
      .MaximumLength(100).WithMessage("Tên kho không được vượt quá 100 ký tự.");

    RuleFor(x => x.Address)
      .NotEmpty().WithMessage("Địa chỉ kho là bắt buộc.")
      .MaximumLength(255).WithMessage("Địa chỉ kho không được vượt quá 255 ký tự.");
  }
}
