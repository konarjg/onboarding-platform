namespace Application.Interfaces;

using Domain;

public interface IUserService {
  Task<User> RegisterAsync(string email, string password, UserRole role, CancellationToken cancellationToken = default);
  Task DeactivateUserAsync(int userId, CancellationToken cancellationToken = default);
  Task<User?> UpdateUserRoleAsync(int userId,
    UserRole role,
    CancellationToken cancellationToken = default);
  Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);
  Task<PagedResult<User>> GetAllUsersAsync(int? pointer, int pageSize, CancellationToken cancellationToken = default);
}
