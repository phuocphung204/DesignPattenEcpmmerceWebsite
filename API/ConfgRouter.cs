using System.Reflection;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

public static class ConfigRouter
{
  public static IServiceCollection RegisterApiServices(this IServiceCollection services)
  {
    services.AddControllers(); // Đăng ký các controller
    services.AddEndpointsApiExplorer(); // Đăng ký dịch vụ khám phá API để hỗ trợ OpenAPI/Swagger
    services.AddOpenApi(); // Đăng ký dịch vụ OpenAPI để tự động tạo tài liệu API

    services.AddSwaggerGen(options =>
    {
      options.SwaggerDoc("v1", new OpenApiInfo { Title = "Design Pattern API", Version = "v1" });

      // Cấu hình Swagger để đọc comments từ file XML
      var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
      options.IncludeXmlComments(xmlPath);
    });

    return services;
  }

  public static IServiceCollection RegisterJwt(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddAuthentication(options =>
    {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        // 1. Kiểm tra chữ ký (Phải khớp với Key trong hàm GenerateToken)
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),

        // 2. Kiểm tra Issuer (Người phát hành)
        ValidateIssuer = true,
        ValidIssuer = configuration["Jwt:Issuer"],

        // 3. Kiểm tra Audience (Người nhận)
        ValidateAudience = true,
        ValidAudience = configuration["Jwt:Audience"],

        // 4. Kiểm tra thời hạn (Hết hạn là không cho vào)
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero, // Khớp giây cuối cùng, không bù trừ 5 phút mặc định
      };
    });

    return services;
  }

}
