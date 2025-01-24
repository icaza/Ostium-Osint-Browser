using System.IO;
using System.Security.Cryptography;

public class FileEnCryptor
{
    private const int KeySize = 256;
    private const int BlockSize = 128;
    private const int SaltSize = 32;
    private const int IvSize = 16;
    private const int Iterations = 100000;

    public static void EncryptFile(string inputFile, string outputFile, string password)
    {
        byte[] salt = GenerateRandomBytes(SaltSize);
        byte[] iv = GenerateRandomBytes(IvSize);
        byte[] key = DeriveKey(password, salt);

        using (Aes aes = Aes.Create())
        {
            aes.KeySize = KeySize;
            aes.BlockSize = BlockSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = iv;

            using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                fsOutput.Write(salt, 0, salt.Length);
                fsOutput.Write(iv, 0, iv.Length);

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                using (CryptoStream cs = new CryptoStream(fsOutput, encryptor, CryptoStreamMode.Write))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = fsInput.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        cs.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }
    }

    public static void DecryptFile(string inputFile, string outputFile, string password)
    {
        byte[] salt = new byte[SaltSize];
        byte[] iv = new byte[IvSize];

        using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
        {
            fsInput.Read(salt, 0, salt.Length);
            fsInput.Read(iv, 0, iv.Length);

            byte[] key = DeriveKey(password, salt);

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = KeySize;
                aes.BlockSize = BlockSize;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = key;
                aes.IV = iv;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                using (CryptoStream cs = new CryptoStream(fsInput, decryptor, CryptoStreamMode.Read))
                using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = cs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fsOutput.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }
    }

    private static byte[] GenerateRandomBytes(int length)
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] randomBytes = new byte[length];
            rng.GetBytes(randomBytes);
            return randomBytes;
        }
    }

    private static byte[] DeriveKey(string password, byte[] salt)
    {
        using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations))
        {
            return deriveBytes.GetBytes(KeySize / 8);
        }
    }
}