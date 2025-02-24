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
            nudPriceCheckInterval.Value = Properties.Settings.Default.PriceCheckIntervalHours;
            chkSoundNotification.Checked = Properties.Settings.Default.EnableSoundNotification;
            chkAutostart.Checked = Properties.Settings.Default.AutoStart;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.PriceCheckIntervalHours = (int)nudPriceCheckInterval.Value;
            Properties.Settings.Default.EnableSoundNotification = chkSoundNotification.Checked;
            Properties.Settings.Default.AutoStart = chkAutostart.Checked;
            Properties.Settings.Default.Save();

            UpdateAutoStart(chkAutostart.Checked);

            MessageBox.Show("Настройки сохранены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void UpdateAutoStart(bool enable)
        {
            string appName = "PriceChecker";
            string exePath = Application.ExecutablePath;
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (enable)
            {
                rk.SetValue(appName, exePath);
            }
            else
            {
                rk.DeleteValue(appName, false);
            }
        }
    }
}
