
using FirstWebApi.DTOs;
using FirstWebApi.Models;

namespace FirstWebApi.Repositories;

public interface ITodoRepository
{
  Task<IQueryable<Todo>> GetQueryableByUserIdAsync(int userId);
  Task<Todo?> GetByIdAndUserIdAsync(int id, int userId);
  Task<Todo> AddAsync(Todo todo);
  void Update(Todo todo);
  void Remove(Todo todo);
  Task SaveChangesAsync();
}
