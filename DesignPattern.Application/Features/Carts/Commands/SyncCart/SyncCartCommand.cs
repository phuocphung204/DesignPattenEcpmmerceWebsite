using MediatR;
using DesignPattern.Domain.Common;

namespace DesignPattern.Application.Features.Carts.Commands.SyncCart;

public record CartItemDto(
    Guid ProductId, string Sku, string Name, string ImageLink,
    int Quantity, decimal SellingPrice);

public record SyncCartCommand(Guid UserId, List<CartItemDto> Items) : IRequest<Result<bool>>;