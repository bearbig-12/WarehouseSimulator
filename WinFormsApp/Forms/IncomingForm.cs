using WarehouseWinForms.Models;

namespace WarehouseWinForms.Forms
{
    public class IncomingForm : Form
    {
        public ContainerModel? Result { get; private set; }

        private TextBox txtId, txtName, txtWeight, txtShelf, txtFloor, txtSlot, txtWidth, txtDepth, txtHeight;

        public IncomingForm()
        {
            Text            = "컨테이너 입고";
            Size            = new Size(360, 420);
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;

            var panel = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 2,
                RowCount    = 10,
                Padding     = new Padding(10),
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,  100));
            Controls.Add(panel);

            txtId     = AddRow(panel, 0, "컨테이너 ID (CNT-XXX)");
            txtName   = AddRow(panel, 1, "물건 이름");
            txtWeight = AddRow(panel, 2, "무게 (kg)");
            txtShelf  = AddRow(panel, 3, "선반 (A~D)");
            txtFloor  = AddRow(panel, 4, "층 (0~2)");
            txtSlot   = AddRow(panel, 5, "슬롯 (0~7)");
            txtWidth  = AddRow(panel, 6, "가로 (m)");
            txtDepth  = AddRow(panel, 7, "세로 (m)");
            txtHeight = AddRow(panel, 8, "높이 (m)");

            var btnOk = new Button { Text = "입고", Dock = DockStyle.Fill, BackColor = Color.Green, ForeColor = Color.White };
            btnOk.Click += BtnOk_Click;
            panel.Controls.Add(btnOk);
            panel.SetCellPosition(btnOk, new TableLayoutPanelCellPosition(0, 9));
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
            // ID 형식 검사 CNT-000
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtId.Text.Trim(), @"^CNT-\d{3}$"))
            {
                MessageBox.Show("ID 형식은 CNT-000 이어야 합니다.", "입력 오류");
                return;
            }
            if (!float.TryParse(txtWeight.Text, out float weight) ||
                !float.TryParse(txtWidth.Text,  out float width)  ||
                !float.TryParse(txtDepth.Text,  out float depth)  ||
                !float.TryParse(txtHeight.Text, out float height) ||
                !int.TryParse(txtFloor.Text,    out int floor)    ||
                !int.TryParse(txtSlot.Text,     out int slot))
            {
                MessageBox.Show("숫자 형식을 확인해주세요.", "입력 오류");
                return;
            }
            if (width > 5 || depth > 5 || height > 5)
            {
                MessageBox.Show("박스 크기는 5x5x5를 초과할 수 없습니다.", "입력 오류");
                return;
            }

            Result = new ContainerModel
            {
                ContainerId = txtId.Text.Trim().ToUpper(),
                ItemName    = txtName.Text.Trim(),
                Weight      = weight,
                ArrivalDate = DateTime.Today.ToString("yyyy-MM-dd"),
                Shelf       = txtShelf.Text.Trim().ToUpper(),
                Floor       = floor,
                Slot        = slot,
                Width       = width,
                Depth       = depth,
                Height      = height
            };
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
