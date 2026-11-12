
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using src.BUS;
using src.DTO;
using src.Helper;
using src.GUI.Components;

namespace src.GUI.DanhMuc
{
    public partial class QuanLyTaiKhoanForm : Form
    {
        private TaiKhoanBUS taiKhoanBUS;
        private NhanVienBUS nhanVienBUS;
        private NhomQuyenBUS nhomQuyenBUS;

        private bool isEditing = false;
        private int currentMaTK = -1;
        private int currentMaNV = -1;

        // Đang chọn tài khoản khách hàng hay không
        private bool isKhachHang = false;

        public QuanLyTaiKhoanForm()
        {
            try
            {
                InitializeComponent();

                taiKhoanBUS = new TaiKhoanBUS();
                nhanVienBUS = new NhanVienBUS();
                nhomQuyenBUS = new NhomQuyenBUS();

                InitializeDataGridView();

                LoadNhomQuyen();
                LoadNhanVien();
                LoadData();

                SetButtonStates(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khởi tạo form: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ============================================
        // 1. KHỞI TẠO BẢNG
        // ============================================

        private void InitializeDataGridView()
        {
            dgvTaiKhoan.Columns.Clear();
            dgvTaiKhoan.AutoGenerateColumns = false;

            dgvTaiKhoan.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "MaTK",
                    DataPropertyName = "MaTK",
                    HeaderText = "Mã TK",
                    Width = 75,
                    ReadOnly = true
                });

            dgvTaiKhoan.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "LoaiTaiKhoan",
                    DataPropertyName = "LoaiTaiKhoan",
                    HeaderText = "Loại tài khoản",
                    Width = 130,
                    ReadOnly = true
                });

            dgvTaiKhoan.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "MNV",
                    DataPropertyName = "MNV",
                    HeaderText = "Mã NV",
                    Width = 80,
                    ReadOnly = true
                });

            dgvTaiKhoan.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "MKH",
                    DataPropertyName = "MKH",
                    HeaderText = "Mã KH",
                    Width = 80,
                    ReadOnly = true
                });

            dgvTaiKhoan.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "TDN",
                    DataPropertyName = "TDN",
                    HeaderText = "Tên đăng nhập",
                    Width = 190,
                    ReadOnly = true
                });

            dgvTaiKhoan.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "MNQ",
                    DataPropertyName = "MNQ",
                    HeaderText = "Mã nhóm quyền",
                    Width = 130,
                    ReadOnly = true
                });

            dgvTaiKhoan.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    Name = "TT",
                    DataPropertyName = "TT",
                    HeaderText = "Trạng thái",
                    Width = 90,
                    ReadOnly = true
                });

            // Không đưa mật khẩu/hash vào DataGridView
            // để tránh vô tình xuất hash qua Excel.
        }

        // ============================================
        // 2. LOAD DỮ LIỆU
        // ============================================

        private void LoadData()
        {
            try
            {
                var danhSach =
                    taiKhoanBUS.GetTaiKhoanAll()
                    ?? new List<TaiKhoanDTO>();

                dgvTaiKhoan.DataSource = null;

                dgvTaiKhoan.DataSource =
                    new BindingList<TaiKhoanDTO>(
                        danhSach);

                if (dgvTaiKhoan.Rows.Count > 0)
                {
                    dgvTaiKhoan.ClearSelection();

                    dgvTaiKhoan.CurrentCell =
                        dgvTaiKhoan.Rows[0].Cells["MaTK"];

                    DisplayTaiKhoanInfo();
                }
                else
                {
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi tải dữ liệu: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadNhomQuyen()
        {
            try
            {
                var danhSach =
                    nhomQuyenBUS.GetAll();

                cboNhomQuyen.Items.Clear();

                foreach (var nq in danhSach)
                {
                    cboNhomQuyen.Items.Add(
                        new ComboBoxItem
                        {
                            Text = nq.Tennhomquyen,
                            Value = nq.Manhomquyen
                        });
                }

                if (cboNhomQuyen.Items.Count > 0)
                    cboNhomQuyen.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi load nhóm quyền: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadNhanVien()
        {
            try
            {
                cboMaNV.Items.Clear();

                var allNV = nhanVienBUS.GetAll();
                var allTK = taiKhoanBUS.GetTaiKhoanAll();

                // Cho phép một nhân viên có nhiều tài khoản
                // vì admin và QL1 đang cùng MNV = 1.
                foreach (var nv in allNV.Where(
                    nv => nv.TT == 1))
                {
                    cboMaNV.Items.Add(
                        new ComboBoxItem
                        {
                            Text = $"{nv.MNV} - {nv.HOTEN}",
                            Value = nv.MNV
                        });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi load nhân viên: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ============================================
        // 3. HIỂN THỊ THÔNG TIN TÀI KHOẢN
        // ============================================

        private TaiKhoanDTO? GetSelectedTaiKhoan()
        {
            return dgvTaiKhoan.CurrentRow?.DataBoundItem
                as TaiKhoanDTO;
        }

        private void DgvTaiKhoan_SelectionChanged(
            object sender,
            EventArgs e)
        {
            if (!isEditing)
            {
                DisplayTaiKhoanInfo();
            }
        }

        private void DisplayTaiKhoanInfo()
        {
            try
            {
                var tk = GetSelectedTaiKhoan();

                if (tk == null)
                    return;

                currentMaTK = tk.MaTK;
                currentMaNV = tk.MNV ?? -1;
                isKhachHang = tk.MKH.HasValue;

                cboMaNV.Items.Clear();
                txtTenNV.Clear();

                if (tk.MNV.HasValue)
                {
                    var nv = nhanVienBUS.GetById(
                        tk.MNV.Value);

                    if (nv != null)
                    {
                        cboMaNV.Items.Add(
                            new ComboBoxItem
                            {
                                Text =
                                    $"{nv.MNV} - {nv.HOTEN}",
                                Value = nv.MNV
                            });

                        cboMaNV.SelectedIndex = 0;
                        txtTenNV.Text = nv.HOTEN;
                    }
                }
                else if (tk.MKH.HasValue)
                {
                    cboMaNV.Items.Add(
                        new ComboBoxItem
                        {
                            Text =
                                $"KH{tk.MKH.Value} - Khách hàng",
                            Value = tk.MKH.Value
                        });

                    cboMaNV.SelectedIndex = 0;

                    txtTenNV.Text =
                        $"Khách hàng (Mã KH: {tk.MKH.Value})";
                }

                txtTenDangNhap.Text = tk.TDN;

                for (int i = 0;
                     i < cboNhomQuyen.Items.Count;
                     i++)
                {
                    var item =
                        (ComboBoxItem)cboNhomQuyen.Items[i];

                    if (item.Value == tk.MNQ)
                    {
                        cboNhomQuyen.SelectedIndex = i;
                        break;
                    }
                }

                txtMatKhau.Clear();
                txtXacNhanMK.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi hiển thị tài khoản: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CboMaNV_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if (!isEditing ||
                isKhachHang ||
                cboMaNV.SelectedItem == null)
            {
                return;
            }

            var item =
                (ComboBoxItem)cboMaNV.SelectedItem;

            var nv = nhanVienBUS.GetById(item.Value);

            txtTenNV.Text = nv?.HOTEN ?? "";
        }

        // ============================================
        // 4. TÌM KIẾM VÀ LÀM MỚI
        // ============================================

        private void BtnTimKiem_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                string keyword = txtTimKiem.Text.Trim();

                string searchType =
                    cboTimKiem.SelectedItem?.ToString()
                    ?? "Tất cả";

                var result = taiKhoanBUS.Search(
                    keyword, searchType);

                dgvTaiKhoan.DataSource = null;

                dgvTaiKhoan.DataSource =
                    new BindingList<TaiKhoanDTO>(result);

                if (dgvTaiKhoan.Rows.Count > 0)
                    DisplayTaiKhoanInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi tìm kiếm: {ex.Message}");
            }
        }

        private void BtnRefresh_Click(
            object sender,
            EventArgs e)
        {
            txtTimKiem.Clear();

            if (cboTimKiem.Items.Count > 0)
                cboTimKiem.SelectedIndex = 0;

            LoadData();
        }

        // ============================================
        // 5. TRẠNG THÁI FORM
        // ============================================

        private void ClearForm()
        {
            currentMaTK = -1;
            currentMaNV = -1;
            isKhachHang = false;

            LoadNhanVien();

            txtTenNV.Clear();
            txtTenDangNhap.Clear();
            txtMatKhau.Clear();
            txtXacNhanMK.Clear();

            if (cboNhomQuyen.Items.Count > 0)
                cboNhomQuyen.SelectedIndex = 0;
        }

        private void SetButtonStates(bool editing)
        {
            isEditing = editing;

            btnThem.Enabled =
                !editing &&
                SessionManager.CanCreate("taikhoan");

            btnSua.Enabled =
                !editing &&
                SessionManager.CanUpdate("taikhoan");

            btnXoa.Enabled =
                !editing &&
                SessionManager.CanDelete("taikhoan");

            btnResetMK.Enabled =
                !editing &&
                SessionManager.CanUpdate("taikhoan");

            btnLuu.Visible = editing;
            btnHuy.Visible = editing;

            dgvTaiKhoan.Enabled = !editing;

            // Chỉ được chọn nhân viên khi thêm mới.
            cboMaNV.Enabled =
                editing && currentMaTK == -1;

            txtTenDangNhap.ReadOnly = !editing;
            txtMatKhau.ReadOnly = !editing;
            txtXacNhanMK.ReadOnly = !editing;

            // Không cho sửa nhóm quyền của khách hàng.
            cboNhomQuyen.Enabled =
                editing && !isKhachHang;
        }

        // ============================================
        // 6. THÊM TÀI KHOẢN
        // ============================================

        private void BtnThem_Click(
            object sender,
            EventArgs e)
        {
            ClearForm();

            SetButtonStates(true);

            cboMaNV.Focus();
        }

        // ============================================
        // 7. SỬA TÀI KHOẢN
        // ============================================

        private void BtnSua_Click(
            object sender,
            EventArgs e)
        {
            var tk = GetSelectedTaiKhoan();

            if (tk == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn tài khoản!");
                return;
            }

            currentMaTK = tk.MaTK;
            currentMaNV = tk.MNV ?? -1;
            isKhachHang = tk.MKH.HasValue;

            SetButtonStates(true);

            txtTenDangNhap.Focus();
        }

        // ============================================
        // 8. XÓA MỀM TÀI KHOẢN
        // ============================================

        private void BtnXoa_Click(
            object sender,
            EventArgs e)
        {
            var tk = GetSelectedTaiKhoan();

            if (tk == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn tài khoản cần xóa!");
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Bạn có chắc chắn muốn khóa tài khoản " +
                $"'{tk.TDN}' (MaTK: {tk.MaTK})?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                // Xóa theo MaTK, không dùng MNV.
                taiKhoanBUS.DeleteAccByMaTK(tk.MaTK);

                MessageBox.Show(
                    "Khóa tài khoản thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadData();
                LoadNhanVien();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ============================================
        // 9. LƯU TÀI KHOẢN
        // ============================================

        private void BtnLuu_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                bool themMoi = currentMaTK == -1;

                string tenDangNhap =
                    txtTenDangNhap.Text.Trim();

                if (string.IsNullOrWhiteSpace(
                    tenDangNhap))
                {
                    MessageBox.Show(
                        "Vui lòng nhập tên đăng nhập!");
                    txtTenDangNhap.Focus();
                    return;
                }

                TaiKhoanDTO tk;

                if (themMoi)
                {
                    // Desktop chỉ thêm tài khoản nhân viên.
                    if (cboMaNV.SelectedItem == null)
                    {
                        MessageBox.Show(
                            "Vui lòng chọn nhân viên!");
                        return;
                    }

                    var selectedNV =
                        (ComboBoxItem)cboMaNV.SelectedItem;

                    if (cboNhomQuyen.SelectedItem
                        is not ComboBoxItem selectedQuyen)
                    {
                        MessageBox.Show(
                            "Vui lòng chọn nhóm quyền!");
                        return;
                    }

                    // Không tạo tài khoản khách hàng
                    // bằng form thêm nhân viên.
                    var quyenKhachHang =
                        taiKhoanBUS.GetNhomQuyenDTO(
                            selectedQuyen.Value);

                    if (quyenKhachHang != null &&
                        string.Equals(
                            quyenKhachHang.Tennhomquyen,
                            "Khách hàng",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show(
                            "Tài khoản khách hàng phải " +
                            "được tạo qua chức năng Web.");
                        return;
                    }

                    if (txtMatKhau.Text.Length < 6)
                    {
                        MessageBox.Show(
                            "Mật khẩu phải có ít nhất 6 ký tự!");
                        return;
                    }

                    if (txtMatKhau.Text !=
                        txtXacNhanMK.Text)
                    {
                        MessageBox.Show(
                            "Mật khẩu xác nhận không khớp!");
                        return;
                    }

                    tk = new TaiKhoanDTO
                    {
                        MNV = selectedNV.Value,
                        MKH = null,
                        TDN = tenDangNhap,
                        MK = txtMatKhau.Text,
                        MNQ = selectedQuyen.Value,
                        TT = 1
                    };

                    taiKhoanBUS.AddAcc(tk);

                    MessageBox.Show(
                        "Thêm tài khoản thành công!");
                }
                else
                {
                    // Lấy bản gốc theo MaTK.
                    var oldTk =
                        taiKhoanBUS.GetTaiKhoanTheoMaTK(
                            currentMaTK);

                    if (oldTk == null)
                    {
                        MessageBox.Show(
                            "Không tìm thấy tài khoản!");
                        return;
                    }

                    // Giữ nguyên MNV, MKH và nhóm quyền
                    // của khách hàng khi chỉnh sửa.
                    tk = new TaiKhoanDTO
                    {
                        MaTK = oldTk.MaTK,
                        MNV = oldTk.MNV,
                        MKH = oldTk.MKH,
                        TDN = tenDangNhap,
                        TT = oldTk.TT,
                        MNQ = oldTk.MNQ,
                        MK = oldTk.MK
                    };

                    if (!oldTk.MKH.HasValue)
                    {
                        if (cboNhomQuyen.SelectedItem
                            is ComboBoxItem selectedQuyen)
                        {
                            tk.MNQ = selectedQuyen.Value;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(
                        txtMatKhau.Text))
                    {
                        if (txtMatKhau.Text.Length < 6)
                        {
                            MessageBox.Show(
                                "Mật khẩu phải có ít nhất " +
                                "6 ký tự!");
                            return;
                        }

                        if (txtMatKhau.Text !=
                            txtXacNhanMK.Text)
                        {
                            MessageBox.Show(
                                "Mật khẩu xác nhận không khớp!");
                            return;
                        }

                        // BUS sẽ băm mật khẩu mới.
                        tk.MK = txtMatKhau.Text;
                    }

                    taiKhoanBUS.UpdateAcc(tk);

                    MessageBox.Show(
                        "Cập nhật tài khoản thành công!");
                }

                SetButtonStates(false);
                LoadData();
                LoadNhanVien();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi lưu tài khoản: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ============================================
        // 10. HỦY THAO TÁC
        // ============================================

        private void BtnHuy_Click(
            object sender,
            EventArgs e)
        {
            SetButtonStates(false);

            LoadNhanVien();
            LoadData();
        }

        // ============================================
        // 11. RESET MẬT KHẨU
        // ============================================

        private void BtnResetMK_Click(
            object sender,
            EventArgs e)
        {
            var tk = GetSelectedTaiKhoan();

            if (tk == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn tài khoản cần reset!");
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Reset mật khẩu cho tài khoản " +
                $"'{tk.TDN}' (MaTK: {tk.MaTK})?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                // Mật khẩu tạm ngẫu nhiên.
                string matKhauMoi =
                    Convert.ToBase64String(
                        RandomNumberGenerator.GetBytes(18))
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');

                var account =
                    taiKhoanBUS.GetTaiKhoanTheoMaTK(
                        tk.MaTK);

                if (account == null)
                {
                    MessageBox.Show(
                        "Không tìm thấy tài khoản!");
                    return;
                }

                // BUS sẽ tự băm BCrypt.
                account.MK = matKhauMoi;

                taiKhoanBUS.UpdateAcc(account);

                MessageBox.Show(
                    "Reset mật khẩu thành công!\n\n" +
                    $"Mật khẩu tạm: {matKhauMoi}\n\n" +
                    "Hãy cung cấp mật khẩu tạm cho " +
                    "đúng chủ tài khoản qua kênh an toàn.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi reset mật khẩu: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ============================================
        // 12. XUẤT EXCEL
        // ============================================

        private void BtnExport_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (dgvTaiKhoan.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "Không có dữ liệu để xuất!");
                    return;
                }

                TableExporter.ExportTableToExcel(
                    dgvTaiKhoan, "TK");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi xuất Excel: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ============================================
        // 13. COMBOBOX ITEM
        // ============================================

        private class ComboBoxItem
        {
            public string Text { get; set; } = "";
            public int Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}
