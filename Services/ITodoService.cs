using FirstWebApi.DTOs;

namespace FirstWebApi.Services;

public interface ITodoService
{
  Task<IEnumerable<TodoDto>> GetAllTodosAsync();
  Task<TodoDto?> GetTodoByIdAsync(int id);
  Task<TodoDto> CreateTodoAsync(CreateTodoDto createTodoDto);
  Task<TodoDto?> UpdateTodoAsync(int id, UpdateTodoDto updateTodoDto);
  Task<bool> DeleteTodoAsync(int id);
}
