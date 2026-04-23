using System.Globalization;
using DesignPattern.Application.Abstractions.Payments;
namespace DesignPattern.Infrastructure.Payments.Momo;

public class MomoAdapter : IPaymentGateway
{
  private readonly FakeMoMoService _momoService;

  public MomoAdapter(FakeMoMoService momoService)
  {
    _momoService = momoService;
  }

  public Task<CreatePaymentResponse> CreatePaymentAsync(CreatePaymentDTO request)
  {
    string paymentUrl = _momoService.CreatePaymentUrl(request);

    return Task.FromResult(new CreatePaymentResponse(paymentUrl, request.OrderId));
  }

  public PaymentResult ProcessCallback(object response)
  {
    var momoResponse = response as MoMoResponse
      ?? throw new ArgumentException("Invalid callback payload for MoMo", nameof(response));

    return new PaymentResult(
      OrderCode: momoResponse.orderId,
      Amount: momoResponse.amount,
      Provider: "MOMO",
      Channel: momoResponse.payType,
      TransactionId: momoResponse.transId.ToString(CultureInfo.InvariantCulture),
      PaidAt: DateTimeOffset.FromUnixTimeMilliseconds(momoResponse.responseTime).UtcDateTime,
      IsSuccess: momoResponse.resultCode == 0,
      ErrorCode: momoResponse.resultCode.ToString(CultureInfo.InvariantCulture),
      Message: momoResponse.message);
  }

}

public class MoMoResponse
{
  public Guid orderId { get; set; } = Guid.Empty;
  public string requestId { get; set; } = null!;
  public long amount { get; set; }
  public long transId { get; set; }
  public int resultCode { get; set; }
  public string message { get; set; } = null!;
  public string payType { get; set; } = null!;
  public long responseTime { get; set; }
}