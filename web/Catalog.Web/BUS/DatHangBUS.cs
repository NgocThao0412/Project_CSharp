using Catalog.Web.DAO;
using Catalog.Web.DTO;

namespace Catalog.Web.BUS;

// Kiem tra nghiep vu o C#; database se tinh lai gia, ton kho trong transaction.
public sealed class DatHangBUS(GioHangBUS gioHang, DatHangDAO donHang)
{
    public async Task<KetQuaDatHangDTO> DatAsync(YeuCauDatHangDTO yeuCau, int maKhach, CancellationToken ct = default)
    {
        if (maKhach <= 0)
            throw new LoiDatHangException("Vui lòng đăng nhập để đặt hàng.");

        if (yeuCau.MaYeuCau == Guid.Empty)
            throw new LoiDatHangException("Mã xác nhận không hợp lệ. Vui lòng tải lại trang đặt hàng.");

        yeuCau.HoTen = (yeuCau.HoTen ?? "").Trim();
        yeuCau.SoDienThoai = (yeuCau.SoDienThoai ?? "").Trim();
        yeuCau.DiaChi = (yeuCau.DiaChi ?? "").Trim();
        yeuCau.Email = string.IsNullOrWhiteSpace(yeuCau.Email) ? null : yeuCau.Email.Trim();
        yeuCau.GhiChu = string.IsNullOrWhiteSpace(yeuCau.GhiChu) ? null : yeuCau.GhiChu.Trim();

        if (yeuCau.HoTen.Length is < 2 or > 100 || yeuCau.DiaChi.Length is < 10 or > 255 ||
            yeuCau.SoDienThoai.Length != 10 || yeuCau.SoDienThoai[0] != '0' ||
            !yeuCau.SoDienThoai.All(char.IsAsciiDigit) ||
            (yeuCau.GhiChu?.Length ?? 0) > 500)
            throw new LoiDatHangException("Thông tin người nhận không hợp lệ.");

        var ds = gioHang.LayMucDaChon();
        if (ds.Count == 0) throw new LoiDatHangException("Giỏ hàng đang trống.");
        if (ds.Count > 30 || ds.Any(x => x.MaSanPham <= 0 || x.SoLuong is < 1 or > 99))
            throw new LoiDatHangException("Giỏ hàng có số lượng không hợp lệ.");

        // Khong tin gia/ton kho tu frontend, ma san pham duoc lay tu Session cua may chu.
        var ketQua = await donHang.TaoDonAsync(yeuCau, ds, maKhach, ct);
        gioHang.XoaSauDatHang();
        return ketQua;
    }
}
