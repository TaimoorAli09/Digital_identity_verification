using QRCoder;
using System.Drawing;

namespace backend.Services;

public class QrService
{
    public byte[] GenerateQr(
        string text)
    {
        using var generator =
            new QRCodeGenerator();

        var data =
            generator.CreateQrCode(
                text,
                QRCodeGenerator
                .ECCLevel.M
            );

        var qrCode =
            new PngByteQRCode(
                data
            );

        return qrCode.GetGraphic(

            pixelsPerModule: 8,

            darkColor:
                Color.Black,

            lightColor:
                Color.White,

            drawQuietZones:
                true
        );
    }
}