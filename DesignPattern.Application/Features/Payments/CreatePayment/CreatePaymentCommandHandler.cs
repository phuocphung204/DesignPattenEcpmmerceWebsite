using MediatR;
using DesignPattern.Application.Abstractions.Payments;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Application.Features.Payments.CreatePayment;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Result<CreatePaymentResponse>>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IPaymentGatewayFactory _paymentGatewayFactory;
  public CreatePaymentCommandHandler(IUnitOfWork unitOfWork, IPaymentGatewayFactory paymentGatewayFactory)
  {
    _unitOfWork = unitOfWork;
    _paymentGatewayFactory = paymentGatewayFactory;
  }
  public async Task<Result<CreatePaymentResponse>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    var order = await _unitOfWork.OrderRepository.GetByIdAsync(dto.OrderId, cancellationToken);
    if (order is null)
      return OrderErrors.OrderNotFound;

    if (order.UserId != request.UserId)
      return OrderErrors.UserConflict;

    if (order.PaymentStatus == PaymentStatusEnum.Paid)
      return OrderErrors.AlreadyPaid;

    var paymentGateway = _paymentGatewayFactory.Create(dto.PaymentMethod);
    if (paymentGateway.IsFailure)
    {
      return paymentGateway.Error;
    }
    var paymentGatewayInstance = paymentGateway.Value;

    // Gửi yêu cầu đến cổng thanh toán và nhận về URL thanh toán hoặc thông tin cần thiết
    var paymentResult = await paymentGatewayInstance.CreatePaymentAsync(dto);
    // Trả về URL thanh toán hoặc thông tin cần thiết để khách hàng thực hiện thanh toán
    return new CreatePaymentResponse(paymentResult.PaymentUrl, paymentResult.OrderId);
  }
}