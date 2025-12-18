namespace FirstWebApi.Models;

public class Todo : BaseEntity
{
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public bool IsCompleted { get; set; } = false;

  public int UserId { get; set; }
  public User? User { get; set; }
}
