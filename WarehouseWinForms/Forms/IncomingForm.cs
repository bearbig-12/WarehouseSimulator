using System.Text.RegularExpressions;
using WarehouseWinForms.Models;

namespace WarehouseWinForms.Forms
{
    public partial class IncomingForm : Form
    {
        public ContainerModel? Result { get; private set; }

        public IncomingForm()
        {
            InitializeComponent();
            btnOk.Click += BtnOk_Click;
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            // ID 형식 검증 (CNT-000 ~ CNT-999)
            if (!Regex.IsMatch(txtId.Text.Trim(), @"^CNT-\d{3}$"))
            {
                MessageBox.Show("ID는 CNT-000 형식이어야 합니다.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            // 품목명
            if (string.IsNullOrWhiteSpace(txtItemName.Text))
            {
                MessageBox.Show("품목명을 입력하세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            // 중량
            if (!float.TryParse(txtWeight.Text, out float weight) || weight <= 0)
            {
                MessageBox.Show("중량은 0보다 큰 숫자여야 합니다.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            // 선반/층/슬롯
            if (cmbShelf.SelectedItem == null || cmbFloor.SelectedItem == null || cmbSlot.SelectedItem == null)
            {
                MessageBox.Show("선반, 층, 슬롯을 선택하세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            // 크기 (최대 5x5x5)
            if (!float.TryParse(txtWidth.Text,  out float width)  || width  <= 0) width  = 1f;
            if (!float.TryParse(txtDepth.Text,  out float depth)  || depth  <= 0) depth  = 1f;
            if (!float.TryParse(txtHeight.Text, out float height) || height <= 0) height = 1f;
            width  = Math.Min(width,  5f);
            depth  = Math.Min(depth,  5f);
            height = Math.Min(height, 5f);

            Result = new ContainerModel
            {
                ContainerId = txtId.Text.Trim(),
                ItemName    = txtItemName.Text.Trim(),
                Weight      = weight,
                ArrivalDate = dtpArrival.Value.ToString("yyyy-MM-dd"),
                Shelf       = cmbShelf.SelectedItem.ToString()!,
                Floor       = (int)cmbFloor.SelectedItem,
                Slot        = (int)cmbSlot.SelectedItem,
                Width       = width,
                Depth       = depth,
                Height      = height
            };
        }
    }
}
