// Services/TodoService.cs
using System.Security.Claims;
using AutoMapper;
using FirstWebApi.DTOs;
using FirstWebApi.Exceptions;
using FirstWebApi.Models;
using FirstWebApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FirstWebApi.Services;

public class TodoService(ITodoRepository todoRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper) : ITodoService
{
  private readonly ITodoRepository _todoRepository = todoRepository;
  private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
  private readonly IMapper _mapper = mapper;

  private int GetUserId()
  {
    var userIdClaim = _httpContextAccessor.HttpContext?.User
        .FindFirstValue(ClaimTypes.NameIdentifier) ?? "0";
    return int.Parse(userIdClaim ?? "0");
  }

  public async Task<ApiResponse<IEnumerable<TodoDto>>> GetAllTodosAsync(
      PaginationParams paginationParams,
      TodoFilterParams? filterParams = null
  )
  {
    int userId = GetUserId();

    var query = await _todoRepository.GetQueryableByUserIdAsync(userId);

    if (!string.IsNullOrWhiteSpace(filterParams?.SearchTerm))
    {
      var searchTerm = filterParams.SearchTerm.Trim().ToLower();
      query = query
          .Where(
              t => t.Title.ToLower().Contains(searchTerm) ||
              t.Description.ToLower().Contains(searchTerm)
          );
    }

    var totalCount = await query.CountAsync();

    query = query.OrderByDescending(t => t.CreatedAt);

    var todos = await query
        .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
        .Take(paginationParams.PageSize)
        .ToListAsync();

    var todoDtos = _mapper.Map<IEnumerable<TodoDto>>(todos);

    var pagination = new Pagination
    {
      Page = paginationParams.Page,
      PageSize = paginationParams.PageSize,
      TotalCount = totalCount
    };

    return ApiResponse<IEnumerable<TodoDto>>.SuccessPagedResponse(todoDtos, pagination);
  }

  public async Task<TodoDto> GetTodoByIdAsync(int id)
  {
    int userId = GetUserId();
    var todo = await _todoRepository.GetByIdAndUserIdAsync(id, userId);

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

    await _todoRepository.AddAsync(todo);
    await _todoRepository.SaveChangesAsync();

    return _mapper.Map<TodoDto>(todo);
  }

  public async Task<TodoDto> UpdateTodoAsync(int id, UpdateTodoDto updateTodoDto)
  {
    int userId = GetUserId();

    var todo = await _todoRepository.GetByIdAndUserIdAsync(id, userId) ??
        throw new NotFoundException($"Todo with id {id} not found");

    _mapper.Map(updateTodoDto, todo);

    todo.UpdatedAt = DateTime.UtcNow;

    _todoRepository.Update(todo);
    await _todoRepository.SaveChangesAsync();

    return _mapper.Map<TodoDto>(todo);
  }

  public async Task DeleteTodoAsync(int id)
  {
    int userId = GetUserId();

    var todo = await _todoRepository.GetByIdAndUserIdAsync(id, userId) ??
        throw new NotFoundException($"Todo with id {id} not found");

    _todoRepository.Remove(todo);

    await _todoRepository.SaveChangesAsync();
  }
}
