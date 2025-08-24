using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace File_Organizer__Command_Line_Tool___5
{
    class AesEncryptionAndDecryptionService
    {
        byte[] _key= Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes = 128-bit key 
        string _myInputFileName;
        string _myEncryptedFileName;
        string _myDecryptedFileName;
        string _myFolderName;
        public AesEncryptionAndDecryptionService(string inputFileName,string encryptedFileName, string decryptedFileName, string myFolderName)
        {
            _myInputFileName = inputFileName;
            _myEncryptedFileName = encryptedFileName;
            _myDecryptedFileName = decryptedFileName;
            _myFolderName = myFolderName;
        }
        public void EncryptFile()
        {
            try
            {
                using Aes aesAlg = Aes.Create();
                aesAlg.GenerateIV(); // Generate a new random IV
                byte[] iv = aesAlg.IV;
                string plainText = File.ReadAllText(_myInputFileName);
                byte[] encrypted = EncryptStringToBytesAes(plainText, _key, iv);
                using FileStream fs = new FileStream(_myFolderName+_myEncryptedFileName, FileMode.Create, FileAccess.Write);
                fs.Write(iv, 0, iv.Length);               // Write IV first
                fs.Write(encrypted, 0, encrypted.Length);   // Then write the encrypted data
                Console.WriteLine("File encrypted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        public void DecryptFile()
        {
            try
            {
                byte[] allBytes = File.ReadAllBytes(_myFolderName + _myEncryptedFileName);
                byte[] extractedIv = new byte[16];
                byte[] encryptedData = new byte[allBytes.Length - 16];

                Array.Copy(allBytes, 0, extractedIv, 0, 16); // First 16 bytes are IV
                Array.Copy(allBytes, 16, encryptedData, 0, encryptedData.Length);

                string decryptedText = DecryptStringFromBytesAes(encryptedData, _key, extractedIv);
                File.WriteAllText(_myFolderName + _myDecryptedFileName, decryptedText);

                Console.WriteLine("File decrypted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        private byte[] EncryptStringToBytesAes(string plainText, byte[] key, byte[] iv)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msEncrypt = new MemoryStream();
            using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }
            return msEncrypt.ToArray();
        }
        private string DecryptStringFromBytesAes(byte[] cipherText, byte[] key, byte[] iv)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new MemoryStream(cipherText);
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
    }
}
