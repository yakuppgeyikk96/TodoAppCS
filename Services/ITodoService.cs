using FirstWebApi.DTOs;
using FirstWebApi.Models;

namespace FirstWebApi.Services;

public interface ITodoService
{
  Task<ApiResponse<IEnumerable<TodoDto>>> GetAllTodosAsync(
      PaginationParams paginationParams,
      TodoFilterParams? filterParams = null);
  Task<TodoDto> GetTodoByIdAsync(int id);
  Task<TodoDto> CreateTodoAsync(CreateTodoDto createTodoDto);
  Task<TodoDto> UpdateTodoAsync(int id, UpdateTodoDto updateTodoDto);
  Task DeleteTodoAsync(int id);
}
