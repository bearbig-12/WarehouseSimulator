namespace WarehouseWinForms.Forms
{
    partial class MoveForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            tableLayout = new TableLayoutPanel();
            lblInfo     = new Label();
            lblShelf    = new Label();
            cmbShelf    = new ComboBox();
            lblFloor    = new Label();
            cmbFloor    = new ComboBox();
            lblSlot     = new Label();
            cmbSlot     = new ComboBox();
            btnOk       = new Button();
            tableLayout.SuspendLayout();
            SuspendLayout();

            // tableLayout
            tableLayout.ColumnCount = 2;
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            tableLayout.RowCount = 5;
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayout.Dock    = DockStyle.Fill;
            tableLayout.Padding = new Padding(8);

            // lblInfo (컨테이너 ID 표시, 2열 전체)
            lblInfo.Dock      = DockStyle.Fill;
            lblInfo.TextAlign = ContentAlignment.MiddleCenter;
            lblInfo.Font      = new Font("맑은 고딕", 11F, FontStyle.Bold);
            tableLayout.Controls.Add(lblInfo, 0, 0);
            tableLayout.SetColumnSpan(lblInfo, 2);

            // 라벨들
            lblShelf.Dock      = DockStyle.Fill;
            lblShelf.TextAlign = ContentAlignment.MiddleLeft;
            lblShelf.Font      = new Font("맑은 고딕", 10F);
            lblShelf.Text      = "선반";

            lblFloor.Dock      = DockStyle.Fill;
            lblFloor.TextAlign = ContentAlignment.MiddleLeft;
            lblFloor.Font      = new Font("맑은 고딕", 10F);
            lblFloor.Text      = "층";

            lblSlot.Dock       = DockStyle.Fill;
            lblSlot.TextAlign  = ContentAlignment.MiddleLeft;
            lblSlot.Font       = new Font("맑은 고딕", 10F);
            lblSlot.Text       = "슬롯";

            // 콤보박스들
            cmbShelf.Dock          = DockStyle.Fill;
            cmbShelf.Font          = new Font("맑은 고딕", 10F);
            cmbShelf.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFloor.Dock          = DockStyle.Fill;
            cmbFloor.Font          = new Font("맑은 고딕", 10F);
            cmbFloor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSlot.Dock           = DockStyle.Fill;
            cmbSlot.Font           = new Font("맑은 고딕", 10F);
            cmbSlot.DropDownStyle  = ComboBoxStyle.DropDownList;
            cmbShelf.Items.AddRange(new object[] { "A", "B", "C", "D" });
            cmbFloor.Items.AddRange(new object[] { 0, 1, 2 });
            cmbSlot.Items.AddRange(new object[] { 0, 1, 2, 3, 4, 5, 6, 7 });

            tableLayout.Controls.Add(lblShelf, 0, 1);
            tableLayout.Controls.Add(cmbShelf, 1, 1);
            tableLayout.Controls.Add(lblFloor, 0, 2);
            tableLayout.Controls.Add(cmbFloor, 1, 2);
            tableLayout.Controls.Add(lblSlot,  0, 3);
            tableLayout.Controls.Add(cmbSlot,  1, 3);
            tableLayout.Controls.Add(btnOk,    0, 4);
            tableLayout.SetColumnSpan(btnOk, 2);

            // 확인 버튼
            btnOk.Dock         = DockStyle.Fill;
            btnOk.Text         = "이동";
            btnOk.Font         = new Font("맑은 고딕", 11F);
            btnOk.BackColor    = Color.FromArgb(128, 128, 255);
            btnOk.DialogResult = DialogResult.OK;

            // Form
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(340, 240);
            Controls.Add(tableLayout);
            Text          = "이동";
            StartPosition = FormStartPosition.CenterParent;
            AcceptButton  = btnOk;

            tableLayout.ResumeLayout(false);
            ResumeLayout(false);
        }

        private TableLayoutPanel tableLayout;
        private Label    lblInfo, lblShelf, lblFloor, lblSlot;
        private ComboBox cmbShelf, cmbFloor, cmbSlot;
        private Button   btnOk;
    }
}
