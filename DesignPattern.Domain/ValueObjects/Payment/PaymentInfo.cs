using DesignPattern.Domain.Enums;

namespace DesignPattern.Domain.ValueObjects.Payment;

public abstract record class PaymentInfo
{
  public PaymentType Type { get; }
  public string? PaymentProvider { get; private set; } // VNPay, SePay
  public string? PaymentChannel { get; private set; }  // VISA, ATM, NCB
  public string? TransactionId { get; private set; }
  public DateTime? PaidAt { get; private set; }

  protected PaymentInfo(PaymentType type)
  {
    Type = type;
  }
  protected PaymentInfo(PaymentType type, string? paymentProvider, string? paymentChannel, string? transactionId, DateTime? paidAt)
  {
    Type = type;
    PaymentProvider = paymentProvider;
    PaymentChannel = paymentChannel;
    TransactionId = transactionId;
    PaidAt = paidAt;
  }
  public PaymentInfo MarkAsPaid(
    string paymentProvider,
    string paymentChannel,
    string transactionId,
    DateTime paidAt)
  {
    PaymentProvider = paymentProvider;
    PaymentChannel = paymentChannel;
    TransactionId = transactionId;
    PaidAt = paidAt;
    return this;
  }
  public void SetPaidAt(DateTime paidAt)
  {
    PaidAt = paidAt;
  }
}