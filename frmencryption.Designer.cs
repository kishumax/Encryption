namespace encryption
{
    partial class frmencryption
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnVideoencryption = new DevExpress.XtraEditors.SimpleButton();
            this.btndecryption = new DevExpress.XtraEditors.SimpleButton();
            this.SuspendLayout();
            // 
            // btnVideoencryption
            // 
            this.btnVideoencryption.Appearance.Font = new System.Drawing.Font("Poppins", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVideoencryption.Appearance.Options.UseFont = true;
            this.btnVideoencryption.Location = new System.Drawing.Point(36, 30);
            this.btnVideoencryption.Name = "btnVideoencryption";
            this.btnVideoencryption.Size = new System.Drawing.Size(164, 47);
            this.btnVideoencryption.TabIndex = 0;
            this.btnVideoencryption.Text = "Video Encryption";
            this.btnVideoencryption.Click += new System.EventHandler(this.btnVideoencryption_Click);
            // 
            // btndecryption
            // 
            this.btndecryption.Appearance.Font = new System.Drawing.Font("Poppins", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btndecryption.Appearance.Options.UseFont = true;
            this.btndecryption.Location = new System.Drawing.Point(36, 101);
            this.btndecryption.Name = "btndecryption";
            this.btndecryption.Size = new System.Drawing.Size(164, 47);
            this.btndecryption.TabIndex = 1;
            this.btndecryption.Text = "Video Decryption";
            this.btndecryption.Click += new System.EventHandler(this.btndecryption_Click);
            // 
            // frmencryption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 173);
            this.Controls.Add(this.btndecryption);
            this.Controls.Add(this.btnVideoencryption);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmencryption";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Encryption ";
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnVideoencryption;
        private DevExpress.XtraEditors.SimpleButton btndecryption;
    }
}

