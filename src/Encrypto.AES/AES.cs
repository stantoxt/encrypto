﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Encrypto.AESLibrary
{
    public class AES
    {
        public static byte[] GetEncryptedByteArray(byte[] encryptedBytes, byte[] password)
        {
            //the salt bytes must be at least 8 bytes
            byte[] saltBytes = new byte[] {1, 2, 3, 4, 5, 6, 7, 8};
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] encyptedbytes = null;
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    var key = new Rfc2898DeriveBytes(password, saltBytes, 1000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.Zeros;
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptedBytes, 0, encryptedBytes.Length);
                        cs.Close();
                    }

                    encyptedbytes = ms.ToArray();
                }

                return encyptedbytes;
            }
        }

        public static byte[] GetDecryptedByteArray(byte[] bytesDecrypted, byte[] password)
        {
            byte[] decrypted = null;


            byte[] saltBytes = new byte[] {1, 2, 3, 4, 5, 6, 7, 8};
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    var key = new Rfc2898DeriveBytes(password, saltBytes, 1000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.Zeros;

                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesDecrypted, 0, bytesDecrypted.Length);
                        cs.Close();
                    }

                    decrypted = ms.ToArray();
                }
            }

            return decrypted;
        }

        public static byte[] EncryptText(string inputText, string password)
        {
            ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

            try
            {
                locker.EnterReadLock();

                byte[] encryptedBytes = System.Text.Encoding.UTF8.GetBytes(inputText);
                byte[] passwordToByteArray = System.Text.Encoding.UTF8.GetBytes(password);
                passwordToByteArray = SHA256.Create().ComputeHash(passwordToByteArray);
                byte[] encryptedByteArray = AES.GetEncryptedByteArray(encryptedBytes, passwordToByteArray);

                return encryptedByteArray;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public static string EncryptFile(string inputFile, string outputFile, string password)
        {
            ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

            try
            {
                locker.EnterReadLock();

                byte[] encryptedBytes = File.ReadAllBytes(inputFile);
                byte[] passwordToByteArray = System.Text.Encoding.ASCII.GetBytes(password);

                //hash the password with sha256
                passwordToByteArray = SHA256.Create().ComputeHash(passwordToByteArray);

                byte[] encryptedByteArray = AES.GetEncryptedByteArray(encryptedBytes, passwordToByteArray);

                File.WriteAllBytes(outputFile, encryptedByteArray);
                return "encryption succeeded";
            }
            catch (Exception)
            {
                return "encryption failed";
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public static string DecryptFile(string inputFile, string outputFile, string password)
        {
            ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

            try
            {
                locker.EnterReadLock();

                byte[] bytesToBeDecrypted = File.ReadAllBytes(outputFile);
                byte[] passwordBytes = System.Text.Encoding.ASCII.GetBytes(password);
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                byte[] bytesDecrypted = AES.GetDecryptedByteArray(bytesToBeDecrypted, passwordBytes);

                File.WriteAllBytes(outputFile, bytesDecrypted);
                return "Decryption succeeded";
            }
            catch (Exception)
            {
                return "Decryption failed";
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public static string DecryptText(byte[] input, string password)
        {
            ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
            try
            {
                locker.EnterReadLock();
                byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
                byte[] bytesDecrypted = AES.GetDecryptedByteArray(input, passwordBytes);
                var result = System.Text.Encoding.UTF8.GetString(bytesDecrypted);
                return result;
            }
            catch (Exception)
            {
                return "Decryption failed";
            }
            finally
            {
                locker.ExitReadLock();
            }
        }
    }
}