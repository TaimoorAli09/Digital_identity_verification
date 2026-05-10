using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;

public class TokenService
{
    public string GenerateToken()
    {
        // this will generate random numbers 
        // a byte can store 0--255 number 
        // we are creating 32 byte number = 256 bits ( extremely secure )

        var bytes = RandomNumberGenerator.GetBytes(32);


        
        //humans / web URLs need text.

        // So we encode bytes into Base64( human readalbe format).
        return Convert.ToBase64String(bytes)
            // important to replace bcs they will create issue in url remeber them as it is 
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}