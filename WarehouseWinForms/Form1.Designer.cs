namespace WarehouseWinForms
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            topPanel = new Panel();
            lblStatus = new Label();
            botPanel = new Panel();
            btnRefresh = new Button();
            btnOutgoing = new Button();
            btnMove = new Button();
            btnIncoming = new Button();
            dataGrid = new DataGridView();
            topPanel.SuspendLayout();
            botPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGrid).BeginInit();
            SuspendLayout();
            // 
            // topPanel
            // 
            topPanel.BackColor = SystemColors.ControlDarkDark;
            topPanel.Controls.Add(lblStatus);
            topPanel.Dock = DockStyle.Top;
            topPanel.Location = new Point(0, 0);
            topPanel.Name = "topPanel";
            topPanel.Size = new Size(984, 42);
            topPanel.TabIndex = 0;
            // 
            // lblStatus
            // 
            lblStatus.Dock = DockStyle.Left;
            lblStatus.Font = new Font("Segoe UI", 11F);
            lblStatus.ForeColor = Color.Yellow;
            lblStatus.Location = new Point(0, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Padding = new Padding(10, 0, 0, 0);
            lblStatus.Size = new Size(260, 42);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "● 서버 연결 중...";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // botPanel
            // 
            botPanel.BackColor = SystemColors.ControlDark;
            botPanel.Controls.Add(btnRefresh);
            botPanel.Controls.Add(btnOutgoing);
            botPanel.Controls.Add(btnMove);
            botPanel.Controls.Add(btnIncoming);
            botPanel.Dock = DockStyle.Bottom;
            botPanel.Location = new Point(0, 506);
            botPanel.Name = "botPanel";
            botPanel.Size = new Size(984, 55);
            botPanel.TabIndex = 1;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.FromArgb(192, 192, 255);
            btnRefresh.Font = new Font("맑은 고딕", 11F);
            btnRefresh.Location = new Point(392, 10);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(120, 36);
            btnRefresh.TabIndex = 3;
            btnRefresh.Text = "새로고침";
            btnRefresh.UseVisualStyleBackColor = false;
            // 
            // btnOutgoing
            // 
            btnOutgoing.BackColor = Color.Red;
            btnOutgoing.Enabled = false;
            btnOutgoing.Font = new Font("맑은 고딕", 11F);
            btnOutgoing.Location = new Point(266, 10);
            btnOutgoing.Name = "btnOutgoing";
            btnOutgoing.Size = new Size(120, 36);
            btnOutgoing.TabIndex = 2;
            btnOutgoing.Text = "출하";
            btnOutgoing.UseVisualStyleBackColor = false;
            // 
            // btnMove
            // 
            btnMove.BackColor = Color.FromArgb(128, 128, 255);
            btnMove.Enabled = false;
            btnMove.Font = new Font("맑은 고딕", 11F);
            btnMove.Location = new Point(140, 10);
            btnMove.Name = "btnMove";
            btnMove.Size = new Size(120, 36);
            btnMove.TabIndex = 1;
            btnMove.Text = "이동";
            btnMove.UseVisualStyleBackColor = false;
            // 
            // btnIncoming
            // 
            btnIncoming.BackColor = Color.FromArgb(128, 255, 128);
            btnIncoming.Font = new Font("맑은 고딕", 11F);
            btnIncoming.Location = new Point(12, 10);
            btnIncoming.Name = "btnIncoming";
            btnIncoming.Size = new Size(120, 36);
            btnIncoming.TabIndex = 0;
            btnIncoming.Text = "입고";
            btnIncoming.UseVisualStyleBackColor = false;
            // 
            // dataGrid
            // 
            dataGrid.AllowUserToAddRows = false;
            dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGrid.BackgroundColor = Color.White;
            dataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Window;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 10F);
            dataGridViewCellStyle1.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.False;
            dataGrid.DefaultCellStyle = dataGridViewCellStyle1;
            dataGrid.Dock = DockStyle.Fill;
            dataGrid.Location = new Point(0, 42);
            dataGrid.MultiSelect = false;
            dataGrid.Name = "dataGrid";
            dataGrid.ReadOnly = true;
            dataGrid.RowHeadersVisible = false;
            dataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGrid.Size = new Size(984, 464);
            dataGrid.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 561);
            Controls.Add(dataGrid);
            Controls.Add(botPanel);
            Controls.Add(topPanel);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "창고 관리 시스템";
            topPanel.ResumeLayout(false);
            botPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGrid).EndInit();
            ResumeLayout(false);
        }

        private Panel topPanel;
        private Label lblStatus;
        private Panel botPanel;
        private DataGridView dataGrid;
        private Button btnOutgoing;
        private Button btnRefresh;
        private Button btnMove;
        private Button btnIncoming;
    }
}
