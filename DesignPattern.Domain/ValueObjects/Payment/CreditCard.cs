using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Domain.ValueObjects.Payment;

public sealed record CreditCard : PaymentInfo
{
  public StandardText CardNumber { get; init; }
  public StandardText CardHolder { get; init; }
  public StandardText CardType { get; init; }
  private CreditCard(
    StandardText cardNumber,
    StandardText cardHolder,
    StandardText cardType) : base(PaymentType.CreditCard)
  {
    CardNumber = cardNumber;
    CardHolder = cardHolder;
    CardType = cardType;
  }

  private CreditCard(
    StandardText cardNumber,
    StandardText cardHolder,
    StandardText cardType,
    string? paymentProvider,
    string? paymentChannel,
    string? transactionId,
    DateTime? paidAt) : base(PaymentType.CreditCard, paymentProvider, paymentChannel, transactionId, paidAt)
  {
    CardNumber = cardNumber;
    CardHolder = cardHolder;
    CardType = cardType;
  }

  public static Result<CreditCard> Create(
    string cardNumber,
    string cardHolder,
    string cardType)
  {
    var cardNumberResult = StandardText.Create(cardNumber);
    var cardHolderResult = StandardText.Create(cardHolder);
    var cardTypeResult = StandardText.Create(cardType);

    if (cardNumberResult.IsFailure)
    {
      return cardNumberResult.Error;
    }

    if (cardHolderResult.IsFailure)
    {
      return cardHolderResult.Error;
    }

    if (cardTypeResult.IsFailure)
    {
      return cardTypeResult.Error;
    }

    return new CreditCard(cardNumberResult.Value, cardHolderResult.Value, cardTypeResult.Value);
  }

  public static CreditCard Rehydrate(string cardNumber, string cardHolder, string cardType, string? paymentProvider, string? paymentChannel, string? transactionId, DateTime? paidAt)
  {
    var cardNumberValue = StandardText.Rehydrate(cardNumber);
    var cardHolderValue = StandardText.Rehydrate(cardHolder);
    var cardTypeValue = StandardText.Rehydrate(cardType);
    return new CreditCard(cardNumberValue, cardHolderValue, cardTypeValue, paymentProvider, paymentChannel, transactionId, paidAt);
  }
}