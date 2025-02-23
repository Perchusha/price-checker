using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace PriceChecker
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            chkAutoStart.Checked = IsAutoStartEnabled();
        }

        // Обработчик нажатия на кнопку "Сохранить"
        private void btnSave_Click(object sender, EventArgs e)
        {
            SetAutoStart(chkAutoStart.Checked);
            MessageBox.Show("Настройки сохранены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private bool IsAutoStartEnabled()
        {
            string runKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(runKey, false))
            {
                return key.GetValue("PriceChecker") != null;
            }
        }

        private void SetAutoStart(bool enable)
        {
            string runKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(runKey, true))
            {
                if (enable)
                {
                    key.SetValue("PriceChecker", Application.ExecutablePath);
                }
                else
                {
                    key.DeleteValue("PriceChecker", false);
                }
            }
        }
    }
}
