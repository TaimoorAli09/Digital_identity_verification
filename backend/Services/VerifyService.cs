using System.Security.Cryptography;
using System.Text;

namespace backend.Services;

public class VerifyService
{
    private readonly RSA _rsa;

    public VerifyService()
    {
        // load PUBLIC key
        var publicKey =
            File.ReadAllText(
                "keys/public.pem"
            );

        _rsa = RSA.Create();

        _rsa.ImportFromPem(
    publicKey.ToCharArray()
);
    }

    public bool VerifySignature(
        string data,
        string signatureString
    )
    {
        try
        {
            // convert text into bytes
            var dataBytes =
                Encoding.UTF8
                .GetBytes(data);

            // convert Base64 signature
            // from QR into byte[]
            var signatureBytes =
                Convert
                .FromBase64String(
                    signatureString
                );

            // verify RSA signature
            return _rsa.VerifyData(
                dataBytes,
                signatureBytes,
                HashAlgorithmName
                .SHA256,
                RSASignaturePadding
                .Pkcs1
            );
        }
        catch
        {
            return false;
        }
    }
}