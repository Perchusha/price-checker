namespace PriceChecker
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.GroupBox groupInput;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblUrl;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.TextBox txtTargetPrice;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DataGridView dgvEntries;
        private System.Windows.Forms.Button btnStartChecking;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem settingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem supportedSitesMenuItem;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.components = new System.ComponentModel.Container();

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 508);
            this.MinimumSize = new System.Drawing.Size(420, 340);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Text = "Price Checker";
            this.DoubleBuffered = true;

            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.settingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsMenuItem.Text = "Настройки";

            this.supportedSitesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.supportedSitesMenuItem.Text = "Поддерживаемые сайты";

            var siteAmazon = new System.Windows.Forms.ToolStripMenuItem();
            siteAmazon.Text = "Amazon";
            var siteNBsklep = new System.Windows.Forms.ToolStripMenuItem();
            siteNBsklep.Text = "NBsklep";
            var siteCeneo = new System.Windows.Forms.ToolStripMenuItem();
            siteCeneo.Text = "Ceneo";
            var siteMorele = new System.Windows.Forms.ToolStripMenuItem();
            siteMorele.Text = "Morele";
            var siteXKom = new System.Windows.Forms.ToolStripMenuItem();
            siteXKom.Text = "X-kom";
            var siteYesStyle = new System.Windows.Forms.ToolStripMenuItem();
            siteYesStyle.Text = "YesStyle";
            var siteIkea = new System.Windows.Forms.ToolStripMenuItem();
            siteIkea.Text = "Ikea";

            this.supportedSitesMenuItem.DropDownItems.Add(siteAmazon);
            this.supportedSitesMenuItem.DropDownItems.Add(siteNBsklep);
            this.supportedSitesMenuItem.DropDownItems.Add(siteCeneo);
            this.supportedSitesMenuItem.DropDownItems.Add(siteMorele);
            this.supportedSitesMenuItem.DropDownItems.Add(siteXKom);
            this.supportedSitesMenuItem.DropDownItems.Add(siteYesStyle);
            this.supportedSitesMenuItem.DropDownItems.Add(siteIkea);

            this.mainMenu.Items.Add(this.settingsMenuItem);
            this.mainMenu.Items.Add(this.supportedSitesMenuItem);

            this.MainMenuStrip = this.mainMenu;
            this.Controls.Add(this.mainMenu);

            this.groupInput = new System.Windows.Forms.GroupBox();
            this.groupInput.Text = "Новый товар";
            this.groupInput.Location = new System.Drawing.Point(10, 30);
            this.groupInput.Size = new System.Drawing.Size(this.ClientSize.Width - 20, 50);
            this.groupInput.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.Controls.Add(this.groupInput);

            this.lblName = new System.Windows.Forms.Label();
            this.lblName.Text = "Имя:";
            this.lblName.Location = new System.Drawing.Point(10, 25);
            this.lblName.AutoSize = true;
            this.groupInput.Controls.Add(this.lblName);

            this.txtName = new System.Windows.Forms.TextBox();
            this.txtName.Location = new System.Drawing.Point(this.lblName.Right + 5, 20);
            this.txtName.Size = new System.Drawing.Size(140, 25);
            this.groupInput.Controls.Add(this.txtName);

            this.lblUrl = new System.Windows.Forms.Label();
            this.lblUrl.Text = "Ссылка:";
            this.lblUrl.Location = new System.Drawing.Point(this.txtName.Right + 10, 25);
            this.lblUrl.AutoSize = true;
            this.groupInput.Controls.Add(this.lblUrl);

            this.txtUrl = new System.Windows.Forms.TextBox();
            this.txtUrl.Location = new System.Drawing.Point(this.lblUrl.Right + 5, 20);
            this.txtUrl.Size = new System.Drawing.Size(220, 25);
            this.groupInput.Controls.Add(this.txtUrl);

            this.lblPrice = new System.Windows.Forms.Label();
            this.lblPrice.Text = "Целевая цена:";
            this.lblPrice.Location = new System.Drawing.Point(this.txtUrl.Right + 10, 25);
            this.lblPrice.AutoSize = true;
            this.groupInput.Controls.Add(this.lblPrice);

            this.txtTargetPrice = new System.Windows.Forms.TextBox();
            this.txtTargetPrice.Location = new System.Drawing.Point(this.lblPrice.Right + 5, 20);
            this.txtTargetPrice.Size = new System.Drawing.Size(80, 25);
            this.groupInput.Controls.Add(this.txtTargetPrice);

            this.btnAdd = new System.Windows.Forms.Button();
            this.btnAdd.Text = "Добавить";
            this.btnAdd.Location = new System.Drawing.Point(this.txtTargetPrice.Right + 5, 20);
            this.btnAdd.AutoSize = true;
            this.groupInput.Controls.Add(this.btnAdd);

            this.dgvEntries = new System.Windows.Forms.DataGridView();
            this.dgvEntries.Location = new System.Drawing.Point(10, this.groupInput.Bottom + 10);
            this.dgvEntries.Size = new System.Drawing.Size(780, 372);
            this.dgvEntries.AllowUserToAddRows = false;
            this.dgvEntries.RowHeadersVisible = false;
            this.dgvEntries.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvEntries.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvEntries.MultiSelect = false;
            this.dgvEntries.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.dgvEntries.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;

            var colCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            colCheck.HeaderText = "";
            colCheck.Name = "Enabled";
            colCheck.FillWeight = 0.5f;
            colCheck.TrueValue = true;
            colCheck.FalseValue = false;
            this.dgvEntries.Columns.Add(colCheck);

            var colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colName.HeaderText = "Имя";
            colName.Name = "Name";
            colName.FillWeight = 2;
            this.dgvEntries.Columns.Add(colName);

            var colUrl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colUrl.HeaderText = "Ссылка";
            colUrl.Name = "Url";
            colUrl.FillWeight = 3;
            this.dgvEntries.Columns.Add(colUrl);

            var colPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colPrice.HeaderText = "Цена";
            colPrice.Name = "Price";
            colPrice.FillWeight = 1;
            colPrice.ValueType = typeof(decimal);
            this.dgvEntries.Columns.Add(colPrice);

            var colDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            colDelete.HeaderText = "Действие";
            colDelete.Name = "Delete";
            colDelete.Text = "Удалить";
            colDelete.UseColumnTextForButtonValue = true;
            colDelete.FillWeight = 1;
            this.dgvEntries.Columns.Add(colDelete);

            this.Controls.Add(this.dgvEntries);

            this.btnStartChecking = new System.Windows.Forms.Button();
            this.btnStartChecking.Text = "Начать проверку";
            this.btnStartChecking.Location = new System.Drawing.Point(10, this.dgvEntries.Bottom + 10);
            this.btnStartChecking.Size = new System.Drawing.Size(200, 30);
            this.btnStartChecking.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.Controls.Add(this.btnStartChecking);

            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressBar.Location = new System.Drawing.Point(220, this.dgvEntries.Bottom + 10);
            this.progressBar.Size = new System.Drawing.Size(300, 30);
            this.progressBar.Visible = false;
            this.progressBar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.Controls.Add(this.progressBar);

            this.lblStatus = new System.Windows.Forms.Label();
            this.lblStatus.AutoSize = true;
            this.lblStatus.Text = "Статус: Ожидание\nПоследняя проверка: -";
            this.lblStatus.Location = new System.Drawing.Point(this.ClientSize.Width - 180, this.dgvEntries.Bottom + 10);
            this.lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.Controls.Add(this.lblStatus);

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
