using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace PriceChecker
{
    public partial class MainForm : Form
    {
        private Label lblSupportedSites;
        private TextBox txtName;
        private TextBox txtUrl;
        private TextBox txtTargetPrice;
        private Button btnAdd;
        private DataGridView dgvEntries;
        private Button btnStartChecking;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private System.Timers.Timer priceCheckTimer;

        private static readonly HttpClient httpClient = new HttpClient();

        private string currentNotificationUrl = "";
        private string dataFilePath;
        private bool isExiting = false;

        public MainForm()
        {
            InitializeComponent();
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
            dataFilePath = Path.Combine(folderPath, "entries.txt");
        }

        private void LoadEntries()
        {
            if (File.Exists(dataFilePath))
            {
                foreach (var line in File.ReadAllLines(dataFilePath))
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 3)
                    {
                        dgvEntries.Rows.Add(parts[0], parts[1], parts[2]);
                    }
                }
            }
        }

        private void SaveEntries()
        {
            var lines = new List<string>();
            foreach (DataGridViewRow row in dgvEntries.Rows)
            {
                if (row.Cells["Name"].Value != null &&
                    row.Cells["Url"].Value != null &&
                    row.Cells["Price"].Value != null)
                {
                    string name = row.Cells["Name"].Value.ToString();
                    string url = row.Cells["Url"].Value.ToString();
                    string price = row.Cells["Price"].Value.ToString();
                    lines.Add($"{name}|{url}|{price}");
                }
            }
            File.WriteAllLines(dataFilePath, lines);
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
                       "• X-kom\n",
                Location = new Point(10, 10),
                AutoSize = true
            };
            this.Controls.Add(lblSupportedSites);

            GroupBox groupInput = new GroupBox
            {
                Text = "Новый товар",
                Location = new Point(10, lblSupportedSites.Bottom + 10),
                Size = new Size(780, 60)
            };

            txtName = new TextBox
            {
                Location = new Point(10, 25),
                Size = new Size(150, 25),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            txtUrl = new TextBox
            {
                Location = new Point(txtName.Right + 10, 25),
                Size = new Size(350, 25),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            txtTargetPrice = new TextBox
            {
                Location = new Point(txtUrl.Right + 10, 25),
                Size = new Size(100, 25),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            btnAdd = new Button
            {
                Text = "Добавить",
                Location = new Point(txtTargetPrice.Right + 10, 25),
                Size = new Size(140, 25),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnAdd.Click += BtnAdd_Click;

            groupInput.Controls.Add(txtName);
            groupInput.Controls.Add(txtUrl);
            groupInput.Controls.Add(txtTargetPrice);
            groupInput.Controls.Add(btnAdd);
            this.Controls.Add(groupInput);

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
                ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText
            };

            dgvEntries.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Name = "Name",
                FillWeight = 2
            });
            dgvEntries.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "URL",
                Name = "Url",
                FillWeight = 3
            });
            dgvEntries.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Цена",
                Name = "Price",
                FillWeight = 1
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

            btnStartChecking = new Button
            {
                Text = "Начать проверку",
                Location = new Point(10, dgvEntries.Bottom + 10),
                Size = new Size(200, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            btnStartChecking.Click += BtnStartChecking_Click;

            this.Controls.Add(dgvEntries);
            this.Controls.Add(btnStartChecking);
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
            priceCheckTimer = new System.Timers.Timer(3600 * 1000);
            priceCheckTimer.Elapsed += async (s, e) =>
            {
                try
                {
                    await StartPriceChecking();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка в таймере: " + ex.Message);
                }
            };
            priceCheckTimer.AutoReset = true;
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

            dgvEntries.Rows.Add(txtName.Text.Trim(), txtUrl.Text.Trim(), targetPrice.ToString());
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

        private async Task<string> GetHtmlUsingSeleniumAsync(string url)
        {
            return await Task.Run(() =>
            {
                var service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;

                var options = new ChromeOptions();
                options.AddArgument("--headless");
                options.AddArgument("--disable-gpu");
                options.AddArgument("--window-size=1920,1080");
                options.AddExcludedArgument("enable-automation");
                options.AddAdditionalOption("useAutomationExtension", false);
                options.AddArgument("--disable-blink-features=AutomationControlled");
                options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                    "AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36");

                using (var driver = new ChromeDriver(service, options))
                {
                    driver.ExecuteScript("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");
                    driver.Navigate().GoToUrl(url);
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                    wait.Until(d => !d.Title.Contains("Just a moment") && !d.PageSource.Contains("Cloudflare"));
                    return driver.PageSource;
                }
            });
        }

        private async Task<string> GetHtmlWithoutSeleniumAsync(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent",
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                        "AppleWebKit/537.36 (KHTML, like Gecko) " +
                        "Chrome/90.0.4430.93 Safari/537.36");

                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string html = await response.Content.ReadAsStringAsync();
                    return html;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении HTML: " + ex.Message);
                return string.Empty;
            }
        }

        private async Task StartPriceChecking()
        {
            List<Task> tasks = new List<Task>();

            var rows = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dgvEntries.Rows)
                rows.Add(row);

            foreach (var row in rows)
            {
                tasks.Add(Task.Run(async () =>
                {
                    string url = row.Cells["Url"].Value?.ToString();
                    string targetPriceStr = row.Cells["Price"].Value?.ToString();

                    if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(targetPriceStr))
                        return;

                    if (!decimal.TryParse(targetPriceStr, out decimal targetPrice))
                        return;

                    decimal actualPrice = await GetPriceFromUrl(url);

                    if (actualPrice < targetPrice)
                    {
                        this.Invoke(new Action(() =>
                        {
                            ShowNotification("Снижение цены",
                                $"Цена по URL:\n{url}\nопустилась ниже {targetPrice}.\nТекущая цена: {actualPrice}", url);
                        }));
                    }
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
        }

        private async Task<decimal> GetPriceFromUrl(string url)
        {
            try
            {
                string html;
                if (url.ToLower().Contains("ceneo"))
                {
                    html = await GetHtmlUsingSeleniumAsync(url);
                }
                else
                {
                    html = await GetHtmlWithoutSeleniumAsync(url);
                }
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                if (url.ToLower().Contains("amazon"))
                {
                    var priceNode = doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'a-price-whole')]");
                    if (priceNode != null)
                    {
                        string priceText = priceNode.InnerText.Trim();
                        string cleaned = new string(priceText.Where(ch => char.IsDigit(ch) || ch == '.' || ch == ',').ToArray());
                        cleaned = cleaned.Replace(',', '.');
                        if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                        {
                            return price;
                        }
                    }
                }
                else if (url.ToLower().Contains("nbsklep"))
                {
                    var priceNode = doc.DocumentNode.SelectSingleNode("//*[@data-qa-product_price]");
                    if (priceNode != null)
                    {
                        string priceAttr = priceNode.GetAttributeValue("data-qa-product_price", "");
                        if (!string.IsNullOrEmpty(priceAttr))
                        {
                            if (decimal.TryParse(priceAttr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                            {
                                return price;
                            }
                        }
                    }
                }
                else if (url.ToLower().Contains("ceneo"))
                {
                    var valueNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'product-top__price')]//span[@class='value']");
                    var pennyNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'product-top__price')]//span[@class='penny']");

                    if (valueNode != null && pennyNode != null)
                    {
                        string priceText = valueNode.InnerText.Trim() + pennyNode.InnerText.Trim();
                        string numericPart = new string(priceText.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray());
                        if (decimal.TryParse(numericPart, NumberStyles.Any, new CultureInfo("pl-PL"), out decimal price))
                        {
                            return price;
                        }
                    }
                }
                else if (url.ToLower().Contains("x-kom"))
                {
                    var priceNode = doc.DocumentNode.SelectSingleNode("//span[contains(@class, 'parts__Price-sc-53da58c9-2')]");
                    if (priceNode != null)
                    {
                        string priceText = priceNode.InnerText.Trim();
                        string numericPart = new string(priceText.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray());
                        if (decimal.TryParse(numericPart, NumberStyles.Any, new CultureInfo("pl-PL"), out decimal price))
                        {
                            return price;
                        }
                    }
                }
                else if (url.ToLower().Contains("morele.net"))
                {
                    var priceNode = doc.DocumentNode.SelectSingleNode("//div[@id='product_price']");
                    if (priceNode != null)
                    {
                        string dataPrice = priceNode.GetAttributeValue("data-price", "");
                        if (!string.IsNullOrEmpty(dataPrice))
                        {
                            if (decimal.TryParse(dataPrice, System.Globalization.NumberStyles.Any,
                                                 System.Globalization.CultureInfo.InvariantCulture, out decimal price))
                            {
                                return price;
                            }
                        }
                    }
                }
                else
                {
                    var priceNode = doc.DocumentNode.SelectSingleNode("//span[@id='price']");
                    if (priceNode != null)
                    {
                        string priceText = priceNode.InnerText.Trim();
                        string cleaned = new string(priceText.Where(ch => char.IsDigit(ch) || ch == '.' || ch == ',').ToArray());
                        cleaned = cleaned.Replace(',', '.');
                        if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                        {
                            return price;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении цены: " + ex.Message);
            }
            return decimal.MaxValue;
        }

        private void ShowNotification(string title, string message, string url)
        {
            currentNotificationUrl = url;
            trayIcon.BalloonTipTitle = title;
            trayIcon.BalloonTipText = message;
            trayIcon.ShowBalloonTip(3000);
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
