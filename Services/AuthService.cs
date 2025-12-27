// Services/AuthService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FirstWebApi.DTOs;
using FirstWebApi.Models;
using FirstWebApi.Repositories;
using Microsoft.IdentityModel.Tokens;
using BC = BCrypt.Net.BCrypt;
using FirstWebApi.Exceptions;

namespace FirstWebApi.Services;

public class AuthService(IUserRepository userRepository, IConfiguration configuration) : IAuthService
{
  private readonly IUserRepository _userRepository = userRepository;
  private readonly IConfiguration _configuration = configuration;

  public async Task<User> RegisterAsync(RegisterDto registerDto)
  {
    if (await _userRepository.ExistsByUsernameAsync(registerDto.Username))
    {
      throw new BadRequestException("Username already exists");
    }

    if (await _userRepository.ExistsByEmailAsync(registerDto.Email))
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

    await _userRepository.AddAsync(user);
    await _userRepository.SaveChangesAsync();

    return user;
  }

  public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
  {
    var user = await _userRepository.GetByUsernameAsync(loginDto.Username);

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
