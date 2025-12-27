
using FirstWebApi.Data;
using FirstWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstWebApi.Repositories;

public class TodoRepository(ApplicationDbContext context) : ITodoRepository
{
  private readonly ApplicationDbContext _context = context;

  public Task<IQueryable<Todo>> GetQueryableByUserIdAsync(int userId)
  {
    var query = _context.Todos
        .Where(t => t.UserId == userId)
        .AsQueryable();

    return Task.FromResult(query);
  }

  public async Task<Todo?> GetByIdAndUserIdAsync(int id, int userId)
  {
    return await _context.Todos
        .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
  }

  public async Task<Todo> AddAsync(Todo todo)
  {
    await _context.Todos.AddAsync(todo);
    return todo;
  }

  public void Update(Todo todo)
  {
    _context.Todos.Update(todo);
  }

  public void Remove(Todo todo)
  {
    _context.Todos.Remove(todo);
  }

  public async Task SaveChangesAsync()
  {
    await _context.SaveChangesAsync();
  }
}
