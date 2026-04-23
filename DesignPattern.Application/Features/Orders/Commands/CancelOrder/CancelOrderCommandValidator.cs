using DesignPattern.Application.Abstractions;
using DesignPattern.Application.Features.Orders.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using FluentValidation;
using MediatR;

namespace DesignPattern.Application.Features.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
  public CancelOrderCommandValidator()
  {
    RuleFor(x => x.dto.Note)
      .NotEmpty().WithMessage("Lý do hủy đơn hàng là bắt buộc.")
      .MaximumLength(500).WithMessage("Lý do hủy đơn hàng không được vượt quá 500 ký tự.");
  }
}