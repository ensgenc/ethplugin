using System.Collections;
using System.Collections.Generic;
using ZXing;
using ZXing.QrCode;
using UnityEngine;
using UnityEngine.UI;

public class QRCodeDisplay : MonoBehaviour
{
    public RawImage QRImage;
    public Vector2 size;

    public void RenderQRCode(string address)
    {
        QRImage.texture = generateQR(address);
    }

    public Texture2D generateQR(string text)
    {
        var encoded = new Texture2D((int)size.x, (int)size.y);
        var color32 = Encode(text, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();
        return encoded;
    }

    private static Color32[] Encode(string textForEncoding,
      int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }

}
