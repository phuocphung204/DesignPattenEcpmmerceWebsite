namespace DesignPattern.Application.Abstractions.Payments;

using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;

public interface IPaymentGatewayFactory
{
  Result<IPaymentGateway> Create(PaymentMethod paymentMethod);
}