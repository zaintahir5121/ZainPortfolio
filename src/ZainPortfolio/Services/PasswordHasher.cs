using System.Security.Cryptography;
using System.Text;

namespace ZainPortfolio.Services;

/// <summary>
/// PBKDF2-SHA256 password hashing. No external identity dependency —
/// this app has a single admin account, so the full Identity stack is overkill.
/// </summary>
public static class PasswordHasher
{
    private const int SaltBytes = 16;
    private const int HashBytes = 32;
    private const int Iterations = 210_000;   // OWASP 2023 guidance for PBKDF2-SHA256

    public static (string Hash, string Salt) Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltBytes);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password), salt, Iterations, HashAlgorithmName.SHA256, HashBytes);
        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public static bool Verify(string password, string storedHash, string storedSalt)
    {
        try
        {
            var salt = Convert.FromBase64String(storedSalt);
            var expected = Convert.FromBase64String(storedHash);
            var actual = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password), salt, Iterations, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>Readable but strong first-run password, shown once in the startup log.</summary>
    public static string GenerateReadablePassword()
    {
        const string words = "Cloud,Azure,Vector,Neural,Cipher,Quantum,Falcon,Nimbus,Copper,Zenith";
        var parts = words.Split(',');
        var a = parts[RandomNumberGenerator.GetInt32(parts.Length)];
        var b = parts[RandomNumberGenerator.GetInt32(parts.Length)];
        var n = RandomNumberGenerator.GetInt32(1000, 9999);
        return $"{a}-{b}-{n}";
    }
}
