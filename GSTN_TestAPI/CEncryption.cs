﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GSTN_TestAPI
{
    public class CEncryption
    {

        public static X509Certificate2 getPublicKey()
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            X509Certificate2 cert2 = new X509Certificate2(@"C:\Users\Vikas\Downloads\GSTN_PublicKey.cer");
            return cert2;
        }


        public byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            
            using (MemoryStream ms = new MemoryStream())
            {
                
                using (AesCryptoServiceProvider AES = new AesCryptoServiceProvider())
                {
                    AES.Key = passwordBytes;
                    AES.BlockSize = 128;
                    AES.Mode = CipherMode.ECB;
                    AES.Padding = PaddingMode.PKCS7;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }



        private string HMAC_Encrypt(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }

            
        }

        public byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.Key = passwordBytes;
                    AES.BlockSize = 128;
                    AES.Mode = CipherMode.ECB;
                    AES.Padding = PaddingMode.PKCS7;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        public string Encrypt(string plainText, string key)
        {
            byte[] dataToEncrypt = UTF8Encoding.UTF8.GetBytes(plainText);
            AesManaged tdes = new AesManaged();
            tdes.KeySize = 256;
            tdes.BlockSize = 128;
            //if (tdes.ValidKeySize(key.Length))
            //{
            //tdes.Key = Convert.FromBase64String(key);
            tdes.Key = Encoding.ASCII.GetBytes(key);
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform crypt = tdes.CreateEncryptor();
            byte[] cipher = crypt.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            tdes.Clear();
            return Convert.ToBase64String(cipher, 0, cipher.Length);
          
        }




        public string EncryptText(string input, byte[] passwordBytes)
        {
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
            //passwordBytes = HMACSHA256.Create().ComputeHash(passwordBytes);
            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);
            string result = Convert.ToBase64String(bytesEncrypted);
            return result;
        }


        public string EncryptTextWithPublicKey(string input)
        {
            X509Certificate2 certificate = getPublicKey();
            RSACryptoServiceProvider RSA = (RSACryptoServiceProvider)certificate.PublicKey.Key;
           // RSA.KeySize = 256;

            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
            byte[] bytesEncrypted = RSA.Encrypt(bytesToBeEncrypted, false);

            string result = Convert.ToBase64String(bytesEncrypted);
            return result;

        }


        public string DecryptText(string input, string password)
        {
            // Get the bytes of the string
            byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            string result = Encoding.UTF8.GetString(bytesDecrypted);

            return result;
        }

    }


}
