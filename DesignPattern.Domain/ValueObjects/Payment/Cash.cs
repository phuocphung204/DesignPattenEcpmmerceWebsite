using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Domain.ValueObjects.Payment;

public sealed record Cash : PaymentInfo
{
  private Cash() : base(PaymentType.Cash) { }
  private Cash(
    string? paymentProvider,
    string? paymentChannel,
    string? transactionId,
    DateTime? paidAt) : base(PaymentType.Cash, paymentProvider, paymentChannel, transactionId, paidAt)
  {
  }
  public static Result<Cash> Create()
  {
    return Result<Cash>.Success(new Cash());
  }

  public static Cash Rehydrate(string? paymentProvider, string? paymentChannel, string? transactionId, DateTime? paidAt)
  {
    return new Cash(paymentProvider, paymentChannel, transactionId, paidAt);
  }
}