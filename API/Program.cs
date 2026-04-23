using DesignPattern.Application;
using DesignPattern.Infrastructure.Mongo;
using DesignPattern.Infrastructure.Mongo.Abstractions;
using API.Middlewares;
using DesignPattern.Domain.Patterns.SingletonPattern;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
  builder.Logging.AddFilter("LuckyPennySoftware.MediatR.License", LogLevel.None);
}

// Cấu hình Serilog từ appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
// Thay thế logger mặc định bằng Serilog
builder.Host.UseSerilog();

builder.Services.RegisterJwt(builder.Configuration);
builder.Services.AddHttpContextAccessor(); // đăng ký http context accessor
builder.Services.AddScoped<JwtService>();
builder.Services.RegisterApiServices();
builder.Services.AddCors(options =>
{
  options.AddPolicy("FrontendDevCors", policy =>
  {
    policy
      .WithOrigins("http://localhost:3000")
      .AllowAnyHeader()
      .AllowAnyMethod();
  });
});

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddExceptionHandler<ExceptionMiddleware>();
builder.Services.AddProblemDetails();

// TỰ ĐỘNG QUÉT VÀ ĐĂNG KÝ MODULE CẤU HÌNH
// Tìm assembly chứa lớp IMongoCollectionConfiguration
var assembly = typeof(IMongoCollectionConfiguration).Assembly;
// Lọc ra các class implement interface này (không phải là interface hoặc abstract class)
var configurationTypes = assembly.GetTypes()
    .Where(t => typeof(IMongoCollectionConfiguration).IsAssignableFrom(t)
                && !t.IsInterface
                && !t.IsAbstract);
// Đăng ký tất cả vào DI
foreach (var type in configurationTypes)
{
  builder.Services.AddTransient(typeof(IMongoCollectionConfiguration), type);
}

// Đăng ký MongoDbInitializer để khởi tạo database khi ứng dụng chạy
builder.Services.AddScoped<MongoDbInitializer>();
// builder.Services.AddUnitOfWork<MongoUnitOfWork>();


var app = builder.Build();

AppLogger.Instance.LogInfo("API bootstrap started");

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.UseSwagger();
  app.UseSwaggerUI();
}


// Khởi chạy Initializer (có cơ chế fallback để API không chết khi Mongo tạm thời không sẵn sàng)
var allowStartupWithoutMongo = builder.Configuration.GetValue<bool>("MongoDb:AllowStartupWithoutMongo", true);
using (var scope = app.Services.CreateScope())
{
  var initializer = scope.ServiceProvider.GetRequiredService<MongoDbInitializer>();
  try
  {
    await initializer.InitializeAsync();
    Console.WriteLine("MongoDB initialization completed.");
  }
  catch (Exception ex) when (allowStartupWithoutMongo &&
                             (ex is TimeoutException ||
                              ex is MongoConnectionException ||
                              ex is MongoConfigurationException))
  {
    Log.Warning(ex,
      "MongoDB initialization failed during startup. Continuing because MongoDb:AllowStartupWithoutMongo=true");
  }
}

// Log các request HTTP (Tùy chọn nhưng rất hữu ích)
app.UseSerilogRequestLogging();

app.UseCors("FrontendDevCors");

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
