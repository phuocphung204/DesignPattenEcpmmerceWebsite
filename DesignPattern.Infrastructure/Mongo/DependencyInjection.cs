using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Abstractions;
using DesignPattern.Infrastructure.Mongo.Repositories;
using DesignPattern.Infrastructure.Mongo.Service;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using DesignPattern.Infrastructure.Mongo.Mappings;
using DesignPattern.Application.Abstractions.Payments;
using DesignPattern.Infrastructure.Payments;
using DesignPattern.Infrastructure.Payments.Momo;
using DesignPattern.Infrastructure.Payments.VNPay;

namespace DesignPattern.Infrastructure.Mongo;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    var settings = configuration.GetSection("MongoDb").Get<MongoDbSettings>()
        ?? throw new InvalidOperationException("Missing MongoDb settings");
    services.AddSingleton(settings);

    // Đăng ký MongoDB client và database
    services.AddSingleton<IMongoClient>(_ => new MongoClient(settings.ConnectionString));
    services.AddSingleton<IMongoDatabase>(sp =>
    {
      var client = sp.GetRequiredService<IMongoClient>();
      return client.GetDatabase(settings.DatabaseName);
    });

    services.AddScoped<MongoContext>();
    services.AddScoped<ISlugChecker, MongoSlugChecker>();
    services.AddScoped<IUnitOfWork, MongoUnitOfWork>();

    // services.AddScoped<IUserQueries, UserQueries>();
    services.AddScoped<ICurrentUserService, CurrentUserService>();
    services.AddScoped<FakeVnPayService>();
    services.AddScoped<FakeMoMoService>();
    services.AddScoped<VnPayAdapter>();
    services.AddScoped<MomoAdapter>();
    services.AddScoped<IPaymentGatewayFactory, PaymentGatewayFactory>();


    services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

    return services;
  }

}
