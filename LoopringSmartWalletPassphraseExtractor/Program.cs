using LoopringSmartWalletRecoveryPhraseExtractor;
using Nethereum.HdWallet;
using Newtonsoft.Json;
using OpenCvSharp;
using System.Security.Cryptography;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        QrCodeJson qrCodeJson = null;

        // Path to the image
        string imagePath = "";
        while (string.IsNullOrEmpty(imagePath))
        {
            Console.WriteLine("Enter the file path to the QR code image: ");
            imagePath = Console.ReadLine().Trim(); ;
        }

        // Read the image from the file
        using Mat image = Cv2.ImRead(imagePath);

        // Initialize the QR code detector
        QRCodeDetector qrCodeDetector = new QRCodeDetector();

        // Detect the QR code
        string decodedInfo = qrCodeDetector.DetectAndDecode(image, out Point2f[] points);

        // Check if the QR code has been detected
        if (points != null && points.Length > 0)
        {
            // Draw a polygon around the detected QR code
            Cv2.Polylines(image, new Point[][] { points.Select(p => p.ToPoint()).ToArray() }, isClosed: true, color: Scalar.Red);

            // Display the detected and decoded information
            Console.WriteLine("QR Code Detected: " + decodedInfo);
            if (decodedInfo.Trim().Length > 0)
            {
                qrCodeJson = JsonConvert.DeserializeObject<QrCodeJson>(decodedInfo);
            }
        }
        else
        {
            Console.WriteLine("QR Code Not Detected. Try a different image...");
            return;
        }


        string loopringAppPassCode = "";
        while (string.IsNullOrEmpty(loopringAppPassCode))
        {
            Console.WriteLine("Enter your Loopring Wallet App Passcode: ");
            loopringAppPassCode = Console.ReadLine().Trim();
        }


        byte[] mnemonic = Encoding.ASCII.GetBytes(qrCodeJson.mnemonic);
        byte[] iv = Encoding.ASCII.GetBytes(qrCodeJson.iv);
        byte[] salt = Encoding.ASCII.GetBytes(qrCodeJson.salt);

        QRCodeDecrypt(mnemonic, iv, salt, loopringAppPassCode);

        Console.WriteLine("Press any key to exit:");
        Console.ReadKey();
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
                        Console.WriteLine("Your recovery phrase for your owner wallet is: " + Environment.NewLine + decryptedMnemonic);
                        Wallet wallet = new Wallet(decryptedMnemonic, null);
                        string walletPrivateKey = BitConverter.ToString(wallet.GetPrivateKey(0)).Replace("-", string.Empty).ToLower();
                        Console.WriteLine("Your L1 private key for your owner wallet is: "  + Environment.NewLine +  walletPrivateKey);

                    }
                    catch(Exception)
                    {
                        Console.WriteLine("You have entered the wrong passcode... try again!");
                    }
                }
            }
        }
    }
