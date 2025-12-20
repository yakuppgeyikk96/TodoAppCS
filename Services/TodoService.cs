using System.Security.Claims;
using AutoMapper;
using FirstWebApi.Data;
using FirstWebApi.DTOs;
using FirstWebApi.Exceptions;
using FirstWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstWebApi.Services;

public class TodoService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper) : ITodoService
{
  private readonly ApplicationDbContext _context = context;
  private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
  private readonly IMapper _mapper = mapper;

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

    return _mapper.Map<IEnumerable<TodoDto>>(todos);
  }

  public async Task<TodoDto> GetTodoByIdAsync(int id)
  {
    int userId = GetUserId();
    var todo = await _context.Todos
        .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    if (todo == null)
    {
      throw new NotFoundException($"Todo with id {id} not found");
    }

    return _mapper.Map<TodoDto>(todo);
  }

  public async Task<TodoDto> CreateTodoAsync(CreateTodoDto createTodoDto)
  {
    var todo = _mapper.Map<Todo>(createTodoDto);

    todo.UserId = GetUserId();
    todo.CreatedAt = DateTime.UtcNow;

    _context.Todos.Add(todo);

    await _context.SaveChangesAsync();

    return _mapper.Map<TodoDto>(todo);
  }

  public async Task<TodoDto> UpdateTodoAsync(int id, UpdateTodoDto updateTodoDto)
  {
    int userId = GetUserId();
    var todo = await _context.Todos
        .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    if (todo == null)
    {
      throw new NotFoundException($"Todo with id {id} not found");
    }

    _mapper.Map(updateTodoDto, todo);

    todo.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return _mapper.Map<TodoDto>(todo);
  }

  public async Task DeleteTodoAsync(int id)
  {
    int userId = GetUserId();

    var todo = await _context.Todos
        .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    if (todo == null)
    {
      throw new NotFoundException($"Todo with id {id} not found");
    }

    _context.Todos.Remove(todo);
    await _context.SaveChangesAsync();
  }

}
