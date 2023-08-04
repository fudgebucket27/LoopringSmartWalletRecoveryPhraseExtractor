using LoopringSmartWalletRecoveryPhraseExtractor;
using Nethereum.HdWallet;
using Newtonsoft.Json;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Enter the path to the image:");
        string imagePath = Console.ReadLine();

        try
        {
            using (Bitmap bitmap = new Bitmap(imagePath))
            {
                ZXing.Result result = result = DecodeQRCode(bitmap);

                if (result == null)
                {
                    Bitmap croppedBitmap = CropMiddleSection(bitmap);
                    Bitmap detectedBitmap = Detect(croppedBitmap);
                    result = DecodeQRCode(detectedBitmap);
                }

                if (result != null)
                {
                    ProcessResult(result);
                    QrCodeJson qrCodeJson = ProcessQRCodeJson(result.Text);
                    string loopringAppPassCode = GetLoopringAppPassCode();
                    DecryptRecoveryPhrase(qrCodeJson, loopringAppPassCode);
                }
                else
                {
                    Console.WriteLine("No QR code found in the image.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        Console.WriteLine("Press any key to exit:");
        Console.ReadKey();
    }

    private static Bitmap CropMiddleSection(Bitmap bitmap)
    {
        int heightToRemoveFromBottom = (int)(bitmap.Height * 0.4);
        int heightToRemoveFromTop = (int)(bitmap.Height * 0.1);

        // Determine the remaining middle section after removing 40% from the bottom and 10% from the top
        Rectangle middleSection = new Rectangle(0, heightToRemoveFromTop, bitmap.Width, bitmap.Height - heightToRemoveFromBottom - heightToRemoveFromTop);

        return bitmap.Clone(middleSection, bitmap.PixelFormat);
    }

    private static ZXing.Result DecodeQRCode(Bitmap bitmap)
    {
        ZXing.BarcodeReader reader = new ZXing.BarcodeReader
        {
            AutoRotate = true,
            TryInverted = true,
            Options = new ZXing.Common.DecodingOptions
            {
                TryHarder = true,
                PossibleFormats = new List<ZXing.BarcodeFormat> { ZXing.BarcodeFormat.QR_CODE }
            }
        };
        return reader.Decode(bitmap);
    }

    private static QrCodeJson ProcessQRCodeJson(string resultText)
    {
        return JsonConvert.DeserializeObject<QrCodeJson>(resultText);
    }

    private static string GetLoopringAppPassCode()
    {
        string loopringAppPassCode = "";
        while (string.IsNullOrEmpty(loopringAppPassCode))
        {
            Console.WriteLine("Enter your Loopring Wallet App Passcode: ");
            loopringAppPassCode = Console.ReadLine().Trim();
        }
        return loopringAppPassCode;
    }

    private static void DecryptRecoveryPhrase(QrCodeJson qrCodeJson, string passphrase)
    {
        byte[] mnemonic = Encoding.ASCII.GetBytes(qrCodeJson.mnemonic);
        byte[] iv = Encoding.ASCII.GetBytes(qrCodeJson.iv);
        byte[] salt = Encoding.ASCII.GetBytes(qrCodeJson.salt);

        QRCodeDecrypt(mnemonic, iv, salt, passphrase);
    }

    private static Bitmap Detect(Bitmap bitmap)
    {
        try
        {
            // Create luminance source
            var luminanceSource = new BitmapLuminanceSource(bitmap);

            // Create a binarizer
            var binarizer = new HybridBinarizer(luminanceSource);

            // Create a binary bitmap
            var binBitmap = new BinaryBitmap(binarizer);

            // Access the BlackMatrix
            BitMatrix bm = binBitmap.BlackMatrix;

            // Process the BitMatrix, e.g., detect QR code
            Detector detector = new Detector(bm);
            DetectorResult result = detector.detect();

            string retStr = "Found at points ";
            foreach (ResultPoint point in result.Points)
            {
                retStr += point.ToString() + ", ";
            }
            // Determine the bounding rectangle
            float minX = float.PositiveInfinity;
            float minY = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float maxY = float.NegativeInfinity;

            foreach (ResultPoint point in result.Points)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
            }

            // Expand the bounding rectangle upward by a fixed amount
            int padding = 40; // You can adjust this value as needed
            minY = Math.Max(minY - padding, 0); // Make sure not to go beyond the image bounds
            minX = Math.Max(minX - padding, 0); // Make sure not to go beyond the image bounds
            maxX = Math.Max(maxX + padding, 0); // Make sure not to go beyond the image bounds

            float qrCodeWidth = maxX - minX;

            // The bottom of the bounding rectangle will be minY plus the width of the QR code
            maxY = minY + qrCodeWidth;

            // Make sure maxY doesn't go beyond the image bounds
            maxY = Math.Min(maxY, bitmap.Height);

            // Create the bounding rectangle (you might want to add some padding)
            Rectangle boundingRectangle = Rectangle.FromLTRB((int)minX, (int)minY, (int)maxX, (int)maxY);

            // Crop the QR code region
            Bitmap croppedBitmap = bitmap.Clone(boundingRectangle, bitmap.PixelFormat);

            return croppedBitmap;
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message);
            return null;
        }
    }

    private static void ProcessResult(ZXing.Result result)
    {
        if (result != null)
        {
            Console.WriteLine($"Decoded QR code text: {result.Text}");
        }
        else
        {
            Console.WriteLine("No QR code found in the image.");
        }
    }

    static void QRCodeDecrypt(byte[] mnemonic, byte[] iv, byte[] salt, string passphrase)
    {
        mnemonic = Convert.FromBase64String(Encoding.UTF8.GetString(mnemonic));
        iv = Convert.FromBase64String(Encoding.UTF8.GetString(iv));
        salt = Convert.FromBase64String(Encoding.UTF8.GetString(salt));

        string password = string.Format("0x{0}", BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("LOOPRING HEBAO Wallet " + passphrase))).Replace("-", "").ToLower());
        byte[] key = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), salt, 4096, HashAlgorithmName.SHA256).GetBytes(32);

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;

            using (ICryptoTransform decryptor = aes.CreateDecryptor())
            {
                byte[] decryptedBytes = decryptor.TransformFinalBlock(mnemonic, 0, mnemonic.Length);
                int paddingLength = decryptedBytes[decryptedBytes.Length - 1];
                try
                {
                    byte[] result = new byte[decryptedBytes.Length - paddingLength];
                    Array.Copy(decryptedBytes, result, result.Length);
                    string decryptedMnemonic = Encoding.UTF8.GetString(result);
                    Console.WriteLine("Your recovery phrase for your owner wallet is: " + decryptedMnemonic);
                    Wallet wallet = new Wallet(decryptedMnemonic, null);
                    string walletPrivateKey = BitConverter.ToString(wallet.GetPrivateKey(0)).Replace("-", string.Empty).ToLower();
                    Console.WriteLine("Your L1 private key for your owner wallet is: " + walletPrivateKey);

                }
                catch (Exception)
                {
                    Console.WriteLine("You have entered the wrong passcode... try again!");
                }
            }
        }
    }
}
