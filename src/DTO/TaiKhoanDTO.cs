
using System;

namespace src.DTO
{
    public class TaiKhoanDTO
    {
        // Khóa chính của bảng TAIKHOAN
        public int MaTK { get; set; }

        // Mã nhân viên và mã khách hàng có thể NULL
        public int? MNV { get; set; }
        public int? MKH { get; set; }

        public string TDN { get; set; } = string.Empty;
        public string MK { get; set; } = string.Empty;
        public int MNQ { get; set; }
        public int TT { get; set; }

        // Phân biệt tài khoản nhân viên và khách hàng
        public string LoaiTaiKhoan
        {
            get
            {
                if (MKH.HasValue)
                    return "Khách hàng";

                if (MNV.HasValue)
                    return "Nhân viên";

                return "Chưa xác định";
            }
        }

        public TaiKhoanDTO()
        {
        }

        // Constructor cũ - giữ tương thích với code Desktop
        public TaiKhoanDTO(
            int MNV,
            string TDN,
            string MK,
            int MNQ,
            int TT)
        {
            this.MNV = MNV;
            this.MKH = null;
            this.TDN = TDN;
            this.MK = MK;
            this.MNQ = MNQ;
            this.TT = TT;
        }

        // Constructor mới - dùng chung Desktop và Web
        public TaiKhoanDTO(
            int maTK,
            int? mnv,
            int? mkh,
            string tdn,
            string mk,
            int mnq,
            int tt)
        {
            MaTK = maTK;
            MNV = mnv;
            MKH = mkh;
            TDN = tdn;
            MK = mk;
            MNQ = mnq;
            TT = tt;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is not TaiKhoanDTO other)
                return false;

            // Tài khoản đã lưu: so sánh theo khóa chính
            if (MaTK > 0 || other.MaTK > 0)
                return MaTK > 0
                    && other.MaTK > 0
                    && MaTK == other.MaTK;

            // Tài khoản chưa lưu
            return MNV == other.MNV
                && MKH == other.MKH
                && TDN == other.TDN
                && MK == other.MK
                && MNQ == other.MNQ
                && TT == other.TT;
        }

        public override int GetHashCode()
        {
            if (MaTK > 0)
                return MaTK.GetHashCode();

            return HashCode.Combine(
                MNV, MKH, TDN, MK, MNQ, TT);
        }

        public override string ToString()
        {
            // Không đưa mật khẩu hoặc hash vào chuỗi hiển thị
            return $"MaTK={MaTK}, " +
                   $"LoaiTaiKhoan={LoaiTaiKhoan}, " +
                   $"MNV={MNV}, MKH={MKH}, " +
                   $"TDN={TDN}, MNQ={MNQ}, TT={TT}";
        }
    }
}
