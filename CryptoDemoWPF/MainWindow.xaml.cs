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

        // DES uses 8-byte key (64-bit, effective 56-bit)
        private readonly byte[] desKey = Encoding.UTF8.GetBytes("12345678");
        // Des block size = 8 bytes
        private readonly byte[] desIV = Encoding.UTF8.GetBytes("abcdefgh");

        // TDES uses 24-byte key (3 x DES keys)
        private readonly byte[] tdesKey = Encoding.UTF8.GetBytes("123456781234567812345678");
        // Same block size as DES (8 bytes)
        private readonly byte[] tdesIV = Encoding.UTF8.GetBytes("abcdefgh");

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

        private string HashSHA256(string input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha.ComputeHash(inputBytes);

                // convert to Hex string
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();

                //Text → UTF-8 bytes → SHA-256 → 32 bytes → Hex string
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

        private string HashSHA256WithSalt(String input, string salt)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] combined = Encoding.UTF8.GetBytes(input + salt);
                byte[] hashBytes = sha.ComputeHash(combined);

                //return Convert.ToHexString(hashBytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        private string GenerateSalt(int size = 16)
        {
            byte[] saltBytes = new byte[size];
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            return Convert.ToBase64String(saltBytes);
        }

        private string EncryptDES(string plainText)
        {
            using (DES des = DES.Create())
            {
                des.Key = desKey;
                des.IV = desIV;
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;

                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

                using(var ms = new MemoryStream())
                using(var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write)){
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        private string DecryptDES(string cipherText)
        {
            using (DES des = DES.Create())
            {
                des.Key = desKey;
                des.IV = desIV;
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;

                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.FlushFinalBlock();

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }


        // TDES


        private void EncryptDES_Click (object sender, RoutedEventArgs e)
        {
            txtOutput.Text = EncryptDES(txtInput.Text);
        }

        private void DecryptDES_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtOutput.Text = DecryptDES(txtOutput.Text);
            }
            catch
            {
                MessageBox.Show("Invalid DES ciphertext or key/IV mismatch");
            }
        }

        private void Hash_Click(object sender, RoutedEventArgs e)
        {
            txtOutput.Text = HashSHA256(txtInput.Text);  //(64 hex characters)
        }

        private void HashSalt_Click(object sender, RoutedEventArgs e)
        {
            string salt = GenerateSalt();
            string hash = HashSHA256WithSalt(txtInput.Text, salt);

            txtOutput.Text = $"Salt:\n{salt}\n\nHash:\n{hash}";
        }

        
    }
}
