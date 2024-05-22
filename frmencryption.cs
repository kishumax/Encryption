using System;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace encryption
{
    public partial class frmencryption : DevExpress.XtraEditors.XtraForm
    {
        private byte[] encryptionKey;
        private byte[] iv;

        public frmencryption()
        {
            InitializeComponent();
        }

        private void btnEncryption_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "All Files|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    byte[] fileBytes = File.ReadAllBytes(filePath);

                    encryptionKey = GenerateRandomKey(32); // AES-256 requires a 32-byte key
                    iv = GenerateRandomIV(16); // 128-bit IV

                    (byte[] encryptedBytes, _) = EncryptBytes(fileBytes, encryptionKey, iv);

                    string downloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
                    string encryptedFilePath = Path.Combine(downloadFolder, GenerateRandomFileName("EncryptedData.dat"));
                    File.WriteAllBytes(encryptedFilePath, encryptedBytes);

                    string keyFilePath = Path.Combine(downloadFolder, "KeyAndIV.txt");
                    string keyAndIVText = "encryptionKey: " + Convert.ToBase64String(encryptionKey) + "\niv: " + Convert.ToBase64String(iv);
                    File.WriteAllText(keyFilePath, keyAndIVText);

                    MessageBox.Show("File encrypted and saved to " + encryptedFilePath + ".\nKey and IV saved to " + keyFilePath + ".", "Encryption Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Prompt for receiver email address
                    string receiverEmail = Prompt.ShowDialog("Enter Receiver Email Address:");

                    if (!string.IsNullOrWhiteSpace(receiverEmail) && IsValidEmail(receiverEmail))
                    {
                        // Send email with attachment
                        SendEmailWithAttachment(receiverEmail, keyFilePath);
                    }
                    else
                    {
                        MessageBox.Show("Invalid email address. Please enter a valid email address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error encrypting the file: " + ex.Message, "Encryption Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateRandomFileName(string suffix)
        {
            return Path.GetRandomFileName().Replace(".", "") + "_" + suffix;
        }
        private void SendEmailWithAttachment(string receiverEmail, string attachmentFilePath)
        {
            string senderEmail = "your.email@example.com"; // Change to your email address
            string senderPassword = "your_password"; // Change to your email password

            SmtpClient client = new SmtpClient("smtp.example.com"); // Change to your SMTP server
            client.Port = 587;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(senderEmail, senderPassword);

            MailMessage mail = new MailMessage(senderEmail, receiverEmail);
            mail.Subject = "Key and IV File";
            mail.Body = "Please find the attached Key and IV file.";
            mail.Attachments.Add(new Attachment(attachmentFilePath));

            client.Send(mail);

            MessageBox.Show("Email sent successfully to " + receiverEmail, "Email Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void btnDecryption_Click(object sender, EventArgs e)
        {
            try
            {
                string keyInput = Prompt.ShowDialog("Enter Encryption Key:");
                string ivInput = Prompt.ShowDialog("Enter IV:");

                if (!IsValidBase64String(keyInput) || !IsValidBase64String(ivInput))
                {
                    MessageBox.Show("Invalid encryption key or IV format. Please provide valid Base64 encoded strings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                encryptionKey = Convert.FromBase64String(keyInput);
                this.iv = Convert.FromBase64String(ivInput);

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Encrypted Files|*.dat";
                openFileDialog.Title = "Select Encrypted Data File";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string encryptedFilePath = openFileDialog.FileName;
                    byte[] encryptedBytes = File.ReadAllBytes(encryptedFilePath);

                    byte[] decryptedBytes = DecryptBytes(encryptedBytes, encryptionKey, this.iv);

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string decryptedFilePath = saveFileDialog.FileName;
                        File.WriteAllBytes(decryptedFilePath, decryptedBytes);

                        MessageBox.Show("File decrypted and saved to " + decryptedFilePath, "Decryption Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error decrypting the file: " + ex.Message, "Decryption Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValidBase64String(string input)
        {
            try
            {
                Convert.FromBase64String(input);
                return true;
            }
            catch
            {
                return false;
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

        private byte[] GenerateRandomIV(int size)
        {
            byte[] iv = new byte[size];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(iv);
            }
            return iv;
        }


        private (byte[] EncryptedBytes, byte[] IV) EncryptBytes(byte[] bytesToEncrypt, byte[] key, byte[] iv)
        {
            byte[] encryptedBytes;
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return (encryptedBytes, iv);
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
    }

    public static class Prompt
    {
        public static string ShowDialog(string text)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = text,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
