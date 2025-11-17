namespace Application.Exceptions;

public class UserAlreadyExistsException(string message) : Exception(message) {
  
}
