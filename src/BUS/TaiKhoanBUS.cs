
using System;
using System.Collections.Generic;
using System.Linq;
using src.DAO;
using src.DTO;

namespace src.BUS
{
    public class TaiKhoanBUS
    {
        private List<TaiKhoanDTO> listTaiKhoan =
            new List<TaiKhoanDTO>();

        private readonly NhomQuyenDAO nhomQuyenDAO =
            NhomQuyenDAO.Instance;

        private readonly TaiKhoanDAO taiKhoanDAO =
            TaiKhoanDAO.Instance;

        public TaiKhoanBUS()
        {
            LoadData();
        }

        // 1. Tải danh sách tài khoản từ SQL Server
        public void LoadData()
        {
            try
            {
                listTaiKhoan = taiKhoanDAO.selectAll()
                    ?? new List<TaiKhoanDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Lỗi tải tài khoản: {ex.Message}");

                listTaiKhoan = new List<TaiKhoanDTO>();
                throw;
            }
        }

        // 2. Lấy danh sách tài khoản
        public List<TaiKhoanDTO> GetTaiKhoanAll()
        {
            LoadData();
            return listTaiKhoan.ToList();
        }

        public TaiKhoanDTO GetTaiKhoan(int index)
        {
            if (index < 0 || index >= listTaiKhoan.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return listTaiKhoan[index];
        }

        // 3. Tìm tài khoản theo MaTK
        public int GetTaiKhoanByMaTK(int maTK)
        {
            return listTaiKhoan.FindIndex(
                tk => tk.MaTK == maTK);
        }

        public TaiKhoanDTO? GetTaiKhoanTheoMaTK(int maTK)
        {
            return taiKhoanDAO.selectById(maTK.ToString());
        }

        // Giữ hàm cũ để tương thích với Form.
        // Chỉ trả về vị trí nếu nhân viên có đúng 1 tài khoản.
        public int GetTaiKhoanByMaNV(int manv)
        {
            var matches = listTaiKhoan
                .Select((tk, index) => new { tk, index })
                .Where(x => x.tk.MNV == manv)
                .ToList();

            if (matches.Count != 1)
                return -1;

            return matches[0].index;
        }

        // 4. Lấy thông tin nhóm quyền
        public NhomQuyenDTO GetNhomQuyenDTO(int manhom)
        {
            return nhomQuyenDAO.selectById(
                manhom.ToString());
        }

        // 5. Thêm tài khoản nhân viên
        public void AddAcc(TaiKhoanDTO tk)
        {
            if (tk == null)
                throw new ArgumentNullException(nameof(tk));

            if (!tk.MNV.HasValue)
                throw new Exception(
                    "Tài khoản nhân viên phải có mã nhân viên.");

            if (string.IsNullOrWhiteSpace(tk.TDN))
                throw new Exception(
                    "Tên đăng nhập không được để trống.");

            if (string.IsNullOrWhiteSpace(tk.MK))
                throw new Exception(
                    "Mật khẩu không được để trống.");

            if (!CheckTDN(tk.TDN))
                throw new Exception(
                    "Tên đăng nhập đã tồn tại.");

            tk.TDN = tk.TDN.Trim();

            tk.MK = BCrypt.Net.BCrypt.HashPassword(
                tk.MK, workFactor: 12);

            int result = taiKhoanDAO.insert(tk);

            if (result <= 0)
                throw new Exception(
                    "Không thể thêm tài khoản vào database.");

            LoadData();
        }

        // 6. Cập nhật tài khoản theo MaTK
        public void UpdateAcc(TaiKhoanDTO tk)
        {
            if (tk == null)
                throw new ArgumentNullException(nameof(tk));

            if (tk.MaTK <= 0)
                throw new Exception(
                    "Không xác định được mã tài khoản (MaTK).");

            TaiKhoanDTO? oldTk =
                taiKhoanDAO.selectById(tk.MaTK.ToString());

            if (oldTk == null)
                throw new Exception(
                    "Tài khoản không tồn tại.");

            if (string.IsNullOrWhiteSpace(tk.TDN))
                throw new Exception(
                    "Tên đăng nhập không được để trống.");

            tk.TDN = tk.TDN.Trim();

            // Không cho đổi sang tên đăng nhập của tài khoản khác
            if (listTaiKhoan.Any(x =>
                x.MaTK != tk.MaTK &&
                string.Equals(
                    x.TDN,
                    tk.TDN,
                    StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception(
                    "Tên đăng nhập đã được sử dụng.");
            }

            // Nếu không nhập mật khẩu thì giữ hash cũ
            if (string.IsNullOrWhiteSpace(tk.MK))
            {
                tk.MK = oldTk.MK;
            }
            else if (!tk.MK.StartsWith(
                "$2a$", StringComparison.Ordinal) &&
                     !tk.MK.StartsWith(
                "$2b$", StringComparison.Ordinal) &&
                     !tk.MK.StartsWith(
                "$2x$", StringComparison.Ordinal) &&
                     !tk.MK.StartsWith(
                "$2y$", StringComparison.Ordinal))
            {
                // Mật khẩu mới: băm BCrypt
                tk.MK = BCrypt.Net.BCrypt.HashPassword(
                    tk.MK, workFactor: 12);
            }

            int result = taiKhoanDAO.update(tk);

            if (result <= 0)
                throw new Exception(
                    "Không thể cập nhật tài khoản.");

            LoadData();
        }

        // 7. Xóa mềm tài khoản theo MaTK
        public void DeleteAccByMaTK(int maTK)
        {
            if (maTK <= 0)
                throw new Exception(
                    "Mã tài khoản không hợp lệ.");

            int result = taiKhoanDAO.delete(
                maTK.ToString());

            if (result <= 0)
                throw new Exception(
                    "Không thể xóa tài khoản.");

            LoadData();
        }

        // Hàm cũ nhận MNV, không còn an toàn
        // vì 1 nhân viên có thể có nhiều tài khoản.
        public void DeleteAcc(int manv)
        {
            throw new InvalidOperationException(
                "Chức năng xóa đã chuyển sang MaTK. " +
                "Hãy cập nhật Form gọi DeleteAccByMaTK().");
        }

        // 8. Kiểm tra tên đăng nhập
        // Kiểm tra cả nhân viên và khách hàng
        public bool CheckTDN(string TDN)
        {
            if (string.IsNullOrWhiteSpace(TDN))
                return false;

            LoadData();

            return !listTaiKhoan.Any(tk =>
                string.Equals(
                    tk.TDN,
                    TDN.Trim(),
                    StringComparison.OrdinalIgnoreCase));
        }

        // 9. Tìm kiếm tài khoản
        public List<TaiKhoanDTO> Search(string txt, string type)
        {
            txt = (txt ?? "").Trim().ToLowerInvariant();

            IEnumerable<TaiKhoanDTO> query = listTaiKhoan;

            switch (type)
            {
                case "Mã nhân viên":
                    query = query.Where(tk =>
                        tk.MNV?.ToString().Contains(txt) == true);
                    break;

                case "Mã tài khoản":
                    query = query.Where(tk =>
                        tk.MaTK.ToString().Contains(txt));
                    break;

                case "Username":
                case "Tên đăng nhập":
                    query = query.Where(tk =>
                        tk.TDN.ToLowerInvariant().Contains(txt));
                    break;

                case "Loại tài khoản":
                    query = query.Where(tk =>
                        tk.LoaiTaiKhoan.ToLowerInvariant()
                            .Contains(txt));
                    break;

                default:
                    query = query.Where(tk =>
                        tk.MaTK.ToString().Contains(txt) ||
                        (tk.MNV?.ToString().Contains(txt) == true) ||
                        (tk.MKH?.ToString().Contains(txt) == true) ||
                        tk.TDN.ToLowerInvariant().Contains(txt) ||
                        tk.LoaiTaiKhoan.ToLowerInvariant()
                            .Contains(txt));
                    break;
            }

            return query.ToList();
        }

        // 10. Đăng nhập Desktop
        // Chỉ tài khoản nhân viên được đăng nhập Desktop
        public TaiKhoanDTO? DangNhap(
            string tenDangNhap,
            string matKhau)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenDangNhap) ||
                    string.IsNullOrWhiteSpace(matKhau))
                    return null;

                TaiKhoanDTO? tk =
                    taiKhoanDAO.selectByUser(
                        tenDangNhap.Trim());

                if (tk == null ||
                    !tk.MNV.HasValue ||
                    tk.TT != 1)
                {
                    return null;
                }

                return BCrypt.Net.BCrypt.Verify(
                    matKhau, tk.MK)
                    ? tk
                    : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Lỗi đăng nhập: {ex.Message}");

                return null;
            }
        }

        // 11. Giữ tương thích với interface hiện tại
        public int getAutoIncrement()
        {
            return taiKhoanDAO.getAutoIncrement();
        }
    }
}
