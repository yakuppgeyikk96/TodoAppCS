

using FirstWebApi.Data;
using FirstWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstWebApi.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
  private readonly ApplicationDbContext _context = context;

  public async Task<User> AddAsync(User user)
  {
    await _context.Users.AddAsync(user);
    return user;
  }

  public async Task<bool> ExistsByEmailAsync(string email)
  {
    return await _context.Users.AnyAsync(
      record => record.Email == email
    );
  }

  public async Task<bool> ExistsByUsernameAsync(string username)
  {
    return await _context.Users.AnyAsync(
      record => record.Username == username
    );
  }

  public async Task<User?> GetByUsernameAsync(string username)
  {
    return await _context.Users.FirstOrDefaultAsync(
      record => record.Username == username
    );
  }

  public async Task SaveChangesAsync()
  {
    await _context.SaveChangesAsync();
  }
}
