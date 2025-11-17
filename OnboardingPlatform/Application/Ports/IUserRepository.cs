namespace Application.Ports;

using Domain;

public interface IUserRepository {
  Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);
  Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
  Task<PagedResult<User>> GetAllAsync(int? pointer, int pageSize, CancellationToken cancellationToken = default);
  void Add(User user);
}
