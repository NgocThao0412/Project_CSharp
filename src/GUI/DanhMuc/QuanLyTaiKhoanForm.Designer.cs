using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace src.GUI.DanhMuc
{
    partial class QuanLyTaiKhoanForm
    {
        private IContainer components = null;

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
            this.Text = "Quản lý Tài khoản";
            this.Size = new Size(1300, 750);
            this.MinimumSize = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(236, 240, 241);

            // --- 1. SETUP LAYOUT CHÍNH ---

            // Panel Header (Tiêu đề + Search)
            Panel pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 110;
            pnlHeader.BackColor = Color.WhiteSmoke;
            pnlHeader.Padding = new Padding(10);

            // Panel Form nhập liệu (Bên phải)
            Panel pnlForm = CreateFormPanel();
            pnlForm.Dock = DockStyle.Right;
            pnlForm.Width = 360;

            // Panel Nút chức năng (Dưới cùng)
            Panel pnlButtons = new Panel();
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.Height = 70;
            pnlButtons.BackColor = Color.White;

            // DataGridView (Ở giữa - Fill)
            dgvTaiKhoan = new DataGridView();
            dgvTaiKhoan.Dock = DockStyle.Fill;
            dgvTaiKhoan.BackgroundColor = Color.White;
            dgvTaiKhoan.AllowUserToAddRows = false;
            dgvTaiKhoan.ReadOnly = true;
            dgvTaiKhoan.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTaiKhoan.SelectionChanged += DgvTaiKhoan_SelectionChanged;

            // --- 2. XỬ LÝ HEADER (TITLE + SEARCH CENTER) ---

            Label lblTitle = new Label();
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(41, 128, 185);
            lblTitle.Location = new Point(20, 10);
            lblTitle.Text = "QUẢN LÝ TÀI KHOẢN";
            pnlHeader.Controls.Add(lblTitle);

            // TẠO CONTAINER RIÊNG CHO CỤM TÌM KIẾM
            Panel pnlSearchBox = new Panel();
            pnlSearchBox.Size = new Size(800, 40);
            pnlSearchBox.BackColor = Color.Transparent;

            // Các control tìm kiếm
            cboTimKiem = new ComboBox();
            cboTimKiem.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTimKiem.Location = new Point(0, 8);
            cboTimKiem.Size = new Size(150, 25);
            cboTimKiem.Items.AddRange(new string[] { "Tất cả", "Mã nhân viên", "Username" });
            cboTimKiem.SelectedIndex = 0;

            txtTimKiem = new TextBox();
            txtTimKiem.Location = new Point(160, 8);
            txtTimKiem.Size = new Size(250, 25);
            txtTimKiem.PlaceholderText = "Nhập từ khóa...";

            btnTimKiem = CreateButtonSmall("🔍 Tìm", 420, Color.FromArgb(41, 128, 185), BtnTimKiem_Click);
            btnRefresh = CreateButtonSmall("⟳ Load", 520, Color.FromArgb(52, 152, 219), BtnRefresh_Click);
            btnExport = CreateButtonSmall("📤 Export", 620, Color.FromArgb(39, 174, 96), BtnExport_Click);

            pnlSearchBox.Controls.AddRange(new Control[] {
                cboTimKiem, txtTimKiem, btnTimKiem, btnRefresh, btnExport
            });

            pnlHeader.Controls.Add(pnlSearchBox);

            // --- 3. XỬ LÝ BUTTONS DƯỚI (CANH GIỮA) ---

            Panel pnlActionBox = new Panel();
            pnlActionBox.Size = new Size(480, 50); // Rộng hơn để chứa nút Reset MK
            pnlActionBox.BackColor = Color.Transparent;

            // Tạo các nút chức năng (Thêm nút Reset MK vào cuối)
            btnThem = CreateBtnAction("➕ Thêm", 0, Color.FromArgb(46, 204, 113), BtnThem_Click);
            btnSua = CreateBtnAction("✏️ Sửa", 1, Color.FromArgb(52, 152, 219), BtnSua_Click);
            btnXoa = CreateBtnAction("🗑️ Xóa", 2, Color.FromArgb(231, 76, 60), BtnXoa_Click);

            // Nút Reset MK
            btnResetMK = CreateBtnAction("🔑 Reset MK", 5, Color.FromArgb(230, 126, 34), BtnResetMK_Click);

            pnlActionBox.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa, btnResetMK });
            pnlButtons.Controls.Add(pnlActionBox);

            // --- 4. SỰ KIỆN RESIZE ---

            // Canh giữa thanh tìm kiếm
            pnlHeader.Resize += (s, e) => {
                pnlSearchBox.Location = new Point(
                    (pnlHeader.Width - pnlSearchBox.Width) / 2,
                    60
                );
            };

            // Canh giữa thanh nút bấm
            pnlButtons.Resize += (s, e) => {
                pnlActionBox.Location = new Point(
                    (pnlButtons.Width - pnlActionBox.Width) / 2,
                    10
                );
            };

            // --- 5. ADD CONTROLS ---
            Controls.Add(dgvTaiKhoan);
            Controls.Add(pnlForm);
            Controls.Add(pnlHeader);
            Controls.Add(pnlButtons);

            ((ISupportInitialize)(dgvTaiKhoan)).EndInit();
            this.ResumeLayout(false);
        }

        // --- HELPER METHODS ---

        private Button CreateButtonSmall(string text, int x, Color color, EventHandler click)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(x, 5);
            btn.Size = new Size(90, 30);
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += click;
            return btn;
        }

        private Button CreateBtnAction(string text, int index, Color color, EventHandler click)
        {
            int btnW = 100, gap = 15;
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point((btnW + gap) * index, 10);
            btn.Size = new Size(btnW, 35);
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += click;
            return btn;
        }

        private Panel CreateFormPanel()
        {
            Panel panel = new Panel
            {
                BackColor = Color.White,
                Padding = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };

            int y = 40;
            int labelW = 90;
            int inputW = 230;
            int startX = 15;

            // Tiêu đề nhỏ
            Label lblInfo = new Label {
                Text = "Thông tin Tài khoản",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                Location = new Point(startX, 10),
                AutoSize = true
            };
            panel.Controls.Add(lblInfo);

            void AddInput(string labelText, Control control)
            {
                Label lbl = new Label { Text = labelText, Location = new Point(startX, y + 3), Size = new Size(labelW, 25) };
                control.Location = new Point(startX + labelW, y);
                control.Size = new Size(inputW, 25);
                panel.Controls.Add(lbl);
                panel.Controls.Add(control);
                y += 40;
            }

            // 1. Chọn Nhân viên
            cboMaNV = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cboMaNV.SelectedIndexChanged += CboMaNV_SelectedIndexChanged;
            AddInput("Nhân viên: *", cboMaNV);

            // 2. Tên Nhân viên (Read Only)
            txtTenNV = new TextBox { ReadOnly = true, BackColor = SystemColors.Control };
            AddInput("Họ tên:", txtTenNV);

            // 3. Tên đăng nhập
            txtTenDangNhap = new TextBox();
            AddInput("Username: *", txtTenDangNhap);

            // 4. Mật khẩu
            txtMatKhau = new TextBox { PasswordChar = '●' };
            AddInput("Mật khẩu: *", txtMatKhau);

            // 5. Xác nhận MK
            txtXacNhanMK = new TextBox { PasswordChar = '●' };
            AddInput("Xác nhận: *", txtXacNhanMK);

            // 6. Nhóm quyền
            cboNhomQuyen = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            AddInput("Quyền: *", cboNhomQuyen);

            // --- THÊM NÚT LƯU & HỦY VÀO ĐÂY ---
            y += 10;

            btnLuu = new Button();
            btnLuu.Text = "💾 Lưu";
            btnLuu.Size = new Size(110, 35);
            btnLuu.Location = new Point(startX + labelW, y); // Căn thẳng hàng với input
            btnLuu.BackColor = Color.FromArgb(41, 128, 185);
            btnLuu.ForeColor = Color.White;
            btnLuu.FlatStyle = FlatStyle.Flat;
            btnLuu.Click += BtnLuu_Click;
            btnLuu.Visible = false; // Mặc định ẩn

            btnHuy = new Button();
            btnHuy.Text = "❌ Hủy";
            btnHuy.Size = new Size(110, 35);
            btnHuy.Location = new Point(startX + labelW + 120, y); // Nằm bên phải nút Lưu
            btnHuy.BackColor = Color.FromArgb(149, 165, 166);
            btnHuy.ForeColor = Color.White;
            btnHuy.FlatStyle = FlatStyle.Flat;
            btnHuy.Click += BtnHuy_Click;
            btnHuy.Visible = false; // Mặc định ẩn

            panel.Controls.Add(btnLuu);
            panel.Controls.Add(btnHuy);

            return panel;
        }

        #region Components
        private DataGridView dgvTaiKhoan;

        // Input Fields
        private ComboBox cboMaNV;
        private TextBox txtTenNV;
        private TextBox txtTenDangNhap;
        private TextBox txtMatKhau;
        private TextBox txtXacNhanMK;
        private ComboBox cboNhomQuyen;

        // Search Fields
        private TextBox txtTimKiem;
        private ComboBox cboTimKiem;

        // Buttons
        private Button btnExport;
        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;
        private Button btnLuu;
        private Button btnHuy;
        private Button btnResetMK;
        private Button btnTimKiem;
        private Button btnRefresh;
        #endregion
    }
}