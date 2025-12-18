using System.Security.Claims;
using FirstWebApi.Data;
using FirstWebApi.DTOs;
using FirstWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstWebApi.Services;

public class TodoService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : ITodoService
{
  private readonly ApplicationDbContext _context = context;
  private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

  private int GetUserId()
  {
    var userIdClaim = _httpContextAccessor.HttpContext?.User
      .FindFirstValue(ClaimTypes.NameIdentifier) ?? "0";
    return int.Parse(userIdClaim ?? "0");
  }

  public async Task<IEnumerable<TodoDto>> GetAllTodosAsync()
  {
    int userId = GetUserId();

    var todos = await _context.Todos
      .Where(t => t.UserId == userId)
      .ToListAsync();

    return todos.Select(MapToDto);
  }

  public async Task<TodoDto?> GetTodoByIdAsync(int id)
  {
    int userId = GetUserId();
    var todo = await _context.Todos
        .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    return todo != null ? MapToDto(todo) : null;
  }

  public async Task<TodoDto> CreateTodoAsync(CreateTodoDto createTodoDto)
  {
    var todo = new Todo
    {
      Title = createTodoDto.Title,
      Description = createTodoDto.Description,
      UserId = GetUserId(),
      CreatedAt = DateTime.UtcNow
    };

    _context.Todos.Add(todo);
    await _context.SaveChangesAsync();
    return MapToDto(todo);
  }

  public async Task<TodoDto?> UpdateTodoAsync(int id, UpdateTodoDto updateTodoDto)
  {
    int userId = GetUserId();
    var todo = await _context.Todos
        .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    if (todo == null)
      return null;

    todo.Title = updateTodoDto.Title;
    todo.Description = updateTodoDto.Description;
    todo.IsCompleted = updateTodoDto.IsCompleted;
    todo.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return MapToDto(todo);
  }

  public async Task<bool> DeleteTodoAsync(int id)
  {
    int userId = GetUserId();

    var todo = await _context.Todos
        .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    if (todo == null)
      return false;

    _context.Todos.Remove(todo);
    await _context.SaveChangesAsync();

    return true;
  }

  private static TodoDto MapToDto(Todo todo)
  {
    return new TodoDto
    {
      Id = todo.Id,
      Title = todo.Title,
      Description = todo.Description,
      IsCompleted = todo.IsCompleted,
      CreatedAt = todo.CreatedAt,
      UpdatedAt = todo.UpdatedAt
    };
  }
}
