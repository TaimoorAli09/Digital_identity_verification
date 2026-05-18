using System.Security.Cryptography;
using System.Text;

namespace backend.Services;

public class VerifyService
{
    private readonly RSA _rsa;

    public VerifyService()
    {
        // Load PUBLIC key (safe to expose)
        var publicKey = File.ReadAllText("keys/public.pem");

        _rsa = RSA.Create();
        _rsa.ImportFromPem(publicKey);
    }

    public bool VerifySignature(string data, byte[] signature)
    {
        try
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);

            return _rsa.VerifyData(
                dataBytes,
                signature,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );
        }
        catch
        {
            return false;
        }
    }
}