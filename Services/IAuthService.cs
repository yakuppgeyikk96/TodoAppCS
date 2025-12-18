using FirstWebApi.DTOs;
using FirstWebApi.Models;

namespace FirstWebApi.Services;

public interface IAuthService
{
  Task<ApiResponse<User>> RegisterAsync(RegisterDto registerDto);
  Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto);
}
