

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FirstWebApi.Data;
using FirstWebApi.DTOs;
using FirstWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BC = BCrypt.Net.BCrypt;
using FirstWebApi.Exceptions;

namespace FirstWebApi.Services;

public class AuthService(ApplicationDbContext context, IConfiguration configuration) : IAuthService
{
  private readonly ApplicationDbContext _context = context;
  private readonly IConfiguration _configuration = configuration;

  public async Task<User> RegisterAsync(RegisterDto registerDto) // Artık ApiResponse değil
  {
    if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
    {
      throw new BadRequestException("Username already exists");
    }

    if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
    {
      throw new BadRequestException("Email already exists");
    }

    string passwordHash = BC.HashPassword(registerDto.Password);

    var user = new User
    {
      Username = registerDto.Username,
      Email = registerDto.Email,
      PasswordHash = passwordHash
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return user;
  }

  public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto) // Artık ApiResponse değil
  {
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

    if (user == null || !BC.Verify(loginDto.Password, user.PasswordHash))
    {
      throw new UnauthorizedException("Invalid username or password");
    }

    string token = CreateToken(user);

    return new AuthResponseDto
    {
      Token = token,
      Username = user.Username
    };
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
