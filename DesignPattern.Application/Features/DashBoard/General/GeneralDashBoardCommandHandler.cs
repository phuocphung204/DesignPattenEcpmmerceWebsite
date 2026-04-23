using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities;
using DesignPattern.Domain.Entities.Orders;
using DesignPattern.Domain.Entities.Users;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.DashBoard.General;

public class GeneralDashBoardQueryHandler : IRequestHandler<GeneralDashBoardQuery, Result<DashboardGeneralResponse>>
{
  private readonly IUnitOfWork _unitOfWork;
  public GeneralDashBoardQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }
  public async Task<Result<DashboardGeneralResponse>> Handle(GeneralDashBoardQuery request, CancellationToken cancellationToken)
  {
    var topN = 10; // Default value for top N products, you can modify this to accept from request if needed
    var users = await _unitOfWork.UserRepository.GetAllAsync(cancellationToken);
    var orders = await _unitOfWork.OrderRepository.GetAllAsync(cancellationToken);
    var paidOrders = await _unitOfWork.OrderRepository.GetListPaidOrdersByDateRangeAsync(DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow, cancellationToken);
    var topSellingProducts = await _unitOfWork.ProductRepository.GetTopSellingProductsAsync(topN, cancellationToken);

    int totalUsers = users.Count();
    int newUsersThisMonth = users.Count(u => u.CreatedAt.Month == DateTime.UtcNow.Month && u.CreatedAt.Year == DateTime.UtcNow.Year);
    int totalOrders = orders.Count();
    decimal totalRevenue = paidOrders.Sum(o => o.GrandAmount.Amount);

    List<TopProductResponse> topProductResponses = topSellingProducts.Select(p => new TopProductResponse(p.Id, p.Name.Value, p.SoldQuantity.Value)).ToList();
    var response = new DashboardGeneralResponse(
      TotalUsers: totalUsers,
      NewUsersThisMonth: newUsersThisMonth,
      TotalOrders: totalOrders,
      TotalRevenue: totalRevenue,
      TopSellingProducts: topProductResponses
    );

    return response;

  }
}