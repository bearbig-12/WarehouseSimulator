using WarehouseWinForms.Forms;
using WarehouseWinForms.Models;
using WarehouseWinForms.Services;

namespace WarehouseWinForms
{
    public partial class Form1 : Form
    {
        private readonly ApiService _api = new();
        private SocketIOClient.SocketIO? _socket;
        private BindingSource _binding = new();

        // ───────── 컨트롤 ─────────
        private DataGridView grid       = new();
        private Label        lblStatus  = new();
        private Button       btnIn      = new();
        private Button       btnMove    = new();
        private Button       btnOut     = new();
        private Button       btnRefresh = new();

        public Form1()
        {
            InitializeComponent();
            BuildUI();
            _ = InitAsync();
        }

        // ─────────────────────────────────────────
        // UI 구성
        // ─────────────────────────────────────────
        private void BuildUI()
        {
            Text          = "창고 관리 시스템";
            Size          = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterScreen;

            // 상단 상태바
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.FromArgb(30, 30, 30) };
            lblStatus = new Label
            {
                Text      = "● 서버 연결 중...",
                ForeColor = Color.Yellow,
                Font      = new Font("Segoe UI", 11),
                Dock      = DockStyle.Left,
                AutoSize  = false,
                Width     = 250,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(10, 0, 0, 0)
            };
            topPanel.Controls.Add(lblStatus);
            Controls.Add(topPanel);

            // DataGridView
            grid = new DataGridView
            {
                Dock                = DockStyle.Fill,
                ReadOnly            = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode       = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect         = false,
                AllowUserToAddRows  = false,
                BackgroundColor     = Color.White,
                RowHeadersVisible   = false,
                Font                = new Font("Segoe UI", 10)
            };
            grid.SelectionChanged += Grid_SelectionChanged;
            Controls.Add(grid);

            // 하단 버튼 패널
            var botPanel = new Panel { Dock = DockStyle.Bottom, Height = 55, BackColor = Color.FromArgb(45, 45, 48) };

            btnIn = MakeButton("입고", Color.Green);
            btnIn.Click += BtnIn_Click;

            btnMove = MakeButton("이동", Color.RoyalBlue);
            btnMove.Click   += BtnMove_Click;
            btnMove.Enabled  = false;

            btnOut = MakeButton("출하", Color.Crimson);
            btnOut.Click   += BtnOut_Click;
            btnOut.Enabled  = false;

            btnRefresh = MakeButton("새로고침", Color.Gray);
            btnRefresh.Click += async (s, e) => await LoadDataAsync();

            var flow = new FlowLayoutPanel
            {
                Dock         = DockStyle.Fill,
                Padding      = new Padding(8),
                WrapContents = false
            };
            flow.Controls.AddRange(new Control[] { btnIn, btnMove, btnOut, btnRefresh });
            botPanel.Controls.Add(flow);
            Controls.Add(botPanel);
        }

        private static Button MakeButton(string text, Color color) => new()
        {
            Text      = text,
            Width     = 120,
            Height    = 38,
            Margin    = new Padding(5, 3, 5, 3),
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        // ─────────────────────────────────────────
        // 초기화
        // ─────────────────────────────────────────
        private async Task InitAsync()
        {
            await LoadDataAsync();
            await ConnectSocketAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var list = await _api.GetAllAsync();
                SafeInvoke(() =>
                {
                    _binding.DataSource = list;
                    grid.DataSource     = _binding;

                    var headers = new Dictionary<string, string>
                    {
                        ["ContainerId"] = "컨테이너 ID",
                        ["ItemName"]    = "물건",
                        ["Weight"]      = "무게(kg)",
                        ["ArrivalDate"] = "입고일",
                        ["Shelf"]       = "선반",
                        ["Floor"]       = "층",
                        ["Slot"]        = "슬롯",
                        ["Width"]       = "가로",
                        ["Depth"]       = "세로",
                        ["Height"]      = "높이",
                        ["Location"]    = "위치"
                    };
                    foreach (DataGridViewColumn col in grid.Columns)
                        if (headers.TryGetValue(col.Name, out var h)) col.HeaderText = h;

                    btnMove.Enabled = false;
                    btnOut.Enabled  = false;
                });
            }
            catch (Exception ex)
            {
                SafeInvoke(() => MessageBox.Show($"데이터 로드 실패: {ex.Message}", "오류"));
            }
        }

        // ─────────────────────────────────────────
        // Socket.IO 연결
        // ─────────────────────────────────────────
        private async Task ConnectSocketAsync()
        {
            _socket = new SocketIOClient.SocketIO(new Uri("http://localhost:3000"));

            _socket.OnConnected += (s, e) =>
                SafeInvoke(() => { lblStatus.Text = "● 서버 연결됨"; lblStatus.ForeColor = Color.LimeGreen; });

            _socket.OnDisconnected += (s, e) =>
                SafeInvoke(() => { lblStatus.Text = "● 서버 연결 끊김"; lblStatus.ForeColor = Color.Red; });

            _socket.On("containerAdded",   ctx => { SafeInvoke(() => { var t = LoadDataAsync(); }); return Task.CompletedTask; });
            _socket.On("containerMoved",   ctx => { SafeInvoke(() => { var t = LoadDataAsync(); }); return Task.CompletedTask; });
            _socket.On("containerRemoved", ctx => { SafeInvoke(() => { var t = LoadDataAsync(); }); return Task.CompletedTask; });

            try { await _socket.ConnectAsync(); }
            catch
            {
                SafeInvoke(() => { lblStatus.Text = "● 서버 없음 (오프라인)"; lblStatus.ForeColor = Color.OrangeRed; });
            }
        }

        // ─────────────────────────────────────────
        // 버튼 이벤트
        // ─────────────────────────────────────────
        private async void BtnIn_Click(object? sender, EventArgs e)
        {
            using var dlg = new IncomingForm();
            if (dlg.ShowDialog(this) != DialogResult.OK || dlg.Result == null) return;

            var ok = await _api.IncomingAsync(dlg.Result);
            if (!ok) MessageBox.Show("입고 실패 (중복 ID 또는 서버 오류)", "오류");
        }

        private async void BtnMove_Click(object? sender, EventArgs e)
        {
            var container = GetSelectedContainer();
            if (container == null) return;

            using var dlg = new MoveForm(container.Location);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            var ok = await _api.MoveAsync(container.ContainerId, dlg.TargetShelf, dlg.TargetFloor, dlg.TargetSlot);
            if (!ok) MessageBox.Show("이동 실패", "오류");
        }

        private async void BtnOut_Click(object? sender, EventArgs e)
        {
            var container = GetSelectedContainer();
            if (container == null) return;

            var confirm = MessageBox.Show(
                $"[{container.ContainerId}] {container.ItemName} 을(를) 출하하시겠습니까?",
                "출하 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            var ok = await _api.OutgoingAsync(container.ContainerId);
            if (!ok) MessageBox.Show("출하 실패", "오류");
        }

        // ─────────────────────────────────────────
        // 헬퍼
        // ─────────────────────────────────────────
        private void Grid_SelectionChanged(object? sender, EventArgs e)
        {
            var hasRow      = grid.SelectedRows.Count > 0;
            btnMove.Enabled = hasRow;
            btnOut.Enabled  = hasRow;
        }

        private ContainerModel? GetSelectedContainer()
        {
            if (grid.SelectedRows.Count == 0) return null;
            return grid.SelectedRows[0].DataBoundItem as ContainerModel;
        }

        private void SafeInvoke(Action action)
        {
            if (InvokeRequired) Invoke(action);
            else action();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _socket?.DisconnectAsync();
            base.OnFormClosed(e);
        }
    }
}
