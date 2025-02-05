
namespace DownloaderVideo.Domain.Interface.Utils;

public interface IGenerateHash
{
    /// <summary>
    /// Generates a random number.
    /// </summary>
    /// <returns>A random integer.</returns>
    int GenerateRandomNumber();

    /// <summary>
    /// Generates a random hash string.
    /// </summary>
    /// <returns>A random hash string.</returns>
    string GenerateHashRandom();

    /// <summary>
    /// Generates a hash from the given password.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>The hashed password.</returns>
    string GenerateHashParameters(string password);

    /// <summary>
    /// Verifies if the given password matches the hashed password.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="hashedPassword">The hashed password to compare against.</param>
    /// <returns>True if the password matches the hashed password, otherwise false.</returns>
    bool VerifyPassword(string password, string hashedPassword);

}
