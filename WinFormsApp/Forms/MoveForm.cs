namespace WarehouseWinForms.Forms
{
    public class MoveForm : Form
    {
        public string TargetShelf { get; private set; } = "";
        public int    TargetFloor { get; private set; }
        public int    TargetSlot  { get; private set; }

        private TextBox txtShelf, txtFloor, txtSlot;

        public MoveForm(string currentLocation)
        {
            Text            = $"컨테이너 이동 (현재: {currentLocation})";
            Size            = new Size(300, 220);
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;

            var panel = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 2,
                RowCount    = 4,
                Padding     = new Padding(10),
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            Controls.Add(panel);

            txtShelf = AddRow(panel, 0, "이동할 선반 (A~D)");
            txtFloor = AddRow(panel, 1, "이동할 층 (0~2)");
            txtSlot  = AddRow(panel, 2, "이동할 슬롯 (0~7)");

            var btnOk = new Button { Text = "이동", Dock = DockStyle.Fill, BackColor = Color.RoyalBlue, ForeColor = Color.White };
            btnOk.Click += BtnOk_Click;
            panel.Controls.Add(btnOk);
            panel.SetCellPosition(btnOk, new TableLayoutPanelCellPosition(0, 3));
            panel.SetColumnSpan(btnOk, 2);
        }

        private TextBox AddRow(TableLayoutPanel panel, int row, string label)
        {
            panel.Controls.Add(new Label { Text = label, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill });
            panel.SetCellPosition(panel.Controls[panel.Controls.Count - 1], new TableLayoutPanelCellPosition(0, row));
            var txt = new TextBox { Dock = DockStyle.Fill };
            panel.Controls.Add(txt);
            panel.SetCellPosition(txt, new TableLayoutPanelCellPosition(1, row));
            return txt;
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtFloor.Text, out int floor) ||
                !int.TryParse(txtSlot.Text,  out int slot)  ||
                string.IsNullOrWhiteSpace(txtShelf.Text))
            {
                MessageBox.Show("값을 올바르게 입력해주세요.", "입력 오류");
                return;
            }
            TargetShelf  = txtShelf.Text.Trim().ToUpper();
            TargetFloor  = floor;
            TargetSlot   = slot;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
