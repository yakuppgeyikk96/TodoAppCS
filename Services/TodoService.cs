using FirstWebApi.Data;
using FirstWebApi.DTOs;
using FirstWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstWebApi.Services;

public class TodoService(ApplicationDbContext context, ILogger<TodoService> logger) : ITodoService
{
  private readonly ApplicationDbContext _context = context;
  private readonly ILogger<TodoService> _logger = logger;

  public async Task<IEnumerable<TodoDto>> GetAllTodosAsync()
  {
    var todos = await _context.Todos
        .OrderByDescending(t => t.CreatedAt)
        .ToListAsync();

    return todos.Select(MapToDto);
  }

  public async Task<TodoDto?> GetTodoByIdAsync(int id)
  {
    var todo = await _context.Todos.FindAsync(id);
    return todo != null ? MapToDto(todo) : null;
  }

  public async Task<TodoDto> CreateTodoAsync(CreateTodoDto createTodoDto)
  {
    var todo = new Todo
    {
      Title = createTodoDto.Title,
      Description = createTodoDto.Description,
      IsCompleted = false,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = null
    };

    _context.Todos.Add(todo);
    await _context.SaveChangesAsync();

    _logger.LogInformation("Todo created with ID: {TodoId}", todo.Id);

    return MapToDto(todo);
  }

  public async Task<TodoDto?> UpdateTodoAsync(int id, UpdateTodoDto updateTodoDto)
  {
    var todo = await _context.Todos.FindAsync(id);
    if (todo == null)
      return null;

    todo.Title = updateTodoDto.Title;
    todo.Description = updateTodoDto.Description;
    todo.IsCompleted = updateTodoDto.IsCompleted;
    todo.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    _logger.LogInformation("Todo updated with ID: {TodoId}", id);

    return MapToDto(todo);
  }

  public async Task<bool> DeleteTodoAsync(int id)
  {
    var todo = await _context.Todos.FindAsync(id);
    if (todo == null)
      return false;

    _context.Todos.Remove(todo);
    await _context.SaveChangesAsync();

    _logger.LogInformation("Todo deleted with ID: {TodoId}", id);

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
