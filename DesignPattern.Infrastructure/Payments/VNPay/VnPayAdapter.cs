using System.Globalization;
using DesignPattern.Application.Abstractions.Payments;

namespace DesignPattern.Infrastructure.Payments.VNPay;

public class VnPayAdapter : IPaymentGateway
{
  private readonly FakeVnPayService _vnPayService;

  public VnPayAdapter(FakeVnPayService vnPayService)
  {
    _vnPayService = vnPayService;
  }

  public Task<CreatePaymentResponse> CreatePaymentAsync(CreatePaymentDTO request)
  {
    string paymentUrl = _vnPayService.CreatePaymentUrl(request);

    return Task.FromResult(new CreatePaymentResponse(paymentUrl, request.OrderId));
  }

  public PaymentResult ProcessCallback(object callbackData)
  {
    var vnPayResponse = callbackData as VnPayResponse
      ?? throw new ArgumentException("Invalid callback payload for VNPay", nameof(callbackData));

    return new PaymentResult(
      OrderCode: vnPayResponse.vnp_TxnRef,
      Amount: vnPayResponse.vnp_Amount / 100m,
      Provider: "VNPAY",
      Channel: vnPayResponse.vnp_BankCode,
      TransactionId: vnPayResponse.vnp_TransactionNo,
      PaidAt: DateTime.ParseExact(
        vnPayResponse.vnp_PayDate,
        "yyyyMMddHHmmss",
        CultureInfo.InvariantCulture),
      IsSuccess: vnPayResponse.vnp_ResponseCode == "00",
      ErrorCode: vnPayResponse.vnp_ResponseCode,
      Message: vnPayResponse.vnp_OrderInfo);

  }
}

public class VnPayResponse
{
  public Guid vnp_TxnRef { get; set; } = Guid.Empty;
  public long vnp_Amount { get; set; }
  public string vnp_TransactionNo { get; set; } = null!;
  public string vnp_ResponseCode { get; set; } = null!;
  public string vnp_BankCode { get; set; } = null!;
  public string vnp_PayDate { get; set; } = null!;
  public string vnp_OrderInfo { get; set; } = null!;
}