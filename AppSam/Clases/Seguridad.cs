using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AppSam.Clases
{
    public class Seguridad
    {
        public static string KeyRand()
        {
            Random rnd = new Random();
            int CodigoASC;
            int inicial;
            string Pass = "";

            for (int i = 0; i <= 7; i++)
            {
                inicial = rnd.Next(1, 4);

                CodigoASC = inicial == 1 ? rnd.Next(48, 58) :
                    inicial == 2 ? rnd.Next(65, 91) : rnd.Next(97, 123);
                Pass += ((char)CodigoASC).ToString();
            }
            return Pass;
        }

        // Codigo obtenido de 
        // http://www.codeproject.com/Tips/1156169/Encrypt-Strings-with-Passwords-AES-SHA
        //License
        //This article, along with any associated source code and files, is licensed under The Code Project Open License(CPOL)
        //http://www.codeproject.com/info/cpol10.aspx
        //"Licencia de Proyecto de Codigo Abierto"
        //About the Author
        //APE-Germany
        //Software Developer(Junior)
        //Germany Germany
        #region Encriptar & desencriptar Contraseña 
        public static string EncryptKey(string cadena, string key)
        {
            string encData = null;
            byte[][] keys = GetHashKeys(key);

            try
            {
                encData = EncryptStringToBytes_Aes(cadena, keys[0], keys[1]);
            }
            catch (CryptographicException) { }
            catch (ArgumentNullException) { }

            return encData;

        }

        public static string DecryptKey(string clave, string key)
        {

            string decData = null;
            byte[][] keys = GetHashKeys(key);

            try
            {
                decData = DecryptStringFromBytes_Aes(clave, keys[0], keys[1]);
            }
            catch (CryptographicException) { }
            catch (ArgumentNullException) { }

            return decData;
        }

        private static byte[][] GetHashKeys(string key)
        {
            byte[][] result = new byte[2][];
            Encoding enc = Encoding.UTF8;

            SHA256 sha2 = new SHA256CryptoServiceProvider();

            byte[] rawKey = enc.GetBytes(key);
            byte[] rawIV = enc.GetBytes(key);

            byte[] hashKey = sha2.ComputeHash(rawKey);
            byte[] hashIV = sha2.ComputeHash(rawIV);

            Array.Resize(ref hashIV, 16);

            result[0] = hashKey;
            result[1] = hashIV;

            return result;
        }
        //source: https://msdn.microsoft.com/de-de/library/system.security.cryptography.aes(v=vs.110).aspx
        private static string EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            byte[] encrypted;

            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt =
                            new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        //source: https://msdn.microsoft.com/de-de/library/system.security.cryptography.aes(v=vs.110).aspx
        private static string DecryptStringFromBytes_Aes(string cipherTextString, byte[] Key, byte[] IV)
        {
            byte[] cipherText = Convert.FromBase64String(cipherTextString);

            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt =
                            new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
        #endregion

        /// <summary>
        /// Funcion para Convertir un Stream en una cadena de Bytes 
        /// </summary>
        /// <param name="stream">Stream a convertir</param>
        /// <param name="tamaño">Tamaño del Stream </param>
        /// <returns>Regresamos una Cadena de Bytes</returns>
        public static byte[] ConvertirStream(Stream stream, int tamaño)
        {
            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(stream))
            {
                fileData = binaryReader.ReadBytes(tamaño);
                return fileData;
            }
        }
        
    }
}