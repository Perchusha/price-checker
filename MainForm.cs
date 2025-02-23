using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PriceChecker
{
    public partial class MainForm : Form
    {
        private Label lblSupportedSites;
        private TextBox txtName;
        private TextBox txtUrl;
        private TextBox txtTargetPrice;
        private Label lblStatus;
        private Button btnAdd;
        private DataGridView dgvEntries;
        private Button btnStartChecking;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private ProgressBar progressBar;
        private System.Timers.Timer priceCheckTimer;
        private PriceCheckerService priceService = new PriceCheckerService();
        private MenuStrip mainMenu;
        private ToolStripMenuItem settingsMenuItem;

        private string currentNotificationUrl = "";
        private string dataFilePath;
        private bool isExiting = false;
        private bool isChecking = false;

        public MainForm()
        {
            InitializeComponent();
            SetupMenu();
            SetupControls();
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
            mainMenu = new MenuStrip();

            settingsMenuItem = new ToolStripMenuItem("Настройки");
            settingsMenuItem.Click += (s, e) =>
            {
                using (var settingsForm = new SettingsForm())
                {
                    settingsForm.ShowDialog(this);
                }
            };

            mainMenu.Items.Add(settingsMenuItem);
            this.MainMenuStrip = mainMenu;
            this.Controls.Add(mainMenu);
        }

        private void LoadEntries()
        {
            if (File.Exists(dataFilePath))
            {
                var entries = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Entry>>(File.ReadAllText(dataFilePath));
                foreach (var entry in entries)
                {
                    dgvEntries.Rows.Add(entry.Name, entry.Url, entry.Price);
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
                    entries.Add(new Entry
                    {
                        Name = row.Cells["Name"].Value.ToString(),
                        Url = row.Cells["Url"].Value.ToString(),
                        Price = decimal.Parse(row.Cells["Price"].Value.ToString())
                    });
                }
            }
            File.WriteAllText(dataFilePath, Newtonsoft.Json.JsonConvert.SerializeObject(entries, Newtonsoft.Json.Formatting.Indented));
        }

        private void SetupControls()
        {
            lblSupportedSites = new Label
            {
                Text = "Поддерживаемые сайты:\n" +
                       "• Amazon\n" +
                       "• NBsklep\n" +
                       "• Ceneo\n" +
                       "• Morele\n" +
                       "• X-kom\n" +
                       "• YesStyle\n",
                Location = new Point(10, 10),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(lblSupportedSites);

            GroupBox groupInput = new GroupBox
            {
                Text = "Новый товар",
                Location = new Point(10, lblSupportedSites.Bottom + 10),
                Size = new Size(this.ClientSize.Width - 20, 50),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };
            this.Controls.Add(groupInput);

            Label lblName = new Label
            {
                Text = "Имя:",
                Location = new Point(10, 25),
                AutoSize = true,
            };
            groupInput.Controls.Add(lblName);

            txtName = new TextBox
            {
                Location = new Point(lblName.Right + 5, 20),
                Size = new Size(140, 25),
            };
            groupInput.Controls.Add(txtName);

            Label lblUrl = new Label
            {
                Text = "Ссылка:",
                Location = new Point(txtName.Right + 10, 25),
                AutoSize = true,
            };
            groupInput.Controls.Add(lblUrl);

            txtUrl = new TextBox
            {
                Location = new Point(lblUrl.Right + 5, 20),
                Size = new Size(220, 25),
            };
            groupInput.Controls.Add(txtUrl);

            Label lblPrice = new Label
            {
                Text = "Целевая цена:",
                Location = new Point(txtUrl.Right + 10, 25),
                AutoSize = true,
            };
            groupInput.Controls.Add(lblPrice);

            txtTargetPrice = new TextBox
            {
                Location = new Point(lblPrice.Right + 5, 20),
                Size = new Size(80, 25),
            };
            groupInput.Controls.Add(txtTargetPrice);

            btnAdd = new Button
            {
                Text = "Добавить",
                Location = new Point(txtTargetPrice.Right + 5, 20),
                AutoSize = true,
            };
            btnAdd.Click += BtnAdd_Click;
            groupInput.Controls.Add(btnAdd);

            dgvEntries = new DataGridView
            {
                Location = new Point(10, groupInput.Bottom + 10),
                Size = new Size(780, 372),
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.CellSelect,
                MultiSelect = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText,
            };

            dgvEntries.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Имя",
                Name = "Name",
                FillWeight = 2
            });
            dgvEntries.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Ссылка",
                Name = "Url",
                FillWeight = 3
            });
            dgvEntries.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Цена",
                Name = "Price",
                FillWeight = 1,
                ValueType = typeof(decimal)
            });
            dgvEntries.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Действие",
                Name = "Delete",
                Text = "Удалить",
                UseColumnTextForButtonValue = true,
                FillWeight = 1
            });
            dgvEntries.CellContentClick += DgvEntries_CellContentClick;
            dgvEntries.CellEndEdit += DgvEntries_CellEndEdit;
            this.Controls.Add(dgvEntries);

            btnStartChecking = new Button
            {
                Text = "Начать проверку",
                Location = new Point(10, dgvEntries.Bottom + 10),
                Size = new Size(200, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            btnStartChecking.Click += BtnStartChecking_Click;
            this.Controls.Add(btnStartChecking);

            progressBar = new ProgressBar
            {
                Location = new Point(220, dgvEntries.Bottom + 10),
                Size = new Size(300, 30),
                Visible = false,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            this.Controls.Add(progressBar);

            lblStatus = new Label
            {
                AutoSize = true,
                Text = "Статус: Ожидание\nПоследняя проверка: -",
                Location = new Point(this.ClientSize.Width - 180, dgvEntries.Bottom + 10),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            this.Controls.Add(lblStatus);
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

            dgvEntries.Rows.Add(txtName.Text.Trim(), txtUrl.Text.Trim(), targetPrice);
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
