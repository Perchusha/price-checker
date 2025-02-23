using System;
using System.Windows.Forms;

namespace PriceChecker
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            nudPriceCheckInterval.Value = Properties.Settings.Default.PriceCheckIntervalHours;
            chkSoundNotification.Checked = Properties.Settings.Default.EnableSoundNotification;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.PriceCheckIntervalHours = (int)nudPriceCheckInterval.Value;
            Properties.Settings.Default.EnableSoundNotification = chkSoundNotification.Checked;
            Properties.Settings.Default.Save();

            MessageBox.Show("Настройки сохранены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}
