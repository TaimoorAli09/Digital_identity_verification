using System.Security.Cryptography;
using System.Text;

namespace backend.Services;

public class SignatureService
{
    private readonly RSA _rsa;

    public SignatureService()
    {
        var privateKey = File.ReadAllText("keys/private.pem");

        _rsa = RSA.Create();
        _rsa.ImportFromPem(privateKey);
    }

    public byte[] SignData(string data)
    {
        var dataBytes = Encoding.UTF8.GetBytes(data);

        return _rsa.SignData(
            dataBytes,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );
    }
}