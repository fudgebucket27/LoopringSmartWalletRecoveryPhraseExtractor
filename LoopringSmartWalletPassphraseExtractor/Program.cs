using LoopringSmartWalletRecoveryPhraseExtractor;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        string migrationCodeQrText = ""; //the text of your Loopring migration qr code in JSON Format, eg: {"wallet":"0x99","iv":"2IcZe","mnemonic":"uvkZ","ens":"fudgey.loopring.eth","isCounterFactual":false,"register":"61,","type":"LoopringWalletSmart","setting":3232,"salt":"ikq","network":"ETHEREUM"}
        string passcode = ""; //change this to suit your Loopring passcode

        var qrTextObject = JsonConvert.DeserializeObject<QrCodeJson>(migrationCodeQrText);
        byte[] mnemonic = Encoding.ASCII.GetBytes(qrTextObject.mnemonic);
        byte[] iv = Encoding.ASCII.GetBytes(qrTextObject.iv);
        byte[] salt = Encoding.ASCII.GetBytes(qrTextObject.salt);

        QRCodeDecrypt(ref mnemonic, ref iv, ref salt, passcode);

        Console.WriteLine("Press any key to exit:");
        Console.ReadKey();

        static void QRCodeDecrypt(ref byte[] mnemonic, ref byte[] iv, ref byte[] salt, string passphrase)
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
                    byte[] result = new byte[decryptedBytes.Length - paddingLength];
                    Array.Copy(decryptedBytes, result, result.Length);
                    string decryptedMnemonic = Encoding.UTF8.GetString(result);
                    Console.WriteLine("Your recovery phrase is: " + decryptedMnemonic);
                }
            }
        }
    }
}