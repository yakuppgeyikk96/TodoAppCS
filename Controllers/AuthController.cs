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
    var result = await _authService.RegisterAsync(registerDto);

    if (!result.Success)
      return BadRequest(result);

    return Ok(result);
  }

  [HttpPost("login")]
  [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status401Unauthorized)]
  public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(LoginDto loginDto)
  {
    var result = await _authService.LoginAsync(loginDto);

    if (!result.Success)
      return Unauthorized(result);

    return Ok(result);
  }
}
