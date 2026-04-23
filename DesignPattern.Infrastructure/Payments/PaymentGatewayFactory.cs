namespace DesignPattern.Infrastructure.Payments;

using DesignPattern.Application.Abstractions.Payments;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Errors;
using DesignPattern.Infrastructure.Payments.Momo;
using DesignPattern.Infrastructure.Payments.VNPay;

public sealed class PaymentGatewayFactory : IPaymentGatewayFactory
{
  private readonly VnPayAdapter _vnPayAdapter;
  private readonly MomoAdapter _momoAdapter;

  public PaymentGatewayFactory(VnPayAdapter vnPayAdapter, MomoAdapter momoAdapter)
  {
    _vnPayAdapter = vnPayAdapter;
    _momoAdapter = momoAdapter;
  }

  public Result<IPaymentGateway> Create(PaymentMethod paymentMethod)
  {
    return paymentMethod switch
    {
      PaymentMethod.VnPay => _vnPayAdapter,
      PaymentMethod.Momo => _momoAdapter,
      _ => PaymentErrors.InvalidPaymentMethod
    };
  }
}