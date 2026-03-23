using System.Net.WebSockets;
using System.Text;
using WarehouseWinForms.Forms;
using WarehouseWinForms.Models;
using WarehouseWinForms.Services;

namespace WarehouseWinForms
{
    public partial class Form1 : Form
    {
        private readonly ApiService _api = new();
        private ClientWebSocket? _ws;
        private CancellationTokenSource _wsCts = new();
        private List<ContainerModel> _containers = new();

        public Form1()
        {
            InitializeComponent();
            Load                      += Form1_Load;
            btnRefresh.Click          += BtnRefresh_Click;
            btnIncoming.Click         += BtnIncoming_Click;
            btnMove.Click             += BtnMove_Click;
            btnOutgoing.Click         += BtnOutgoing_Click;
            dataGrid.SelectionChanged += DataGrid_SelectionChanged;
        }

        private async void Form1_Load(object? sender, EventArgs e)
        {
            await LoadDataAsync();
            await ConnectSocketAsync();
        }

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async void BtnIncoming_Click(object? sender, EventArgs e)
        {
            await OnIncomingAsync();
        }

        private async void BtnMove_Click(object? sender, EventArgs e)
        {
            await OnMoveAsync();
        }

        private async void BtnOutgoing_Click(object? sender, EventArgs e)
        {
            await OnOutgoingAsync();
        }

        private void DataGrid_SelectionChanged(object? sender, EventArgs e)
        {
            UpdateButtons();
        }

        // ── 데이터 로드 ──────────────────────────────
        private async Task LoadDataAsync()
        {
            try
            {
                _containers = await _api.GetAllAsync();
                var source = new BindingSource { DataSource = _containers };
                dataGrid.DataSource = source;

                foreach (DataGridViewColumn col in dataGrid.Columns)
                    col.Visible = false;

                SetColumn("ContainerId", "ID",       80);
                SetColumn("ItemName",    "품목",     120);
                SetColumn("Weight",      "중량(kg)",  80);
                SetColumn("ArrivalDate", "입고일",   100);
                SetColumn("Location",    "위치",      80);

                UpdateButtons();
            }
            catch
            {
                lblStatus.Text      = "● 서버 연결 실패";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void SetColumn(string name, string header, int fillWeight)
        {
            if (!dataGrid.Columns.Contains(name)) return;
            var col = dataGrid.Columns[name];
            col.Visible    = true;
            col.HeaderText = header;
            col.FillWeight = fillWeight;
        }

        // ── WebSocket 연결 ───────────────────────────
        private async Task ConnectSocketAsync()
        {
            _ws = new ClientWebSocket();
            try
            {
                await _ws.ConnectAsync(new Uri("ws://localhost:3000"), _wsCts.Token);
                SafeInvoke(() =>
                {
                    lblStatus.Text      = "● 서버 연결됨";
                    lblStatus.ForeColor = Color.LimeGreen;
                });
                _ = ReceiveLoopAsync();
            }
            catch
            {
                SafeInvoke(() =>
                {
                    lblStatus.Text      = "● 서버 연결 실패";
                    lblStatus.ForeColor = Color.Red;
                });
            }
        }

        private async Task ReceiveLoopAsync()
        {
            var buffer = new byte[4096];
            try
            {
                while (_ws?.State == WebSocketState.Open)
                {
                    var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), _wsCts.Token);
                    if (result.MessageType == WebSocketMessageType.Close) break;
                    // containerAdded / containerMoved / containerRemoved 어떤 이벤트든 새로고침
                    await OnSocketEventAsync();
                }
            }
            catch { }

            SafeInvoke(() =>
            {
                lblStatus.Text      = "● 서버 연결 끊김";
                lblStatus.ForeColor = Color.Red;
            });
        }

        // ── 버튼 활성화 ──────────────────────────────
        private void UpdateButtons()
        {
            bool selected       = dataGrid.SelectedRows.Count > 0;
            btnMove.Enabled     = selected;
            btnOutgoing.Enabled = selected;
        }

        private ContainerModel? SelectedContainer()
        {
            if (dataGrid.SelectedRows.Count == 0) return null;
            return dataGrid.SelectedRows[0].DataBoundItem as ContainerModel;
        }

        // ── 입고 ─────────────────────────────────────
        private async Task OnIncomingAsync()
        {
            using var dlg = new IncomingForm();
            if (dlg.ShowDialog() != DialogResult.OK) return;

            // 중복 ID 체크
            bool exists = false;
            foreach (var x in _containers)
            {
                if (x.ContainerId == dlg.Result!.ContainerId)
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
            {
                MessageBox.Show($"{dlg.Result!.ContainerId} 은(는) 이미 존재합니다.", "중복 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 슬롯 중복 체크
            bool slotOccupied = false;
            foreach (var x in _containers)
            {
                if (x.Shelf == dlg.Result!.Shelf && x.Floor == dlg.Result.Floor && x.Slot == dlg.Result.Slot)
                {
                    slotOccupied = true;
                    break;
                }
            }

            if (slotOccupied)
            {
                MessageBox.Show("해당 슬롯에 이미 컨테이너가 있습니다.", "슬롯 중복", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool ok = await _api.IncomingAsync(dlg.Result!);
            if (ok) await LoadDataAsync();
            else MessageBox.Show("입고 실패", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // ── 이동 ─────────────────────────────────────
        private async Task OnMoveAsync()
        {
            var c = SelectedContainer();
            if (c == null) return;

            using var dlg = new MoveForm(c);
            if (dlg.ShowDialog() != DialogResult.OK) return;

            // 목적지 슬롯 중복 체크
            bool occupied = false;
            foreach (var x in _containers)
            {
                if (x.Shelf == dlg.Shelf && x.Floor == dlg.Floor && x.Slot == dlg.Slot)
                {
                    occupied = true;
                    break;
                }
            }

            if (occupied)
            {
                MessageBox.Show("해당 위치에 이미 컨테이너가 있습니다.", "이동 불가", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool ok = await _api.MoveAsync(c.ContainerId, dlg.Shelf, dlg.Floor, dlg.Slot);
            if (ok) await LoadDataAsync();
            else MessageBox.Show("이동 실패", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // ── 출하 ─────────────────────────────────────
        private async Task OnOutgoingAsync()
        {
            var c = SelectedContainer();
            if (c == null) return;

            var confirm = MessageBox.Show(
                $"{c.ContainerId} 을(를) 출하하시겠습니까?",
                "출하 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            bool ok = await _api.OutgoingAsync(c.ContainerId);
            if (ok) await LoadDataAsync();
            else MessageBox.Show("출하 실패", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // ── 유틸 ─────────────────────────────────────
        private void SafeInvoke(Action action)
        {
            if (IsDisposed || !IsHandleCreated) return;
            try { Invoke(action); } catch { }
        }

        private async Task OnSocketEventAsync()
        {
            if (IsDisposed || !IsHandleCreated) return;
            try { await Invoke(async () => await LoadDataAsync()); } catch { }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _wsCts.Cancel();
            _ws?.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            base.OnFormClosing(e);
        }
    }
}
