using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Domain.ValueObjects.Payment;

public abstract class PaymentData
{
  public PaymentType Type { get; }
  protected PaymentData(PaymentType type)
  {
    Type = type;
  }
}

public class BankTransferPaymentData : PaymentData
{
  public string BankName { get; init; }
  public string AccountNumber { get; init; }

  public BankTransferPaymentData(
    string bankName,
    string accountNumber) : base(PaymentType.BankTransfer)
  {
    BankName = bankName;
    AccountNumber = accountNumber;
  }
}

public class CreditCardPaymentData : PaymentData
{
  public string CardNumber { get; init; }
  public string CardHolder { get; init; }
  public string CardType { get; init; }

  public CreditCardPaymentData(
    string cardNumber,
    string cardHolder,
    string cardType) : base(PaymentType.CreditCard)
  {
    CardNumber = cardNumber;
    CardHolder = cardHolder;
    CardType = cardType;
  }
}

public class CashPaymentData : PaymentData
{
  public CashPaymentData() : base(PaymentType.Cash)
  {
  }
}