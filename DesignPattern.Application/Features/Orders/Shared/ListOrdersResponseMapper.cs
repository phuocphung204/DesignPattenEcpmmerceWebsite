using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Orders;

namespace DesignPattern.Application.Features.Orders.Shared;

public static class ListOrdersResponseMapper
{
  public static List<ListOrdersResponse> MapToListOrdersResponse(List<Order> orders)
  {
    if (orders.Count == 0)
      return new List<ListOrdersResponse>();
    var response = orders.Select(order => new ListOrdersResponse(
       order.Id,
       order.GrandAmount.Amount,
       order.Status,
       order.CreatedAt,
       order.Items.Select(i => new OrderItemQueryResponse(
         i.ProductId,
         i.ProductName.Value,
         i.Quantity.Value,
         i.SellingPrice.Amount,
         i.ThumbnailUrl
       )).ToList()
     )).ToList();
    return response;
  }
}