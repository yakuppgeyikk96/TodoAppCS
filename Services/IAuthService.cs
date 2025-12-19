using FirstWebApi.DTOs;
using FirstWebApi.Models;

namespace FirstWebApi.Services;

public interface IAuthService
{
  Task<User> RegisterAsync(RegisterDto registerDto);
  Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
}
