namespace PriceChecker
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.NumericUpDown nudPriceCheckInterval;
        private System.Windows.Forms.Label lblInterval;
        private System.Windows.Forms.CheckBox chkSoundNotification;
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
            this.nudPriceCheckInterval = new System.Windows.Forms.NumericUpDown();
            this.lblInterval = new System.Windows.Forms.Label();
            this.chkSoundNotification = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudPriceCheckInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // lblInterval
            // 
            this.lblInterval.AutoSize = true;
            this.lblInterval.Location = new System.Drawing.Point(12, 15);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(192, 13);
            this.lblInterval.TabIndex = 0;
            this.lblInterval.Text = "Интервал проверки цен (в часах):";
            // 
            // nudPriceCheckInterval
            // 
            this.nudPriceCheckInterval.Location = new System.Drawing.Point(210, 13);
            this.nudPriceCheckInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPriceCheckInterval.Name = "nudPriceCheckInterval";
            this.nudPriceCheckInterval.Size = new System.Drawing.Size(60, 20);
            this.nudPriceCheckInterval.TabIndex = 1;
            this.nudPriceCheckInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkSoundNotification
            // 
            this.chkSoundNotification.AutoSize = true;
            this.chkSoundNotification.Location = new System.Drawing.Point(15, 50);
            this.chkSoundNotification.Name = "chkSoundNotification";
            this.chkSoundNotification.Size = new System.Drawing.Size(177, 17);
            this.chkSoundNotification.TabIndex = 2;
            this.chkSoundNotification.Text = "Включить звуковые уведомления";
            this.chkSoundNotification.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(15, 120);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 25);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // SettingsForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 161);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.chkSoundNotification);
            this.Controls.Add(this.nudPriceCheckInterval);
            this.Controls.Add(this.lblInterval);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройки";
            ((System.ComponentModel.ISupportInitialize)(this.nudPriceCheckInterval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
