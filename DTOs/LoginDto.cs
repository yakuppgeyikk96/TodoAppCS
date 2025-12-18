using System.ComponentModel.DataAnnotations;

namespace FirstWebApi.DTOs;

public class LoginDto
{
  public string Username { get; set; } = string.Empty;

  [Required]
  public string Password { get; set; } = string.Empty;
}
