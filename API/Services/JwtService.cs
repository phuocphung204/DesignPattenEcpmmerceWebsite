using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtService
{
  private readonly IConfiguration _config;

  public JwtService(IConfiguration config)
  {
    _config = config;
  }

  public string GenerateToken(string userId, string email, string role)
  {
    // 1. Tạo danh sách Claims (Thông tin người dùng)
    var claims = new[]
    {
      new Claim(ClaimTypes.NameIdentifier, userId),
      new Claim(ClaimTypes.Email, email),
      new Claim(ClaimTypes.Role, role), // Phân quyền

      // new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // ID của Token
    };

    // 2. Lấy Secret Key từ config và tạo SigningCredentials
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    // 3. Thiết lập thông tin Token (Hạn dùng, Người phát hành...)
    var token = new JwtSecurityToken(
      issuer: _config["Jwt:Issuer"],
      audience: _config["Jwt:Audience"],
      claims: claims,
      expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:DurationInMinutes"])),
      signingCredentials: creds
    );

    // 4. Chuyển đối tượng Token thành chuỗi String
    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  public bool isValidToken(string token)
  {
    var validationParameters = new TokenValidationParameters
    {
      ValidateIssuerSigningKey = true, // Bắt buộc kiểm tra chữ ký
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)),

      ValidateIssuer = true, // Kiểm tra nguồn phát hành (iss)
      ValidIssuer = _config["Jwt:Issuer"],

      ValidateAudience = true, // Kiểm tra đối tượng nhận (aud)
      ValidAudience = _config["Jwt:Audience"],

      ValidateLifetime = true, // Kiểm tra hết hạn (exp)
      ClockSkew = TimeSpan.Zero // Loại bỏ thời gian trễ mặc định (thường là 5p)
    };

    var tokenHandler = new JwtSecurityTokenHandler();

    try
    {
      // Thực hiện xác thực
      var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
    }
    catch
    {
      return false;
    }

    return true;
  }
}
