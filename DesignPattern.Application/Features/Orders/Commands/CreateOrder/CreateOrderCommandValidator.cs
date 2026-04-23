using DesignPattern.Domain.Enums;
using FluentValidation;

namespace DesignPattern.Application.Features.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
  public CreateOrderCommandValidator()
  {
    RuleFor(x => x.dto.Items)
      .NotNull().WithMessage("Danh sách sản phẩm là bắt buộc.")
      .NotEmpty().WithMessage("Đơn hàng phải có ít nhất một sản phẩm.");

    RuleForEach(x => x.dto.Items)
      .SetValidator(new OrderItemDtoValidator());

    RuleFor(x => x.dto.PointsUsed)
      .GreaterThanOrEqualTo(0)
      .WithMessage("Số điểm sử dụng phải lớn hơn hoặc bằng 0.");

    RuleFor(x => x.dto.ShippingInfo)
      .NotNull()
      .WithMessage("Thông tin giao hàng là bắt buộc.");

    RuleFor(x => x.dto.ShippingInfo)
      .SetValidator(new ShippingInfoDtoValidator())
      .When(x => x.dto.ShippingInfo is not null);

    RuleFor(x => x.dto.PaymentInfo)
      .NotNull()
      .WithMessage("Thông tin thanh toán là bắt buộc.");

    RuleFor(x => x.dto.PaymentInfo)
      .SetValidator(new PaymentInfoDtoValidator())
      .When(x => x.dto.PaymentInfo is not null);
  }
}

public sealed class ShippingInfoDtoValidator : AbstractValidator<ShippingInfoDto>
{
  public ShippingInfoDtoValidator()
  {
    RuleFor(x => x.shippingAddressId)
      .NotEmpty()
      .WithMessage("shippingAddressId là bắt buộc.");
  }
}

public sealed class PaymentInfoDtoValidator : AbstractValidator<PaymentInfoDto>
{
  public PaymentInfoDtoValidator()
  {
    RuleFor(x => x.Type)
      .IsInEnum()
      .WithMessage("Loại thanh toán không hợp lệ.");

    When(x => x.Type == PaymentType.CreditCard, () =>
    {
      RuleFor(x => x.CardNumber)
        .NotEmpty()
        .WithMessage("Số thẻ là bắt buộc khi thanh toán bằng thẻ.");

      RuleFor(x => x.CardHolder)
        .NotEmpty()
        .WithMessage("Tên chủ thẻ là bắt buộc khi thanh toán bằng thẻ.");

      RuleFor(x => x.CardType)
        .NotEmpty()
        .WithMessage("Loại thẻ là bắt buộc khi thanh toán bằng thẻ.");
    });

    When(x => x.Type == PaymentType.BankTransfer, () =>
    {
      RuleFor(x => x.BankName)
        .NotEmpty()
        .WithMessage("Tên ngân hàng là bắt buộc khi chuyển khoản.");

      RuleFor(x => x.AccountNumber)
        .NotEmpty()
        .WithMessage("Số tài khoản là bắt buộc khi chuyển khoản.");
    });
  }
}

public sealed class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
{
  public OrderItemDtoValidator()
  {
    RuleFor(x => x.ProductId)
      .NotEmpty()
      .WithMessage("ProductId là bắt buộc.");

    RuleFor(x => x.RequiredQuantity)
      .GreaterThan(0)
      .WithMessage("Số lượng sản phẩm phải lớn hơn 0.");
  }
}
