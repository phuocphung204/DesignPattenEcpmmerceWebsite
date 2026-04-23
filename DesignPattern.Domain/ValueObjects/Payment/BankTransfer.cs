using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Domain.ValueObjects.Payment;

public sealed record BankTransfer : PaymentInfo
{
  public StandardText BankName { get; init; }
  public StandardText AccountNumber { get; init; }
  private BankTransfer(
    StandardText bankName,
    StandardText accountNumber) : base(PaymentType.BankTransfer)
  {
    BankName = bankName;
    AccountNumber = accountNumber;
  }
  private BankTransfer(
    StandardText bankName,
    StandardText accountNumber,
    string? paymentProvider,
    string? paymentChannel,
    string? transactionId,
    DateTime? paidAt) : base(PaymentType.BankTransfer, paymentProvider, paymentChannel, transactionId, paidAt)
  {
    BankName = bankName;
    AccountNumber = accountNumber;
  }
  public static Result<BankTransfer> Create(
    string bankName,
    string accountNumber)
  {
    var bankNameResult = StandardText.Create(bankName);
    var accountNumberResult = StandardText.Create(accountNumber);
    if (bankNameResult.IsFailure)
    {
      return bankNameResult.Error;
    }
    if (accountNumberResult.IsFailure)
    {
      return accountNumberResult.Error;
    }
    return new BankTransfer(bankNameResult.Value, accountNumberResult.Value);
  }

  public static BankTransfer Rehydrate(
    string bankName,
    string accountNumber,
    string? paymentProvider,
    string? paymentChannel,
    string? transactionId,
    DateTime? paidAt)
  {
    var bankNameValue = StandardText.Rehydrate(bankName);
    var accountNumberValue = StandardText.Rehydrate(accountNumber);
    return new BankTransfer(bankNameValue, accountNumberValue, paymentProvider, paymentChannel, transactionId, paidAt);
  }
}