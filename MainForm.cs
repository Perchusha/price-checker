using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PriceChecker
{
    public partial class MainForm : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private System.Timers.Timer priceCheckTimer;
        private PriceCheckerService priceService = new PriceCheckerService();
        private string currentNotificationUrl = "";
        private string dataFilePath;
        private bool isExiting = false;
        private bool isChecking = false;

        public MainForm()
        {
            InitializeComponent();
            SetupMenu();
            SetupControlsEvents();
            SetupTrayIcon();
            SetupTimer();
            InitializeDataStorage();
            LoadEntries();
        }

        private void InitializeDataStorage()
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PriceChecker");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            dataFilePath = Path.Combine(folderPath, "entries.json");
        }

        private void SetupMenu()
        {
            settingsMenuItem.Click += (s, e) =>
            {
                using (var settingsForm = new SettingsForm())
                {
                    settingsForm.ShowDialog(this);
                }
            };
        }

        private void SetupControlsEvents()
        {
            btnAdd.Click += BtnAdd_Click;
            btnStartChecking.Click += BtnStartChecking_Click;
            dgvEntries.CellContentClick += DgvEntries_CellContentClick;
            dgvEntries.CellEndEdit += DgvEntries_CellEndEdit;
        }

        private void LoadEntries()
        {
            if (File.Exists(dataFilePath))
            {
                var entries = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Entry>>(File.ReadAllText(dataFilePath));
                foreach (var entry in entries)
                {
                    dgvEntries.Rows.Add(entry.Enabled, entry.Name, entry.Url, entry.Price);
                }
            }
        }

        private void SaveEntries()
        {
            var entries = new List<Entry>();
            foreach (DataGridViewRow row in dgvEntries.Rows)
            {
                if (row.Cells["Name"].Value != null &&
                    row.Cells["Url"].Value != null &&
                    row.Cells["Price"].Value != null)
                {
                    bool enabled = false;
                    if (row.Cells["Enabled"].Value is bool val)
                        enabled = val;

                    entries.Add(new Entry
                    {
                        Enabled = enabled,
                        Name = row.Cells["Name"].Value.ToString(),
                        Url = row.Cells["Url"].Value.ToString(),
                        Price = decimal.Parse(row.Cells["Price"].Value.ToString())
                    });
                }
            }
            File.WriteAllText(dataFilePath, Newtonsoft.Json.JsonConvert.SerializeObject(entries, Newtonsoft.Json.Formatting.Indented));
        }

        private void SetupTrayIcon()
        {
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Открыть", null, (s, e) => ShowApp());
            trayMenu.Items.Add("Выйти", null, (s, e) => ExitApp());

            trayIcon = new NotifyIcon
            {
                Icon = Properties.Resources.AppIcon,
                ContextMenuStrip = trayMenu,
                Visible = true,
                Text = "Price Checker"
            };

            trayIcon.DoubleClick += (s, e) => ShowApp();
            trayIcon.BalloonTipClicked += (s, e) =>
            {
                if (!string.IsNullOrEmpty(currentNotificationUrl))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = currentNotificationUrl,
                        UseShellExecute = true
                    });
                }
            };
        }

        private void SetupTimer()
        {
            int intervalInHours = Properties.Settings.Default.PriceCheckIntervalHours;
            int intervalInMilliseconds = intervalInHours * 3600000;
            priceCheckTimer = new System.Timers.Timer(intervalInMilliseconds);
            priceCheckTimer.Elapsed += Timer_Elapsed;
            priceCheckTimer.AutoReset = true;
        }

        private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (isChecking)
                return;

            isChecking = true;
            try
            {
                await StartPriceChecking();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка в таймере: " + ex.Message);
            }
            finally
            {
                isChecking = false;
            }
        }

        private void ShowApp()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void ExitApp()
        {
            isExiting = true;
            trayIcon.Visible = false;
            priceCheckTimer?.Stop();
            trayIcon.Dispose();
            Application.Exit();
        }

        private void DgvEntries_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SaveEntries();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUrl.Text) || string.IsNullOrWhiteSpace(txtTargetPrice.Text))
            {
                MessageBox.Show("Введите URL и цену.");
                return;
            }

            if (!decimal.TryParse(txtTargetPrice.Text, out decimal targetPrice))
            {
                MessageBox.Show("Введите корректное числовое значение для цены.");
                return;
            }

            dgvEntries.Rows.Add(true, txtName.Text.Trim(), txtUrl.Text.Trim(), targetPrice);
            txtName.Clear();
            txtUrl.Clear();
            txtTargetPrice.Clear();

            SaveEntries();
        }

        private void DgvEntries_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvEntries.Columns["Delete"].Index && e.RowIndex >= 0)
            {
                dgvEntries.Rows.RemoveAt(e.RowIndex);
                SaveEntries();
            }
        }

        private async void BtnStartChecking_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            await StartPriceChecking();
            priceCheckTimer.Start();
            this.Cursor = Cursors.Default;
            this.Hide();
        }

        private async Task StartPriceChecking()
        {
            this.Invoke(new Action(() =>
            {
                lblStatus.Text = "Статус: Идет проверка...\nПоследняя проверка: " + DateTime.Now.ToString("HH:mm:ss");
                trayIcon.Text = "Price Checker\nПоследняя проверка: " + DateTime.Now.ToString("HH:mm:ss");
            }));

            progressBar.Visible = true;
            progressBar.Minimum = 0;
            progressBar.Maximum = dgvEntries.Rows.Count;
            progressBar.Value = 0;

            List<Task> tasks = new List<Task>();
            var rows = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dgvEntries.Rows)
                rows.Add(row);

            foreach (var row in rows)
            {
                bool isEnabled = row.Cells["Enabled"].Value as bool? ?? false;
                if (!isEnabled)
                {
                    this.Invoke(new Action(() => progressBar.Value++));
                    continue;
                }

                tasks.Add(Task.Run(async () =>
                {
                    string name = row.Cells["Name"].Value?.ToString();
                    string url = row.Cells["Url"].Value?.ToString();
                    string targetPriceStr = row.Cells["Price"].Value?.ToString();

                    if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(targetPriceStr))
                        return;

                    if (!decimal.TryParse(targetPriceStr, out decimal targetPrice))
                        return;

                    decimal actualPrice = await priceService.GetPriceFromUrl(url, name);

                    if (actualPrice < targetPrice)
                    {
                        this.Invoke(new Action(() =>
                        {
                            ShowNotification("Снижение цены",
                                $"Цена по URL:\n{url}\nопустилась ниже {targetPrice}.\nТекущая цена: {actualPrice}", url);
                        }));
                    }
                })
                .ContinueWith(t =>
                {
                    this.Invoke(new Action(() => progressBar.Value++));
                }));
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при проверке цен: " + ex.Message);
            }

            progressBar.Visible = false;

            this.Invoke(new Action(() =>
            {
                lblStatus.Text = "Статус: Готов\nПоследняя проверка: " + DateTime.Now.ToString("HH:mm:ss");
                trayIcon.Text = "Price Checker\nПоследняя проверка: " + DateTime.Now.ToString("HH:mm:ss");
            }));
        }

        private void ShowNotification(string title, string message, string url)
        {
            currentNotificationUrl = url;
            trayIcon.BalloonTipTitle = title;
            trayIcon.BalloonTipText = message;
            trayIcon.ShowBalloonTip(3000);

            if (Properties.Settings.Default.EnableSoundNotification)
            {
                System.Media.SystemSounds.Exclamation.Play();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!isExiting)
            {
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                base.OnFormClosing(e);
            }
        }
    }
}
