using FluentValidation;

namespace DesignPattern.Application.Features.WarehouseItems.Commands.ProccessAllocation;

public sealed class ProcessAllocationCommandValidator : AbstractValidator<ProcessAllocationCommand>
{
  public ProcessAllocationCommandValidator()
  {
    RuleFor(x => x.orderId)
      .NotEmpty()
      .WithMessage("OrderId là bắt buộc.");

    RuleFor(x => x.dtos)
      .NotNull().WithMessage("Danh sách sản phẩm phân bổ là bắt buộc.")
      .NotEmpty().WithMessage("Danh sách sản phẩm phân bổ không được rỗng.");

    RuleForEach(x => x.dtos)
      .SetValidator(new ProccessAllocationDtoValidator())
      .When(x => x.dtos is not null);
  }
}

public sealed class ProccessAllocationDtoValidator : AbstractValidator<ProccessAllocationDTO>
{
  public ProccessAllocationDtoValidator()
  {
    RuleFor(x => x.ProductId)
      .NotEmpty()
      .WithMessage("ProductId là bắt buộc.");

    RuleFor(x => x.ProductName)
      .NotEmpty().WithMessage("Tên sản phẩm là bắt buộc.")
      .MaximumLength(100).WithMessage("Tên sản phẩm không được vượt quá 100 ký tự.");

    RuleFor(x => x.RequestedQuantity)
      .GreaterThan(0)
      .WithMessage("Số lượng yêu cầu phải lớn hơn 0.");
  }
}
