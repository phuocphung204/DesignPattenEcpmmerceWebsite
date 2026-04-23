
using DesignPattern.Application.Abstractions.Payments;

namespace DesignPattern.Infrastructure.Payments.Momo;

public class FakeMoMoService
{
  public string CreatePaymentUrl(CreatePaymentDTO request)
  {
    // TODO: frontend tạo API fake để thanh toán - được thì tạo trang thanh toán giả lập
    // Sau khi thanh toán xong, mình tự gọi callback tới URL bạn đã cấu hình, ví dụ:
    // Post https://localhost:5001/api/payment/momo-return
    // Body:
    // {
    //   "orderId": "ORDER001",
    //   "amount": 100000,
    //   "currency": "VND",
    //   "paymentMethod": "MoMo",
    //   "returnUrl": "https://localhost:5001/order/ORDER001",
    //   "transactionId": "123456",
    //   "responseCode": "00"
    // }
    return $"http://localhost:5074/api/pay-momo?orderId={request.OrderId}&amount={request.Amount}&currency={request.Currency}&paymentMethod={request.PaymentMethod}&returnUrl={request.ReturnUrl}";
  }
}
