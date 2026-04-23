using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Abstractions.Payments;

public interface IPaymentGateway
{
  Task<CreatePaymentResponse> CreatePaymentAsync(CreatePaymentDTO request);

  PaymentResult ProcessCallback(object callbackData);
}

public record CreatePaymentDTO(
  Guid OrderId, // ID của đơn hàng cần thanh toán
  decimal Amount, // Số tiền cần thanh toán
  string Currency, // Loại tiền tệ (ví dụ: "USD", "VND")
  PaymentMethod PaymentMethod, // Phương thức thanh toán (ví dụ: "VnPay", "MoMo")
  string ReturnUrl); // URL mà khách hàng sẽ được chuyển đến sau khi hoàn thành thanh toán (thường là trang xác nhận đơn hàng hoặc trang chi tiết đơn hàng)

public record CreatePaymentResponse(
  string PaymentUrl,
  Guid OrderId);