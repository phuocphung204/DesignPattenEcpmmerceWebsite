using MediatR;
using DesignPattern.Application.Abstractions.Payments;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Features.Payments.CreatePayment;

public record CreatePaymentCommand(CreatePaymentDTO dto)
  : IRequest<Result<CreatePaymentResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Customer };
}
// Trả về URL thanh toán hoặc thông tin cần thiết để khách hàng thực hiện thanh toán