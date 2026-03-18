namespace WarehouseWinForms.Forms
{
    partial class IncomingForm
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
            lblId = new Label();
            txtId = new TextBox();
            lblItemName = new Label();
            txtItemName = new TextBox();
            lblWeight = new Label();
            txtWeight = new TextBox();
            lblArrival = new Label();
            dtpArrival = new DateTimePicker();
            lblShelf = new Label();
            cmbShelf = new ComboBox();
            lblFloor = new Label();
            cmbFloor = new ComboBox();
            lblSlot = new Label();
            cmbSlot = new ComboBox();
            lblWidth = new Label();
            txtWidth = new TextBox();
            lblDepth = new Label();
            txtDepth = new TextBox();
            lblHeight = new Label();
            txtHeight = new TextBox();
            btnOk = new Button();
            tableLayout.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayout
            // 
            tableLayout.ColumnCount = 2;
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            tableLayout.Controls.Add(lblId, 0, 0);
            tableLayout.Controls.Add(txtId, 1, 0);
            tableLayout.Controls.Add(lblItemName, 0, 1);
            tableLayout.Controls.Add(txtItemName, 1, 1);
            tableLayout.Controls.Add(lblWeight, 0, 2);
            tableLayout.Controls.Add(txtWeight, 1, 2);
            tableLayout.Controls.Add(lblArrival, 0, 3);
            tableLayout.Controls.Add(dtpArrival, 1, 3);
            tableLayout.Controls.Add(lblShelf, 0, 4);
            tableLayout.Controls.Add(cmbShelf, 1, 4);
            tableLayout.Controls.Add(lblFloor, 0, 5);
            tableLayout.Controls.Add(cmbFloor, 1, 5);
            tableLayout.Controls.Add(lblSlot, 0, 6);
            tableLayout.Controls.Add(cmbSlot, 1, 6);
            tableLayout.Controls.Add(lblWidth, 0, 7);
            tableLayout.Controls.Add(txtWidth, 1, 7);
            tableLayout.Controls.Add(lblDepth, 0, 8);
            tableLayout.Controls.Add(txtDepth, 1, 8);
            tableLayout.Controls.Add(lblHeight, 0, 9);
            tableLayout.Controls.Add(txtHeight, 1, 9);
            tableLayout.Controls.Add(btnOk, 0, 10);
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.Location = new Point(0, 0);
            tableLayout.Name = "tableLayout";
            tableLayout.Padding = new Padding(8);
            tableLayout.RowCount = 11;
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayout.Size = new Size(400, 460);
            tableLayout.TabIndex = 0;
            // 
            // lblId
            // 
            lblId.Dock = DockStyle.Fill;
            lblId.Font = new Font("맑은 고딕", 10F);
            lblId.Location = new Point(11, 8);
            lblId.Name = "lblId";
            lblId.Size = new Size(147, 38);
            lblId.TabIndex = 0;
            lblId.Text = "ID (CNT-000)";
            lblId.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtId
            // 
            txtId.Dock = DockStyle.Fill;
            txtId.Font = new Font("맑은 고딕", 10F);
            txtId.Location = new Point(164, 11);
            txtId.Name = "txtId";
            txtId.Size = new Size(225, 25);
            txtId.TabIndex = 1;
            // 
            // lblItemName
            // 
            lblItemName.Dock = DockStyle.Fill;
            lblItemName.Font = new Font("맑은 고딕", 10F);
            lblItemName.Location = new Point(11, 46);
            lblItemName.Name = "lblItemName";
            lblItemName.Size = new Size(147, 38);
            lblItemName.TabIndex = 2;
            lblItemName.Text = "품목명";
            lblItemName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtItemName
            // 
            txtItemName.Dock = DockStyle.Fill;
            txtItemName.Font = new Font("맑은 고딕", 10F);
            txtItemName.Location = new Point(164, 49);
            txtItemName.Name = "txtItemName";
            txtItemName.Size = new Size(225, 25);
            txtItemName.TabIndex = 3;
            // 
            // lblWeight
            // 
            lblWeight.Dock = DockStyle.Fill;
            lblWeight.Font = new Font("맑은 고딕", 10F);
            lblWeight.Location = new Point(11, 84);
            lblWeight.Name = "lblWeight";
            lblWeight.Size = new Size(147, 38);
            lblWeight.TabIndex = 4;
            lblWeight.Text = "중량(kg)";
            lblWeight.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtWeight
            // 
            txtWeight.Dock = DockStyle.Fill;
            txtWeight.Font = new Font("맑은 고딕", 10F);
            txtWeight.Location = new Point(164, 87);
            txtWeight.Name = "txtWeight";
            txtWeight.Size = new Size(225, 25);
            txtWeight.TabIndex = 5;
            // 
            // lblArrival
            // 
            lblArrival.Dock = DockStyle.Fill;
            lblArrival.Font = new Font("맑은 고딕", 10F);
            lblArrival.Location = new Point(11, 122);
            lblArrival.Name = "lblArrival";
            lblArrival.Size = new Size(147, 38);
            lblArrival.TabIndex = 6;
            lblArrival.Text = "입고일";
            lblArrival.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // dtpArrival
            // 
            dtpArrival.Dock = DockStyle.Fill;
            dtpArrival.Format = DateTimePickerFormat.Short;
            dtpArrival.Location = new Point(164, 125);
            dtpArrival.Name = "dtpArrival";
            dtpArrival.Size = new Size(225, 23);
            dtpArrival.TabIndex = 7;
            // 
            // lblShelf
            // 
            lblShelf.Dock = DockStyle.Fill;
            lblShelf.Font = new Font("맑은 고딕", 10F);
            lblShelf.Location = new Point(11, 160);
            lblShelf.Name = "lblShelf";
            lblShelf.Size = new Size(147, 38);
            lblShelf.TabIndex = 8;
            lblShelf.Text = "선반";
            lblShelf.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cmbShelf
            // 
            cmbShelf.Dock = DockStyle.Fill;
            cmbShelf.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbShelf.Font = new Font("맑은 고딕", 10F);
            cmbShelf.Items.AddRange(new object[] { "A", "B", "C", "D" });
            cmbShelf.Location = new Point(164, 163);
            cmbShelf.Name = "cmbShelf";
            cmbShelf.Size = new Size(225, 25);
            cmbShelf.TabIndex = 9;
            // 
            // lblFloor
            // 
            lblFloor.Dock = DockStyle.Fill;
            lblFloor.Font = new Font("맑은 고딕", 10F);
            lblFloor.Location = new Point(11, 198);
            lblFloor.Name = "lblFloor";
            lblFloor.Size = new Size(147, 38);
            lblFloor.TabIndex = 10;
            lblFloor.Text = "층";
            lblFloor.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cmbFloor
            // 
            cmbFloor.Dock = DockStyle.Fill;
            cmbFloor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFloor.Font = new Font("맑은 고딕", 10F);
            cmbFloor.Items.AddRange(new object[] { 0, 1, 2 });
            cmbFloor.Location = new Point(164, 201);
            cmbFloor.Name = "cmbFloor";
            cmbFloor.Size = new Size(225, 25);
            cmbFloor.TabIndex = 11;
            // 
            // lblSlot
            // 
            lblSlot.Dock = DockStyle.Fill;
            lblSlot.Font = new Font("맑은 고딕", 10F);
            lblSlot.Location = new Point(11, 236);
            lblSlot.Name = "lblSlot";
            lblSlot.Size = new Size(147, 38);
            lblSlot.TabIndex = 12;
            lblSlot.Text = "슬롯";
            lblSlot.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cmbSlot
            // 
            cmbSlot.Dock = DockStyle.Fill;
            cmbSlot.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSlot.Font = new Font("맑은 고딕", 10F);
            cmbSlot.Items.AddRange(new object[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            cmbSlot.Location = new Point(164, 239);
            cmbSlot.Name = "cmbSlot";
            cmbSlot.Size = new Size(225, 25);
            cmbSlot.TabIndex = 13;
            // 
            // lblWidth
            // 
            lblWidth.Dock = DockStyle.Fill;
            lblWidth.Font = new Font("맑은 고딕", 10F);
            lblWidth.Location = new Point(11, 274);
            lblWidth.Name = "lblWidth";
            lblWidth.Size = new Size(147, 38);
            lblWidth.TabIndex = 14;
            lblWidth.Text = "가로(m)";
            lblWidth.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtWidth
            // 
            txtWidth.Dock = DockStyle.Fill;
            txtWidth.Font = new Font("맑은 고딕", 10F);
            txtWidth.Location = new Point(164, 277);
            txtWidth.Name = "txtWidth";
            txtWidth.Size = new Size(225, 25);
            txtWidth.TabIndex = 15;
            // 
            // lblDepth
            // 
            lblDepth.Dock = DockStyle.Fill;
            lblDepth.Font = new Font("맑은 고딕", 10F);
            lblDepth.Location = new Point(11, 312);
            lblDepth.Name = "lblDepth";
            lblDepth.Size = new Size(147, 38);
            lblDepth.TabIndex = 16;
            lblDepth.Text = "세로(m)";
            lblDepth.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDepth
            // 
            txtDepth.Dock = DockStyle.Fill;
            txtDepth.Font = new Font("맑은 고딕", 10F);
            txtDepth.Location = new Point(164, 315);
            txtDepth.Name = "txtDepth";
            txtDepth.Size = new Size(225, 25);
            txtDepth.TabIndex = 17;
            // 
            // lblHeight
            // 
            lblHeight.Dock = DockStyle.Fill;
            lblHeight.Font = new Font("맑은 고딕", 10F);
            lblHeight.Location = new Point(11, 350);
            lblHeight.Name = "lblHeight";
            lblHeight.Size = new Size(147, 38);
            lblHeight.TabIndex = 18;
            lblHeight.Text = "높이(m)";
            lblHeight.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtHeight
            // 
            txtHeight.Dock = DockStyle.Fill;
            txtHeight.Font = new Font("맑은 고딕", 10F);
            txtHeight.Location = new Point(164, 353);
            txtHeight.Name = "txtHeight";
            txtHeight.Size = new Size(225, 25);
            txtHeight.TabIndex = 19;
            // 
            // btnOk
            // 
            btnOk.BackColor = Color.FromArgb(128, 255, 128);
            tableLayout.SetColumnSpan(btnOk, 2);
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Dock = DockStyle.Fill;
            btnOk.Font = new Font("맑은 고딕", 11F);
            btnOk.Location = new Point(11, 391);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(378, 58);
            btnOk.TabIndex = 20;
            btnOk.Text = "확인";
            btnOk.UseVisualStyleBackColor = false;
            // 
            // IncomingForm
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 460);
            Controls.Add(tableLayout);
            Name = "IncomingForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "입고";
            tableLayout.ResumeLayout(false);
            tableLayout.PerformLayout();
            ResumeLayout(false);
        }

        private TableLayoutPanel tableLayout;
        private Label   lblId, lblItemName, lblWeight, lblArrival;
        private Label   lblShelf, lblFloor, lblSlot;
        private Label   lblWidth, lblDepth, lblHeight;
        private TextBox txtId, txtItemName, txtWeight;
        private TextBox txtWidth, txtDepth, txtHeight;
        private DateTimePicker dtpArrival;
        private ComboBox cmbShelf, cmbFloor, cmbSlot;
        private Button btnOk;
    }
}
