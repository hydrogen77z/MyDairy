using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MyDairy.Common;
using MyDairy.Models;
using MyDairy.Serialization;

namespace MyDairy.Helpers;

public static class EncryptHelper
{
    public const int SaltSize = 16;
    public const int NonceSize = 12;
    public const int TagSize = 16;
    public const int Iterations = 10_0000;

    public static string EncryptToBase64(string jsonText, string password, byte[] salt, byte[] nonce, byte[] tag)
    {
        var key = GetDeriveKey(password, salt);

        var cipherText = new byte[jsonText.Length];

        var aes = new AesGcm(key, TagSize);
        aes.Encrypt(nonce, Encoding.UTF8.GetBytes(jsonText), cipherText, tag);

        return Convert.ToBase64String(cipherText);
    }
    public static string DecryptFromBase64(string base64Cipher, string password, byte[] salt, byte[] nonce, byte[] tag)
    {
        var cipher = Convert.FromBase64String(base64Cipher);

        var key = GetDeriveKey(password, salt);
        var json = new byte[cipher.Length];
        var aes = new AesGcm(key, TagSize);
        aes.Decrypt(nonce, cipher, tag, json);

        return Encoding.UTF8.GetString(json);
    }
    private static byte[] GetDeriveKey(string password, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, Iterations, HashAlgorithmName.SHA256, 32);
    }
}
