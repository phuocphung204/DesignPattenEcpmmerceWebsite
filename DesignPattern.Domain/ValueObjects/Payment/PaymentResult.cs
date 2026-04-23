public record PaymentResult(
  Guid OrderCode,
  decimal Amount,
  string Provider,
  string Channel,
  string TransactionId,
  DateTime PaidAt,
  bool IsSuccess,
  string? ErrorCode = null,
  string? Message = null);