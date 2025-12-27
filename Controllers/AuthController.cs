// Controllers/AuthController.cs
using FirstWebApi.DTOs;
using FirstWebApi.Models;
using FirstWebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FirstWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
  private readonly IAuthService _authService = authService;

  [HttpPost("register")]
  [ProducesResponseType(typeof(ApiResponse<User>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<User>), StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<ApiResponse<User>>> Register(RegisterDto registerDto)
  {
    var user = await _authService.RegisterAsync(registerDto);
    return Ok(ApiResponse<User>.SuccessResponse(user, "User registered successfully"));
  }

  [HttpPost("login")]
  [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status401Unauthorized)]
  public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(LoginDto loginDto)
  {
    var authResponse = await _authService.LoginAsync(loginDto);
    return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(authResponse, "Login successful"));
  }
}
