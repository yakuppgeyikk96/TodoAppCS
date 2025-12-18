

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FirstWebApi.Data;
using FirstWebApi.DTOs;
using FirstWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BC = BCrypt.Net.BCrypt;

namespace FirstWebApi.Services;

public class AuthService(ApplicationDbContext context, IConfiguration configuration) : IAuthService
{
  private readonly ApplicationDbContext _context = context;
  private readonly IConfiguration _configuration = configuration;

  public async Task<ApiResponse<User>> RegisterAsync(RegisterDto registerDto)
  {
    if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
      return ApiResponse<User>.ErrorResponse("Username already exists");

    string passwordHash = BC.HashPassword(registerDto.Password);

    var user = new User
    {
      Username = registerDto.Username,
      Email = registerDto.Email,
      PasswordHash = passwordHash
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return ApiResponse<User>.SuccessResponse(user, "User registered successfully");
  }

  public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto)
  {
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

    if (user == null || !BC.Verify(loginDto.Password, user.PasswordHash))
      return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid username or password");

    string token = CreateToken(user);

    return ApiResponse<AuthResponseDto>.SuccessResponse(new AuthResponseDto
    {
      Token = token,
      Username = user.Username
    }, "Login successful");
  }



  private string CreateToken(User user)
  {
    var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email)
        };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
        _configuration.GetSection("Jwt:Key").Value!));

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = DateTime.Now.AddDays(1),
      SigningCredentials = creds,
      Issuer = _configuration.GetSection("Jwt:Issuer").Value,
      Audience = _configuration.GetSection("Jwt:Audience").Value
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var token = tokenHandler.CreateToken(tokenDescriptor);

    return tokenHandler.WriteToken(token);
  }
}
