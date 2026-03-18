using WarehouseWinForms.Models;

namespace WarehouseWinForms.Forms
{
    public partial class MoveForm : Form
    {
        public string Shelf { get; private set; } = "";
        public int    Floor { get; private set; }
        public int    Slot  { get; private set; }

        public MoveForm(ContainerModel container)
        {
            InitializeComponent();
            lblInfo.Text = $"컨테이너: {container.ContainerId}  현재 위치: {container.Location}";
            btnOk.Click += BtnOk_Click;
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (cmbShelf.SelectedItem == null || cmbFloor.SelectedItem == null || cmbSlot.SelectedItem == null)
            {
                MessageBox.Show("선반, 층, 슬롯을 선택하세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            Shelf = cmbShelf.SelectedItem.ToString()!;
            Floor = (int)cmbFloor.SelectedItem;
            Slot  = (int)cmbSlot.SelectedItem;
        }
    }
}
