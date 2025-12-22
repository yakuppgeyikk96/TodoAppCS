namespace FirstWebApi.DTOs;

public class TodoFilterParams
{
  public bool? IsCompleted { get; set; }
  public string? SearchTerm { get; set; }
}
