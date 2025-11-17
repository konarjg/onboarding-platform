namespace Infrastructure;

using Application.Ports;
using Konscious.Security.Cryptography;
using System;
using System.Security.Cryptography;
using System.Text;

public class PasswordHasher : IPasswordHasher {
  private const int SALT_SIZE = 16;
  private const int HASH_SIZE = 32;
  private const int DEGREE_OF_PARALLELISM = 8;
  private const int ITERATIONS = 4;
  private const int MEMORY_SIZE_KB = 12288; 

  public string HashPassword(string password) {
    byte[] salt = RandomNumberGenerator.GetBytes(SALT_SIZE);
    byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

    using Argon2id argon2 = new Argon2id(passwordBytes) {
      Salt = salt,
      DegreeOfParallelism = DEGREE_OF_PARALLELISM,
      Iterations = ITERATIONS,
      MemorySize = MEMORY_SIZE_KB
    };

    byte[] hash = argon2.GetBytes(HASH_SIZE);
    byte[] combinedBytes = new byte[SALT_SIZE + HASH_SIZE];
    
    Buffer.BlockCopy(salt, 0, combinedBytes, 0, SALT_SIZE);
    Buffer.BlockCopy(hash, 0, combinedBytes, SALT_SIZE, HASH_SIZE);

    return Convert.ToBase64String(combinedBytes);
  }

  public bool VerifyPassword(string password, string hash) {
    try {
      byte[] combinedBytes = Convert.FromBase64String(hash);
      if (combinedBytes.Length != SALT_SIZE + HASH_SIZE) {
        return false;
      }

      byte[] salt = new byte[SALT_SIZE];
      byte[] originalHash = new byte[HASH_SIZE];

      Buffer.BlockCopy(combinedBytes, 0, salt, 0, SALT_SIZE);
      Buffer.BlockCopy(combinedBytes, SALT_SIZE, originalHash, 0, HASH_SIZE);

      byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
      
      using Argon2id argon2 = new Argon2id(passwordBytes) {
        Salt = salt,
        DegreeOfParallelism = DEGREE_OF_PARALLELISM,
        Iterations = ITERATIONS,
        MemorySize = MEMORY_SIZE_KB
      };
      
      byte[] newHash = argon2.GetBytes(HASH_SIZE);

      return CryptographicOperations.FixedTimeEquals(originalHash, newHash);
    }
    catch {
      return false;
    }
  }
}