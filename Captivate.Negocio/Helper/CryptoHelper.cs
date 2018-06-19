using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Captivate.Negocio.Helper
{
  public class CryptoHelper
  {
    public static string EncryptAES(string InputString)
    {
      string SecretKey = "*k1n4d5p4t3lbl1*";
      byte[] ByteKey = Encoding.UTF8.GetBytes(SecretKey);
      byte[] ByteIV = new byte[16];
      byte[] encrypted = EncryptStringToBytes_Aes(InputString, ByteKey, ByteIV);
      return Convert.ToBase64String(encrypted);
    }

    static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
    {
      // Check arguments. 
      if (plainText == null || plainText.Length <= 0)
        throw new ArgumentNullException("plainText");
      if (Key == null || Key.Length <= 0)
        throw new ArgumentNullException("Key");
      if (IV == null || IV.Length <= 0)
        throw new ArgumentNullException("Key");
      byte[] encrypted;
      using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
      {
        aesAlg.Key = Key;
        aesAlg.IV = IV;
        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        using (MemoryStream msEncrypt = new MemoryStream())
        {
          using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
          {
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
              swEncrypt.Write(plainText);
            }
            encrypted = msEncrypt.ToArray();
          }
        }
      }
      return encrypted;
    }


    public static string DecryptAES(string InputString)
    {
      string SecretKey = "*k1n4d5p4t3lbl1*";
      byte[] keybytes = Encoding.UTF8.GetBytes(SecretKey);
      byte[] cipheredData = Convert.FromBase64String(InputString);
      byte[] ByteIV = new byte[16];
      return DecryptStringFromBytes_Aes(cipheredData, keybytes, ByteIV);
    }

    static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
    {
      if (cipherText == null || cipherText.Length <= 0)
        throw new ArgumentNullException("cipherText");
      if (Key == null || Key.Length <= 0)
        throw new ArgumentNullException("Key");
      if (IV == null || IV.Length <= 0)
        throw new ArgumentNullException("Key");

      System.IO.MemoryStream msDecrypt = null;
      CryptoStream csDecrypt = null;
      StreamReader srDecrypt = null;

      string plaintext = null;
      try
      {
        using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
        {
          aesAlg.Key = Key;
          aesAlg.IV = IV;
          ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
          msDecrypt = new MemoryStream(cipherText);
          csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
          srDecrypt = new StreamReader(csDecrypt);
          plaintext = srDecrypt.ReadToEnd();
        }

      }
      finally
      {
        if (srDecrypt != null)
          srDecrypt.Close();
        if (csDecrypt != null)
          csDecrypt.Close();
        if (msDecrypt != null)
          msDecrypt.Close();
      }
      return plaintext;
    }
  }
}
