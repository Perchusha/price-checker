namespace PriceChecker
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.CheckBox chkAutoStart;
        private System.Windows.Forms.Button btnSave;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkAutoStart
            // 
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Location = new System.Drawing.Point(12, 12);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new System.Drawing.Size(259, 17);
            this.chkAutoStart.TabIndex = 0;
            this.chkAutoStart.Text = "Запускать PriceChecker при старте системы";
            this.chkAutoStart.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(12, 35);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(85, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // SettingsForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 71);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.chkAutoStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройки";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
