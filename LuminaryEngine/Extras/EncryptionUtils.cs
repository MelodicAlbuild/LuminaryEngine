using System.Security.Cryptography;
using System.Text;

namespace LuminaryEngine.Extras;

public static class EncryptionUtils
{
    private const int SaltSize = 16; // 128-bit salt
    private const int KeySize = 32; // 256-bit key
    private const int IvSize = 12; // 96-bit IV (recommended for AES-GCM)
    private const int TagSize = 16; // 128-bit authentication tag
    private const int Iterations = 10000; // PBKDF2 iteration count

    /// <summary>
    /// Encrypts the plaintext using AES-GCM with a password-derived key.
    /// </summary>
    public static string Encrypt(string plainText, string password)
    {
        using AesGcm aes = new AesGcm(DeriveKeyFromPassword(password, out byte[] salt));
        byte[] iv = new byte[IvSize];
        RandomNumberGenerator.Fill(iv);

        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cipherBytes = new byte[plainBytes.Length];
        byte[] tag = new byte[TagSize];

        aes.Encrypt(iv, plainBytes, cipherBytes, tag);

        // Combine salt, IV, ciphertext, and tag for output
        using MemoryStream memoryStream = new();
        memoryStream.Write(salt, 0, salt.Length);
        memoryStream.Write(iv, 0, iv.Length);
        memoryStream.Write(cipherBytes, 0, cipherBytes.Length);
        memoryStream.Write(tag, 0, tag.Length);

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    /// <summary>
    /// Decrypts the ciphertext using AES-GCM with a password-derived key.
    /// </summary>
    public static string Decrypt(string encryptedText, string password)
    {
        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

        using MemoryStream memoryStream = new(encryptedBytes);
        byte[] salt = new byte[SaltSize];
        memoryStream.Read(salt, 0, salt.Length);

        byte[] iv = new byte[IvSize];
        memoryStream.Read(iv, 0, iv.Length);

        byte[] cipherBytes = new byte[encryptedBytes.Length - SaltSize - IvSize - TagSize];
        memoryStream.Read(cipherBytes, 0, cipherBytes.Length);

        byte[] tag = new byte[TagSize];
        memoryStream.Read(tag, 0, tag.Length);

        using AesGcm aes = new AesGcm(DeriveKeyFromPassword(password, salt));
        byte[] plainBytes = new byte[cipherBytes.Length];
        aes.Decrypt(iv, cipherBytes, tag, plainBytes);

        return Encoding.UTF8.GetString(plainBytes);
    }

    /// <summary>
    /// Derives a 256-bit key from the provided password and generates a salt.
    /// </summary>
    private static byte[] DeriveKeyFromPassword(string password, out byte[] salt)
    {
        salt = new byte[SaltSize];
        RandomNumberGenerator.Fill(salt);
        return DeriveKeyFromPassword(password, salt);
    }

    /// <summary>
    /// Derives a 256-bit key from the provided password and salt.
    /// </summary>
    private static byte[] DeriveKeyFromPassword(string password, byte[] salt)
    {
        using var keyDerivationFunction = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        return keyDerivationFunction.GetBytes(KeySize);
    }
}