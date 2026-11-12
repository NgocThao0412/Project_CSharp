
using System;
using System.Drawing;
using System.Windows.Forms;
using src.BUS;
using src.DTO;
using src.Helper;
using src.GUI.Components;

namespace src.GUI.Auth
{
    public partial class LoginPage : Form
    {
        private TaiKhoanBUS taiKhoanBUS;
        private NhanVienBUS nhanVienBUS;
        private NhomQuyenBUS nhomQuyenBUS;

        public LoginPage()
        {
            try
            {
                string iconPath = "icon/app.ico";
                if (System.IO.File.Exists(iconPath))
                {
                    this.Icon = new System.Drawing.Icon(iconPath);
                }
            }
            catch { }

            InitializeComponent();

            taiKhoanBUS = new TaiKhoanBUS();
            nhanVienBUS = new NhanVienBUS();
            nhomQuyenBUS = new NhomQuyenBUS();

            // Giá trị mặc định
            txtUsername.Text = "QL1";
            txtPassword.Text = "";

            // Đăng ký sự kiện
            AddEvents();
        }

        private void AddEvents()
        {
            // Sự kiện đăng nhập
            btnLogin.Click += BtnLogin_Click;

            btnLogin.MouseEnter += (s, e) =>
                btnLogin.BackColor = Color.FromArgb(0, 202, 232);

            btnLogin.MouseLeave += (s, e) =>
                btnLogin.BackColor = Color.Black;

            // Sự kiện quên mật khẩu
            lblForgotPass.Click += (s, e) =>
            {
                new QuenMatKhau().ShowDialog();
            };

            lblForgotPass.MouseEnter += (s, e) =>
                lblForgotPass.ForeColor = Color.FromArgb(0, 202, 232);

            lblForgotPass.MouseLeave += (s, e) =>
                lblForgotPass.ForeColor = Color.Black;

            // Sự kiện Enter
            txtUsername.KeyDown += Enter_KeyDown;
            txtPassword.KeyDown += Enter_KeyDown;
        }

        private void Enter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CheckLogin();
                e.SuppressKeyPress = true;
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            CheckLogin();
        }

        private void CheckLogin()
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;

                // 1. Kiểm tra tên đăng nhập
                if (string.IsNullOrWhiteSpace(username))
                {
                    MessageBox.Show(
                        "Vui lòng nhập tên đăng nhập!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtUsername.Focus();
                    return;
                }

                // 2. Kiểm tra mật khẩu
                if (string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show(
                        "Vui lòng nhập mật khẩu!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtPassword.Focus();
                    return;
                }

                // 3. Xác thực tài khoản
                TaiKhoanDTO? account =
                    taiKhoanBUS.DangNhap(username, password);

                if (account == null)
                {
                    MessageBox.Show(
                        "Tên đăng nhập hoặc mật khẩu không đúng!",
                        "Lỗi đăng nhập",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    txtPassword.Clear();
                    txtPassword.Focus();
                    return;
                }

                // 4. Kiểm tra trạng thái tài khoản
                if (account.TT != 1)
                {
                    MessageBox.Show(
                        "Tài khoản đã bị khóa hoặc không hoạt động!",
                        "Lỗi đăng nhập",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // 5. Kiểm tra tài khoản có thuộc nhân viên không
                // MNV là int? nên phải kiểm tra trước khi dùng
                if (!account.MNV.HasValue)
                {
                    MessageBox.Show(
                        "Tài khoản khách hàng không có quyền đăng nhập Desktop!",
                        "Lỗi đăng nhập",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // 6. Lấy thông tin nhân viên
                // .Value chuyển int? thành int sau khi đã kiểm tra
                NhanVienDTO? employee =
                    nhanVienBUS.GetById(account.MNV.Value);

                if (employee == null)
                {
                    MessageBox.Show(
                        "Không tìm thấy thông tin nhân viên!",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // 7. Lấy nhóm quyền và quyền truy cập
                NhomQuyenDTO? role =
                    nhomQuyenBUS.GetById(account.MNQ);

                var permissions =
                    nhomQuyenBUS.GetChiTietQuyen(account.MNQ);

                // 8. Lưu phiên đăng nhập
                SessionManager.Login(
                    account,
                    employee,
                    role ?? new NhomQuyenDTO(),
                    permissions);

                MessageBox.Show(
                    $"Đăng nhập thành công!\nXin chào, {employee.HOTEN}",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // 9. Mở màn hình chính
                this.Hide();

                MainForm mainForm = new MainForm();

                mainForm.FormClosed += (s, args) =>
                    this.Close();

                mainForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi đăng nhập: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
