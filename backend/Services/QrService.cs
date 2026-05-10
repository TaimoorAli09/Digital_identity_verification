using QRCoder;
using System.Drawing;

namespace backend.Services;

public class QrService
{
    public byte[] GenerateQr(string url)
    {
        using var generator = new QRCodeGenerator();

        var data = generator.CreateQrCode(
            url,
            QRCodeGenerator.ECCLevel.Q
        );

        var qrCode = new PngByteQRCode(data);

        return qrCode.GetGraphic(20);
    }
}