using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Domain.ValueObjects.Payment;

public static class PaymentFactory
{
  public static Result<PaymentInfo> Create(PaymentData data)
  {
    switch (data)
    {
      case BankTransferPaymentData bankTransferData:
        var bankTransferResult = BankTransfer.Create(
          bankTransferData.BankName,
          bankTransferData.AccountNumber);
        if (bankTransferResult.IsFailure)
          return bankTransferResult.Error;
        return bankTransferResult.Value;
      case CreditCardPaymentData creditCardData:
        var creditCardResult = CreditCard.Create(
          creditCardData.CardNumber,
          creditCardData.CardHolder,
          creditCardData.CardType);
        if (creditCardResult.IsFailure)
          return creditCardResult.Error;
        return creditCardResult.Value;
      case CashPaymentData cashData:
        var cashResult = Cash.Create();
        if (cashResult.IsFailure)
          return cashResult.Error;
        return cashResult.Value;
      default:
        return Error.Validation("Domain.Payment.Invalid.PaymentDataType", "Invalid payment data type");
    }
  }
}