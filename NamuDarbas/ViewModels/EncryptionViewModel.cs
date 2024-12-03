using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NamuDarbas.Models;

namespace NamuDarbas.ViewModels
{
    public class EncryptionViewModel
    {
        public string? PublicKey { get; private set; }
        public string? PrivateKey { get; private set; }
        public string? base64Key { get; private set; }
        public string? base64IV { get; private set; }
        public string Encrypt(string plainText, string algorithm)
        {
            switch (algorithm)
            {
                case "AES":
                    return EncryptWithAES(plainText);
                case "DES":
                    return EncryptWithDES(plainText);
                case "Triple DES":
                    return EncryptWithTripleDES(plainText);
                case "RSA":
                    return EncryptWithRSA(plainText);
                default:
                    throw new NotImplementedException("Unknown algorithm");
            }
        }
        private string EncryptWithAES(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                // Generate a new key and IV
                aes.GenerateKey();
                aes.GenerateIV();
        
                // Perform the encryption
                byte[] encrypted = EncryptStringToBytes_Aes(plainText, aes.Key, aes.IV);
                string encryptedMessage = Convert.ToBase64String(encrypted);

                // Convert key and IV to Base64 for storage
                base64Key = Convert.ToBase64String(aes.Key);
                base64IV = Convert.ToBase64String(aes.IV);

                // Store the AES key and IV along with the encrypted message
                AESDESStoreKeysAndMessage("AES", base64Key, base64IV, encryptedMessage);

                // Return the encrypted message
                return encryptedMessage;
            }
        }
        
        private void AESDESStoreKeysAndMessage(string algorithm, string key, string iv, string encryptedMessage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(algorithm) || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(iv) || string.IsNullOrWhiteSpace(encryptedMessage))
                {
                    throw new ArgumentException("Algorithm, key, IV, or encrypted message cannot be null or empty.");
                }

                using (var context = new EncryptionContext())
                {
                    var keyInfo = new KeyInfo
                    {
                        Algorithm = algorithm,
                        PublicKey = key,        // Using the 'PublicKey' field to store the AES key
                        PrivateKey = iv,        // Using the 'PrivateKey' field to store the AES IV
                        Message = encryptedMessage
                    };

                    context.Keys.Add(keyInfo);
                    context.SaveChanges();
                    Console.WriteLine("[Info] Successfully saved AES key, IV, and encrypted message to the database.");
                }
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"[Error] DbUpdateException: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"[Error] Inner Exception: {dbEx.InnerException.Message}");
                }
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] General Exception during key and message storage: {ex.Message}");
                throw;
            }
        }

        private byte[] EncryptStringToBytes_Aes(string plainText, byte[] key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        private string EncryptWithDES(string plainText)
        {
            using (DES des = DES.Create())
            {
                des.GenerateKey();
                des.GenerateIV();
                byte[] encrypted = EncryptStringToBytes_DES(plainText, des.Key, des.IV);
                base64Key = Convert.ToBase64String(des.Key);
                base64IV = Convert.ToBase64String(des.IV);
                string encryptedMessage = Convert.ToBase64String(encrypted);
                AESDESStoreKeysAndMessage("DES", base64Key, base64IV, encryptedMessage);
                return Convert.ToBase64String(encrypted);
            }
        }

        private byte[] EncryptStringToBytes_DES(string plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            using (DES desAlg = DES.Create())
            {
                desAlg.Key = key;
                desAlg.IV = iv;

                ICryptoTransform encryptor = desAlg.CreateEncryptor(desAlg.Key, desAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        private string EncryptWithTripleDES(string plainText)
        {
            using (TripleDES tripleDES = TripleDES.Create())
            {
                tripleDES.GenerateKey();
                tripleDES.GenerateIV();
                byte[] encrypted = EncryptStringToBytes_TripleDES(plainText, tripleDES.Key, tripleDES.IV);
                base64Key = Convert.ToBase64String(tripleDES.Key);
                base64IV = Convert.ToBase64String(tripleDES.IV);
                string encryptedMessage = Convert.ToBase64String(encrypted);
                AESDESStoreKeysAndMessage("TDES", base64Key, base64IV, encryptedMessage);
                return Convert.ToBase64String(encrypted);
            }
        }
        
        private byte[] EncryptStringToBytes_TripleDES(string plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            using (TripleDES tripleDesAlg = TripleDES.Create())
            {
                tripleDesAlg.Key = key;
                tripleDesAlg.IV = iv;

                ICryptoTransform encryptor = tripleDesAlg.CreateEncryptor(tripleDesAlg.Key, tripleDesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return msEncrypt.ToArray();
                    }
                }
            }
        }
        private string EncryptWithRSA(string plainText)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                // Export public parameters (public key)
                RSAParameters publicKeyParams = rsa.ExportParameters(false);
                PublicKey = ConvertPublicKeyToBase64(publicKeyParams);

                // Export private parameters (private key)
                RSAParameters privateKeyParams = rsa.ExportParameters(true);
                PrivateKey = ConvertPrivateKeyToBase64(privateKeyParams);

                // Encrypt the plaintext message using the public key
                byte[] encrypted = rsa.Encrypt(Encoding.UTF8.GetBytes(plainText), false);
                string encryptedMessage = Convert.ToBase64String(encrypted);

                // Store keys and the encrypted message in the database
                StoreKeysAndMessage("RSA", PublicKey, PrivateKey, encryptedMessage);

                return encryptedMessage;
            }
        }
        private string ConvertPublicKeyToBase64(RSAParameters publicKeyParams)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(publicKeyParams.Modulus!);
                    writer.Write(publicKeyParams.Exponent!);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private string ConvertPrivateKeyToBase64(RSAParameters privateKeyParams)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(privateKeyParams.Modulus!);
                    writer.Write(privateKeyParams.Exponent!);
                    writer.Write(privateKeyParams.D!);
                    writer.Write(privateKeyParams.P!);
                    writer.Write(privateKeyParams.Q!);
                    writer.Write(privateKeyParams.DP!);
                    writer.Write(privateKeyParams.DQ!);
                    writer.Write(privateKeyParams.InverseQ!);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
        private void StoreKeysAndMessage(string algorithm, string publicKey, string privateKey, string encryptedMessage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(algorithm) || string.IsNullOrWhiteSpace(publicKey) || string.IsNullOrWhiteSpace(privateKey) || string.IsNullOrWhiteSpace(encryptedMessage))
                {
                    throw new ArgumentException("Algorithm, public key, private key, or encrypted message cannot be null or empty.");
                }

                using (var context = new EncryptionContext())
                {
                    var keyInfo = new KeyInfo
                    {
                        Algorithm = algorithm,
                        Message = encryptedMessage,
                        PublicKey = publicKey,
                        PrivateKey = privateKey,
                       
                    };

                    context.Keys.Add(keyInfo);
                    context.SaveChanges();
                    Console.WriteLine("[Info] Successfully saved keys and encrypted message to the database.");
                }
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"[Error] DbUpdateException: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"[Error] Inner Exception: {dbEx.InnerException.Message}");
                }
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] General Exception during key and message storage: {ex.Message}");
                throw;
            }
        }
        public KeyInfo GetKeyInfo(int id)
        {
            using (var context = new EncryptionContext())
            {
                return context.Keys.Find(id)!;
            }
        }
        public void TestDatabaseInsert()
        {
            try
            {
                using (var context = new EncryptionContext())
                {
                    var keyInfo = new KeyInfo
                    {
                        Algorithm = "Test Algorithm",
                        PublicKey = "TestPublicKey",
                        PrivateKey = "TestPrivateKey"
                    };

                    context.Keys.Add(keyInfo);
                    context.SaveChanges();
                }
                Console.WriteLine("Test insert successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Exception during test insert: {ex.Message}");
            }
        }

    }
    
}