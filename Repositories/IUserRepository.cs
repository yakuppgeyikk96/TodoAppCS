using FirstWebApi.Models;

namespace FirstWebApi.Repositories;

public interface IUserRepository
{
  Task<bool> ExistsByUsernameAsync(string username);
  Task<bool> ExistsByEmailAsync(string email);
  Task<User?> GetByUsernameAsync(string username);
  Task<User> AddAsync(User user);
  Task SaveChangesAsync();
}
