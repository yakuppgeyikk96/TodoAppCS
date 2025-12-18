using System.ComponentModel.DataAnnotations;

namespace FirstWebApi.DTOs;

public class UpdateTodoDto
{
  [Required(ErrorMessage = "Title is required")]
  [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
  public string Title { get; set; } = string.Empty;

  [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
  public string Description { get; set; } = string.Empty;

  public bool IsCompleted { get; set; }
}
