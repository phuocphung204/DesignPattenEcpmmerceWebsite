using DesignPattern.Application.Abstractions.Payments;

namespace DesignPattern.Infrastructure.Payments.VNPay;

public class FakeVnPayService
{
  public string CreatePaymentUrl(CreatePaymentDTO request)
  {
    // TODO: frontend tạo API fake để thanh toán - được thì tạo trang thanh toán giả lập
    // Sau khi thanh toán xong, mình tự gọi callback tới URL bạn đã cấu hình, ví dụ:
    // Post https://localhost:5001/api/payment/vnpay-return
    // Body:
    // {
    //   "orderId": "ORDER001",
    //   "amount": 100000,
    //   "currency": "VND",
    //   "paymentMethod": "VnPay",
    //   "returnUrl": "https://localhost:5001/order/ORDER001",
    //   "transactionId": "123456",
    //   "responseCode": "00"
    // }
    return $"http://localhost:5074/api/pay-vnpay?orderId={request.OrderId}&amount={request.Amount}&currency={request.Currency}&paymentMethod={request.PaymentMethod}&returnUrl={request.ReturnUrl}";
  }
}