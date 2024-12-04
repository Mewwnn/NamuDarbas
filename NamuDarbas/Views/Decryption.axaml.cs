using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Security.Cryptography;
using System.Text;
using NamuDarbas.Exceptions;
using NamuDarbas.Models;

namespace NamuDarbas.Views
{
    public partial class Decryption : Window
    {
        public Decryption()
        {
            InitializeComponent();
        }

        private async void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Retrieve selected algorithm and message ID
                var selectedAlgorithmItem = AlgorithmComboBox.SelectedItem as ComboBoxItem;
                if (selectedAlgorithmItem?.Content != null && !string.IsNullOrEmpty(MessageIdTextBox.Text))
                {
                    string selectedAlgorithm = selectedAlgorithmItem.Content.ToString();
                    if (int.TryParse(MessageIdTextBox.Text, out int messageId))
                    {
                        // Retrieve key and encrypted message from database using message ID
                        var keyInfo = GetKeyInfoById(messageId);
                        if (keyInfo != null)
                        {
                            string decryptedMessage = string.Empty;

                            if (selectedAlgorithm == "AES")
                            {
                                decryptedMessage = DecryptWithAES(keyInfo.Message, keyInfo.PublicKey, keyInfo.PrivateKey);
                            }
                            else if (selectedAlgorithm == "RSA")
                            {
                                decryptedMessage = DecryptWithRSA(keyInfo.Message, keyInfo.PrivateKey);
                            }
                            else if (selectedAlgorithm == "DES")
                            {
                                decryptedMessage = DecryptWithDES(keyInfo.Message, keyInfo.PublicKey,
                                    keyInfo.PrivateKey);
                            }
                            else if (selectedAlgorithm == "Triple DES")
                            {
                                decryptedMessage = DecryptWithTripleDES(keyInfo.Message, keyInfo.PublicKey,
                                    keyInfo.PrivateKey);
                            }

                            DecryptedMessageTextBox.Text = decryptedMessage;
                        }
                        else
                        {
                            throw new InvalidAlgorithmException($"Algorithm '{selectedAlgorithm}' is not supported for decryption.");                        }
                    }
                    else
                    {
                        await ShowMessageDialog("Error", "Invalid Message ID format.");
                    }
                }
                else
                {
                    await ShowMessageDialog("Error", "Please select an algorithm and provide a valid message ID.");
                }
            }
            catch (InvalidAlgorithmException ex)
            {
                await ShowMessageDialog("Algorithm Error", ex.Message);
            }
            catch (Exception ex)
            {
                await ShowMessageDialog("Decryption Error", $"An error occurred during decryption: {ex.Message}");
            }
        }

        private KeyInfo GetKeyInfoById(int id)
        {
            using (var context = new EncryptionContext())
            {
                var keyInfo = context.Keys.Find(id);
                if (keyInfo == null)
                {
                    throw new KeyNotFoundException($"The key with ID '{id}' was not found in the database.");
                }
                return keyInfo;
            }
        }

        private string DecryptWithAES(string encryptedMessage, string keyBase64, string ivBase64)
        {
            byte[] key = Convert.FromBase64String(keyBase64);
            byte[] iv = Convert.FromBase64String(ivBase64);
            byte[] cipherText = Convert.FromBase64String(encryptedMessage);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new System.IO.MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        
        private string DecryptWithDES(string encryptedMessage, string keyBase64, string ivBase64)
        {
            byte[] key = Convert.FromBase64String(keyBase64);
            byte[] iv = Convert.FromBase64String(ivBase64);
            byte[] cipherText = Convert.FromBase64String(encryptedMessage);

            using (DES desAlg = DES.Create())
            {
                desAlg.Key = key;
                desAlg.IV = iv;

                ICryptoTransform decryptor = desAlg.CreateDecryptor(desAlg.Key, desAlg.IV);

                using (var msDecrypt = new System.IO.MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        
        private string DecryptWithTripleDES(string encryptedMessage, string keyBase64, string ivBase64)
        {
            byte[] key = Convert.FromBase64String(keyBase64);
            byte[] iv = Convert.FromBase64String(ivBase64);
            byte[] cipherText = Convert.FromBase64String(encryptedMessage);

            using (TripleDES TdesAlg = TripleDES.Create())
            {
                TdesAlg.Key = key;
                TdesAlg.IV = iv;

                ICryptoTransform decryptor = TdesAlg.CreateDecryptor(TdesAlg.Key, TdesAlg.IV);

                using (var msDecrypt = new System.IO.MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        private string DecryptWithRSA(string encryptedMessage, string privateKeyBase64)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedMessage);

            RSAParameters privateKeyParams = ConvertBase64ToPrivateKey(privateKeyBase64);

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.ImportParameters(privateKeyParams);
                byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, false);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }

        private RSAParameters ConvertBase64ToPrivateKey(string privateKeyBase64)
        {
            byte[] keyBytes = Convert.FromBase64String(privateKeyBase64);
            using (var ms = new System.IO.MemoryStream(keyBytes))
            {
                using (var reader = new System.IO.BinaryReader(ms))
                {
                    RSAParameters parameters = new RSAParameters
                    {
                        Modulus = reader.ReadBytes(256),
                        Exponent = reader.ReadBytes(3),
                        D = reader.ReadBytes(256),
                        P = reader.ReadBytes(128),
                        Q = reader.ReadBytes(128),
                        DP = reader.ReadBytes(128),
                        DQ = reader.ReadBytes(128),
                        InverseQ = reader.ReadBytes(128)
                    };
                    return parameters;
                }
            }
        }

        private async System.Threading.Tasks.Task ShowMessageDialog(string title, string message)
        {
            var dialog = new Window
            {
                Title = title,
                Width = 300,
                Height = 150,
                Content = new StackPanel
                {
                    Margin = new Avalonia.Thickness(20),
                    Children =
                    {
                        new TextBlock { Text = message, TextWrapping = Avalonia.Media.TextWrapping.Wrap, Margin = new Avalonia.Thickness(0, 0, 0, 10) },
                        new Button { Content = "OK", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center }
                    }
                }
            };

            var button = (Button)((StackPanel)dialog.Content).Children[1];
            button.Click += (s, e) => dialog.Close();

            await dialog.ShowDialog(this);
        }
    }
}
