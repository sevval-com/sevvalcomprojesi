using System.Security.Cryptography;
using System.Text;

namespace Sevval.Application.Utilities;

public static class Md5Service
{
    static string hash = "ucand38turNaGurbet38diyar38diyar38geZerOgullari";
    public static string ConvertTextToMd5(string Text)
    {
        MD5CryptoServiceProvider mD5 = new MD5CryptoServiceProvider();
        return EncryptionToMd5(Text, mD5);

    }

    private static string EncryptionToMd5(string text, HashAlgorithm alg)
    {
        byte[] byteValue = Encoding.UTF8.GetBytes(text);
        byte[] encryptByte = alg.ComputeHash(byteValue);
        return Convert.ToBase64String(encryptByte);
    }

    public static string Decrypt(string EncryptedText)
    {

        if (string.IsNullOrEmpty(EncryptedText)) return null;
        EncryptedText = EncryptedText.Substring(1);
        EncryptedText = EncryptedText.Replace("_", "+").Replace("-", "%").Replace("8hatec8", "/").Replace("4hatec4", "=");
        byte[] data = Convert.FromBase64String(EncryptedText);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            using (TripleDESCryptoServiceProvider triple = new TripleDESCryptoServiceProvider()
            {
                Key = keys,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            })
            {
                ICryptoTransform transform = triple.CreateDecryptor();
                byte[] result = transform.TransformFinalBlock(data, 0, data.Length);

                var text = UTF8Encoding.UTF8.GetString(result);


                return text;

            }
        }
    }

    public static string Encrypt(string Text)
    {
        if (string.IsNullOrEmpty(Text)) return null;



        byte[] data = UTF8Encoding.UTF8.GetBytes(Text);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            using (TripleDESCryptoServiceProvider triple = new TripleDESCryptoServiceProvider()
            {
                Key = keys,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7,

            })
            {
                ICryptoTransform transform = triple.CreateEncryptor();
                byte[] result = transform.TransformFinalBlock(data, 0, data.Length);

                return "a" + Convert.ToBase64String(result, 0, result.Length).Replace("+", "_").Replace("%", "-").Replace("=", "4hatec4").Replace("/", "8hatec8");

            }
        }
    }


}
