using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.Security.Cryptography;

namespace CryptoDemoWPF
{
    public partial class MainWindow : Window
    {
        // 16 bytes = 128-bit key (AES-128)
        private readonly byte[] aesKey = Encoding.UTF8.GetBytes("1234567890123456");

        // 16 bytes IV (Aes block size)
        private readonly byte[] aesIV = Encoding.UTF8.GetBytes("abcdef1234567890");
        public MainWindow()
        {
            InitializeComponent();
        }


        private string EncryptAES(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = aesKey;
                aes.IV = aesIV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] plainBytes =Encoding.UTF8.GetBytes(plainText);

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms,aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();

                    return Convert.ToBase64String(ms.ToArray());
                }

            }
        }

        private string DecryptAES(string cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = aesKey;
                aes.IV = aesIV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (var ms = new MemoryStream())
                    using (var cs = new CryptoStream(ms,aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.FlushFinalBlock();

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

        private void EncryptAES_Click(object sender, RoutedEventArgs e)
        {
            txtOutput.Text = EncryptAES(txtInput.Text);
        }

        private void DecryptAES_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtOutput.Text = DecryptAES(txtOutput.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Invalid cipherText or key/IV mismatch");
            }
        }

        private void Hash_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hashing will be implemented next phase");
        }
    }
}
