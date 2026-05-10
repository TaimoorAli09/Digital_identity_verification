using System.Security.Cryptography;
using System.Text;

namespace backend.Services;

public class HashService
{
    public string GenerateHash(string data)
    {
        using var sha256 = SHA256.Create();

        var bytes = sha256.ComputeHash(
            Encoding.UTF8.GetBytes(data)
        );

        return Convert.ToHexString(bytes);
    }
}