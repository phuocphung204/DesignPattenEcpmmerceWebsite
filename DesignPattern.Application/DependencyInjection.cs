using DesignPattern.Application.Behaviors;
using DesignPattern.Application.Features.Orders.Observers;
using DesignPattern.Domain.Abstractions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DesignPattern.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
    // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

    services.AddMediatR(cfg =>
    {
      cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

      // 3. UserContextBehavior (Xác thực & Điền UserId)
      cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UserContextBehavior<,>));

      // 4. AuthorizationBehavior (Kiểm tra role của request)
      cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));

      // 5. ValidationBehavior (Kiểm tra dữ liệu sạch)
      cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    });

    services.AddScoped<IOrderObserver, InventoryObserver>();
    services.AddScoped<IOrderObserver, RewardPointObserver>();
    services.AddScoped<IOrderObserver, DiscountCodeObserver>();

    return services;
  }
}
