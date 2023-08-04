using LoopringSmartWalletRecoveryPhraseExtractor;
using Nethereum.HdWallet;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        QrCodeJson qrCodeJson = null;
        string loopringMigrationCodeQrText = ""; //the text of your Loopring migration qr code in JSON Format, eg: {"wallet":"0x99","iv":"2IcZe","mnemonic":"uvkZ","ens":"fudgey.loopring.eth","isCounterFactual":false,"register":"61,","type":"LoopringWalletSmart","setting":3232,"salt":"ikq","network":"ETHEREUM"}
        while (qrCodeJson == null)
        {
            Console.WriteLine("Enter your Loopring Migration QR Code Text in JSON Format: ");
            loopringMigrationCodeQrText = Console.ReadLine().Trim(); ;
            try
            {
                qrCodeJson = JsonConvert.DeserializeObject<QrCodeJson>(loopringMigrationCodeQrText);
            }
            catch (JsonException)
            {
                Console.WriteLine("You have entered invalid Loopring Migration QR Code Text in JSON Format! Try Again...");
            }
        }

        string loopringAppPassCode = ""; //change this to suit your Loopring passcode
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
                    catch(Exception)
                    {
                        Console.WriteLine("You have entered the wrong passcode... try again!");
                    }
                }
            }
        }
    }
}