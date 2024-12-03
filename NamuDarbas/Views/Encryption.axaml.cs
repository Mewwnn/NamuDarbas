using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using NamuDarbas.ViewModels;

namespace NamuDarbas.Views
{
    public partial class Encryption : Window
    {
        private EncryptionViewModel _viewModel;

        public Encryption()
        {
            InitializeComponent();
            _viewModel = new EncryptionViewModel();
        }

        private async void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string messageToEncrypt = MessageTextBox.Text;
                var selectedAlgorithmItem = AlgorithmComboBox.SelectedItem as ComboBoxItem;
                if (selectedAlgorithmItem?.Content != null)
                {
                    string selectedAlgorithm = selectedAlgorithmItem.Content.ToString();

                    if (string.IsNullOrEmpty(messageToEncrypt))
                    {
                        await ShowMessageDialog("Error", "Please enter a message to encrypt.");
                        return;
                    }

                    if (string.IsNullOrEmpty(selectedAlgorithm))
                    {
                        await ShowMessageDialog("Error", "Please select an encryption algorithm.");
                        return;
                    }

                    // Perform the encryption asynchronously to prevent freezing
                    string encryptedMessage = await Task.Run(() => _viewModel.Encrypt(messageToEncrypt, selectedAlgorithm));

                    // Display the encrypted message
                    EncryptedMessageTextBox.Text = encryptedMessage;

                    // If RSA, we also need to display the public and private keys
                    if (selectedAlgorithm == "RSA")
                    {
                        PublicKeyTextBox.Text = _viewModel.PublicKey;
                        PrivateKeyTextBox.Text = _viewModel.PrivateKey;
                    }
                    else if (selectedAlgorithm == "AES" || selectedAlgorithm == "DES" || selectedAlgorithm == "Triple DES")
                    {
                        PublicKeyTextBox.Text = _viewModel.base64Key;
                        PrivateKeyTextBox.Text = _viewModel.base64IV;
                    }
                    else
                    {
                        PublicKeyTextBox.Text = string.Empty;
                        PrivateKeyTextBox.Text = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowMessageDialog("Encryption Error", $"An error occurred during encryption: {ex.Message}");
            }
        }

        private async Task ShowMessageDialog(string title, string message)
        {
            var dialog = new Window
            {
                Title = title,
                Width = 300,
                Height = 150,
                Content = new StackPanel
                {
                    Margin = new Thickness(20),
                    Children =
                    {
                        new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 10) },
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