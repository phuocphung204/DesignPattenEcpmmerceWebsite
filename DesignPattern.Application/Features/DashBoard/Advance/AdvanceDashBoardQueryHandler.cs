using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Orders;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.DashBoard.Advance;

public class AdvanceDashBoardQueryHandler : IRequestHandler<AdvanceDashBoardQuery, Result<AdvanceDashboardResponse>>
{
  private readonly IUnitOfWork _unitOfWork;

  private enum TimelineGroupKey
  {
    Month,
    Week,
    Day
  }

  private sealed record TimelineRawPoint(
    int? Month,
    int? Week,
    int? Day,
    decimal TotalRevenue,
    decimal TotalProfit,
    int TotalProductsSold);

  public AdvanceDashBoardQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<AdvanceDashboardResponse>> Handle(AdvanceDashBoardQuery request, CancellationToken cancellationToken)
  {
    var allOrders = await _unitOfWork.OrderRepository.GetAllAsync(cancellationToken);
    var paidOrders = allOrders
      .Where(o => o.PaymentStatus == PaymentStatusEnum.Paid && o.Status == OrderStatusEnum.Completed)
      .ToList();

    var filteredOrders = ApplyTimeFilter(paidOrders, request);

    var general = new AdvanceDashboardGeneralResponse(
      TotalOrders: filteredOrders.Count,
      TotalRevenue: filteredOrders.Sum(o => o.GrandAmount.Amount),
      TotalProfit: filteredOrders.Sum(CalculateOrderProfit));

    var products = await _unitOfWork.ProductRepository.GetAllAsync(cancellationToken);
    var productCategoryById = products
      .GroupBy(p => p.Id)
      .ToDictionary(g => g.Key, g => g.First().CategoryName.Value);

    var category = filteredOrders
      .SelectMany(o => o.Items)
      .GroupBy(i => productCategoryById.TryGetValue(i.ProductId, out var categoryName) ? categoryName : "Unknown")
      .Select(g => new AdvanceDashboardCategoryResponse(
        CategoryName: g.Key,
        TotalQuantity: g.Sum(i => i.Quantity.Value)))
      .OrderByDescending(x => x.TotalQuantity)
      .ToList();

    var timelineGroup = GetTimelineGrouping(request);
    var timelineRaw = BuildTimelineRaw(filteredOrders, timelineGroup)
      .OrderBy(x => x.Day ?? x.Week ?? x.Month ?? 0)
      .ToList();

    var year = request.Annual ?? DateTime.UtcNow.Year;
    var month = request.Monthly ?? DateTime.UtcNow.Month;
    var week = request.Weekly ?? 1;

    var timeline = BuildTimelineSkeleton(request, timelineRaw, year, month, week);

    return new AdvanceDashboardResponse(
      General: general,
      Category: category,
      Timeline: timeline);
  }

  private static decimal CalculateOrderProfit(Order order)
  {
    return order.Items.Sum(i => (i.SellingPrice.Amount - i.PurchasePrice.Amount) * i.Quantity.Value);
  }

  private static List<Order> ApplyTimeFilter(List<Order> source, AdvanceDashBoardQuery request)
  {
    var now = DateTime.UtcNow;

    if (request.StartDate.HasValue && request.EndDate.HasValue)
    {
      var start = request.StartDate.Value;
      var end = request.EndDate.Value;
      return source.Where(o => o.CreatedAt >= start && o.CreatedAt < end).ToList();
    }

    var year = request.Annual ?? now.Year;
    var startOfPeriod = new DateTime(year, 1, 1);
    var endOfPeriod = new DateTime(year + 1, 1, 1);

    if (request.Quarterly.HasValue)
    {
      var q = Math.Clamp(request.Quarterly.Value, 1, 4);
      var startMonth = (q - 1) * 3 + 1;
      startOfPeriod = new DateTime(year, startMonth, 1);
      endOfPeriod = startOfPeriod.AddMonths(3);
    }

    if (request.Monthly.HasValue)
    {
      var month = Math.Clamp(request.Monthly.Value, 1, 12);
      startOfPeriod = new DateTime(year, month, 1);
      endOfPeriod = startOfPeriod.AddMonths(1);
    }

    if (request.Weekly.HasValue && request.Monthly.HasValue)
    {
      var month = Math.Clamp(request.Monthly.Value, 1, 12);
      var week = Math.Max(request.Weekly.Value, 1);
      var startOfMonth = new DateTime(year, month, 1);
      startOfPeriod = startOfMonth.AddDays((week - 1) * 7);
      endOfPeriod = startOfPeriod.AddDays(7);

      var endOfMonth = startOfMonth.AddMonths(1);
      if (endOfPeriod > endOfMonth)
      {
        endOfPeriod = endOfMonth;
      }
    }

    return source.Where(o => o.CreatedAt >= startOfPeriod && o.CreatedAt < endOfPeriod).ToList();
  }

  private static TimelineGroupKey GetTimelineGrouping(AdvanceDashBoardQuery request)
  {
    var hasCustomRange = request.StartDate.HasValue && request.EndDate.HasValue;

    if ((request.Weekly.HasValue && request.Monthly.HasValue) || hasCustomRange)
      return TimelineGroupKey.Day;

    if (request.Monthly.HasValue)
      return TimelineGroupKey.Week;

    return TimelineGroupKey.Month;
  }

  private static IEnumerable<TimelineRawPoint> BuildTimelineRaw(IEnumerable<Order> orders, TimelineGroupKey key)
  {
    return key switch
    {
      TimelineGroupKey.Day => orders
        .GroupBy(o => o.CreatedAt.Day)
        .Select(g => new TimelineRawPoint(
          Month: null,
          Week: null,
          Day: g.Key,
          TotalRevenue: g.Sum(o => o.GrandAmount.Amount),
          TotalProfit: g.Sum(CalculateOrderProfit),
          TotalProductsSold: g.Sum(o => o.Items.Sum(i => i.Quantity.Value)))),

      TimelineGroupKey.Week => orders
        .GroupBy(o => (int)Math.Ceiling(o.CreatedAt.Day / 7.0m))
        .Select(g => new TimelineRawPoint(
          Month: null,
          Week: g.Key,
          Day: null,
          TotalRevenue: g.Sum(o => o.GrandAmount.Amount),
          TotalProfit: g.Sum(CalculateOrderProfit),
          TotalProductsSold: g.Sum(o => o.Items.Sum(i => i.Quantity.Value)))),

      _ => orders
        .GroupBy(o => o.CreatedAt.Month)
        .Select(g => new TimelineRawPoint(
          Month: g.Key,
          Week: null,
          Day: null,
          TotalRevenue: g.Sum(o => o.GrandAmount.Amount),
          TotalProfit: g.Sum(CalculateOrderProfit),
          TotalProductsSold: g.Sum(o => o.Items.Sum(i => i.Quantity.Value))))
    };
  }

  private static List<AdvanceDashboardTimelineResponse> BuildTimelineSkeleton(
    AdvanceDashBoardQuery request,
    List<TimelineRawPoint> timelineRaw,
    int year,
    int month,
    int week)
  {
    timelineRaw ??= new List<TimelineRawPoint>();
    var timeline = new List<AdvanceDashboardTimelineResponse>();

    TimelineRawPoint? FindByDay(int day) => timelineRaw.FirstOrDefault(t => t.Day == day);
    TimelineRawPoint? FindByWeek(int value) => timelineRaw.FirstOrDefault(t => t.Week == value);
    TimelineRawPoint? FindByMonth(int value) => timelineRaw.FirstOrDefault(t => t.Month == value);

    AdvanceDashboardTimelineResponse BuildFallback(int? monthValue, int? weekValue, int? dayValue)
      => new(
        Month: monthValue,
        Week: weekValue,
        Day: dayValue,
        TotalRevenue: 0,
        TotalProfit: 0,
        TotalProductsSold: 0);

    if (request.Weekly.HasValue && request.Monthly.HasValue)
    {
      var safeMonth = Math.Clamp(month, 1, 12);
      var safeWeek = Math.Max(week, 1);

      var startOfMonth = new DateTime(year, safeMonth, 1);
      var startOfWeek = startOfMonth.AddDays((safeWeek - 1) * 7);
      var endOfWeek = startOfWeek.AddDays(7);
      var firstDayOfNextMonth = startOfMonth.AddMonths(1);
      var lastDay = endOfWeek < firstDayOfNextMonth
        ? endOfWeek.Day
        : DateTime.DaysInMonth(year, safeMonth);

      for (var d = startOfWeek.Day; d < lastDay; d++)
      {
        var item = FindByDay(d);
        timeline.Add(item is null
          ? BuildFallback(null, null, d)
          : new AdvanceDashboardTimelineResponse(item.Month, item.Week, item.Day, item.TotalRevenue, item.TotalProfit, item.TotalProductsSold));
      }
    }
    else if (request.Monthly.HasValue)
    {
      for (var w = 1; w <= 4; w++)
      {
        var item = FindByWeek(w);
        timeline.Add(item is null
          ? BuildFallback(null, w, null)
          : new AdvanceDashboardTimelineResponse(item.Month, item.Week, item.Day, item.TotalRevenue, item.TotalProfit, item.TotalProductsSold));
      }
    }
    else if (request.Quarterly.HasValue)
    {
      var quarter = Math.Clamp(request.Quarterly.Value, 1, 4);
      var startMonth = (quarter - 1) * 3 + 1;
      for (var m = startMonth; m < startMonth + 3; m++)
      {
        var item = FindByMonth(m);
        timeline.Add(item is null
          ? BuildFallback(m, null, null)
          : new AdvanceDashboardTimelineResponse(item.Month, item.Week, item.Day, item.TotalRevenue, item.TotalProfit, item.TotalProductsSold));
      }
    }
    else if (request.Annual.HasValue)
    {
      for (var m = 1; m <= 12; m++)
      {
        var item = FindByMonth(m);
        timeline.Add(item is null
          ? BuildFallback(m, null, null)
          : new AdvanceDashboardTimelineResponse(item.Month, item.Week, item.Day, item.TotalRevenue, item.TotalProfit, item.TotalProductsSold));
      }
    }
    else if (request.StartDate.HasValue && request.EndDate.HasValue)
    {
      var startDay = request.StartDate.Value.Day;
      var endDay = request.EndDate.Value.Day;
      for (var d = startDay; d <= endDay; d++)
      {
        var item = FindByDay(d);
        timeline.Add(item is null
          ? BuildFallback(null, null, d)
          : new AdvanceDashboardTimelineResponse(item.Month, item.Week, item.Day, item.TotalRevenue, item.TotalProfit, item.TotalProductsSold));
      }
    }

    return timeline;
  }
}