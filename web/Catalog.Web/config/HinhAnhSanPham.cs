using Catalog.Web.DTO;

namespace Catalog.Web.config;

public static class HinhAnhSanPham
{
    private static readonly IReadOnlyDictionary<int, string> AnhCoSan = new Dictionary<int, string>
    {
        [3] = "mouse_g502.png",
        [4] = "keychron_k2.png",
        [15] = "samsung_g5.png",
        [16] = "dell_u2422.png",
        [17] = "asus_proart.png",
        [18] = "logitech_mx3.png",
        [19] = "logi_k380.png",
        [20] = "sony_xm5.png",
        [22] = "keychron_k8.png",
        [23] = "razer_pad.png",
        [24] = "kingston_ram16.png",
        [25] = "ssd_980pro.png",
        [26] = "ssd_nv2.png",
        [27] = "intel_i9.png",
        [28] = "intel_i5.png",
        [29] = "main_z790.png",
        [30] = "tplink_ax73.png",
        [31] = "tplink_switch.png",
        [32] = "asus_mesh.png",
        [33] = "cable_cat6.png",
        [1002] = "intel_i3.png",
        [1003] = "intel_i5_14400F.png",
        [1004] = "intel_i5_14600KF.png",
        [1005] = "intel_i7.png",
        [1006] = "intel_core.png",
        [1007] = "asus_prime.png",
        [1008] = "asus_tuf.png",
        [1009] = "asus_rog_b650.png",
        [1010] = "kingston_ddr4.png",
        [1011] = "kingston_ddr5.png",
        [1012] = "kingston_ddr5_32gb.png",
        [1013] = "samsung_990pro_1tb.png",
        [1014] = "samsung_990pro_2tb.png",
        [1015] = "kingston_nv3_1tb.png",
        [1016] = "asus_rtx4060.png",
        [1017] = "asus_rtx4070super.png",
        [1018] = "asus_rog_750g.png",
        [1019] = "asus_tuf_gt301.png",
    };

    public static string LayDuongDan(SanPhamDTO sanPham, string webRoot)
    {
        var tuSql = Path.GetFileName((sanPham.HinhAnh ?? "").Replace('\\', '/'));
        if (!string.IsNullOrWhiteSpace(tuSql) && File.Exists(Path.Combine(webRoot, "images", "products", tuSql)))
            return "/images/products/" + Uri.EscapeDataString(tuSql);

        if (AnhCoSan.TryGetValue(sanPham.Ma, out var tenAnh)
            && File.Exists(Path.Combine(webRoot, "images", "products", tenAnh)))
            return "/images/products/" + Uri.EscapeDataString(tenAnh);

        return "/images/catalog/part.svg";
    }
}
