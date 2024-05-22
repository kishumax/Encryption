using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace encryption
{
    public partial class frmencryption : DevExpress.XtraEditors.XtraForm
    {
        public frmencryption()
        {
            InitializeComponent();
        }

        private void btnVideoencryption_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Video Files|*.mp4;*.avi;*.mkv;*.mov";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filePath = openFileDialog.FileName;
                    byte[] videoBytes = File.ReadAllBytes(filePath);

                    byte[] encryptionKey = GenerateRandomKey(32); // AES-256 requires a 32-byte key

                    (byte[] encryptedBytes, byte[] iv) = EncryptBytes(videoBytes, encryptionKey);

                    string downloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
                    string encryptedFilePath = Path.Combine(downloadFolder, GenerateRandomFileName("Video.dat"));
                    File.WriteAllBytes(encryptedFilePath, encryptedBytes);
                    string ivFilePath = Path.Combine(downloadFolder, GenerateRandomFileName("IV.dat"));
                    File.WriteAllBytes(ivFilePath, iv);
                    string keyFilePath = Path.Combine(downloadFolder, GenerateRandomFileName("Key.dat"));
                    File.WriteAllBytes(keyFilePath, encryptionKey);

                    MessageBox.Show("Video encrypted and saved to " + encryptedFilePath + ".\nIV saved to " + ivFilePath + ".\nKey saved to " + keyFilePath + ".", "Encryption Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error encrypting the video: " + ex.Message, "Encryption Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private byte[] GenerateRandomKey(int size)
        {
            byte[] key = new byte[size];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(key);
            }
            return key;
        }

        private (byte[] EncryptedBytes, byte[] IV) EncryptBytes(byte[] bytesToEncrypt, byte[] key)
        {
            byte[] encryptedBytes;
            byte[] iv;
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();
                iv = aes.IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return (encryptedBytes, iv);
        }

        private void btndecryption_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Encrypted Files|*.dat";
            openFileDialog.Title = "Select Encrypted Video File";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string encryptedFilePath = openFileDialog.FileName;
                    byte[] encryptedBytes = File.ReadAllBytes(encryptedFilePath);

                    openFileDialog.Title = "Select Encryption IV File";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string ivFilePath = openFileDialog.FileName;
                        byte[] iv = File.ReadAllBytes(ivFilePath);

                        openFileDialog.Title = "Select Encryption Key File";
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string keyFilePath = openFileDialog.FileName;
                            byte[] key = File.ReadAllBytes(keyFilePath);

                            byte[] decryptedBytes = DecryptBytes(encryptedBytes, key, iv);

                            SaveFileDialog saveFileDialog = new SaveFileDialog();
                            saveFileDialog.Filter = "Video Files|*.mp4;*.avi;*.mkv;*.mov";
                            saveFileDialog.Title = "Save Decrypted Video File As";
                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                string decryptedFilePath = saveFileDialog.FileName;
                                File.WriteAllBytes(decryptedFilePath, decryptedBytes);

                                MessageBox.Show("Video decrypted and saved to " + decryptedFilePath, "Decryption Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error decrypting the video: " + ex.Message, "Decryption Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private byte[] DecryptBytes(byte[] bytesToDecrypt, byte[] key, byte[] iv)
        {
            byte[] decryptedBytes;
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(bytesToDecrypt))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream outputStream = new MemoryStream())
                        {
                            cs.CopyTo(outputStream);
                            decryptedBytes = outputStream.ToArray();
                        }
                    }
                }
            }
            return decryptedBytes;
        }

        private string GenerateRandomFileName(string suffix)
        {
            return Path.GetRandomFileName().Replace(".", "") + "_" + suffix;
        }
    }
}
