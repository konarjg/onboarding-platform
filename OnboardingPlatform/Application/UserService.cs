namespace Application;

using Domain;
using Exceptions;
using Interfaces;
using Ports;

public class UserService(
  IUserRepository userRepository,
  IPasswordHasher passwordHasher,
  IUnitOfWork unitOfWork
) : IUserService {

  public async Task<User> RegisterAsync(string email, string password, UserRole role, CancellationToken cancellationToken = default) {
    if (await userRepository.GetByEmailAsync(email, cancellationToken) is not null) {
      throw new UserAlreadyExistsException($"User with email: {email} already exists.");
    }

    string passwordHash = passwordHasher.HashPassword(password);

    User newUser = new User {
      Email = email,
      PasswordHash = passwordHash,
      Role = role,
      IsActive = true
    };

    userRepository.Add(newUser);
    await unitOfWork.SaveChangesAsync(cancellationToken);

    return newUser;
  }

  public async Task DeactivateUserAsync(int userId, CancellationToken cancellationToken = default) {
    User? user = await userRepository.GetByIdAsync(userId, cancellationToken);
    if (user is null) {
      throw new UserNotFoundException($"User with id: {userId} not found.");
    }

    user.IsActive = false;
    await unitOfWork.SaveChangesAsync(cancellationToken);
  }
  public async Task<User?> UpdateUserRoleAsync(int userId,
    UserRole role,
    CancellationToken cancellationToken = default) {
    
    User? user = await userRepository.GetByIdAsync(userId, cancellationToken);
    if (user is null) {
      throw new UserNotFoundException($"User with id: {userId} not found.");
    }

    user.Role = role;
    
    await unitOfWork.SaveChangesAsync(cancellationToken);

    return user;
  }

  public async Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default) {
    return await userRepository.GetByIdAsync(userId, cancellationToken);
  }

  public async Task<PagedResult<User>> GetAllUsersAsync(int? pointer, int pageSize, CancellationToken cancellationToken = default) {
    return await userRepository.GetAllAsync(pointer, pageSize, cancellationToken);
  }
}
