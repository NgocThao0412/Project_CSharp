async function goi(url, method = 'GET', data = null) {
  const response = await fetch(url, {
    method,
    credentials: 'same-origin',
    headers: { Accept: 'application/json', ...(data ? { 'Content-Type': 'application/json' } : {}) },
    ...(data ? { body: JSON.stringify(data) } : {}),
  });
  const result = await response.json().catch(() => null);
  if (!response.ok) {
    const invalidField = result?.errors && Object.values(result.errors).flat().find(Boolean);
    throw new Error(result?.message || result?.detail || invalidField || `Không xử lý được yêu cầu (HTTP ${response.status}).`);
  }
  if ((url.startsWith('/api/gio-hang') && url !== '/api/gio-hang/mua-ngay' && method !== 'GET') || url === '/api/don-hang/dat') {
    window.dispatchEvent(new Event('gio-hang:changed'));
  }
  return result;
}

export const layGioHang = () => goi('/api/gio-hang');
export const layMuaNgay = () => goi('/api/gio-hang/mua-ngay');
export const muaNgaySanPham = productId => goi('/api/gio-hang/mua-ngay', 'POST', { productId });
export const themGioHang = (productId, quantity = 1) =>
  goi('/api/gio-hang/them', 'POST', { productId, quantity });
export const capNhatGioHang = (productId, quantity) =>
  goi('/api/gio-hang/cap-nhat', 'POST', { productId, quantity });
export const xoaGioHang = productId =>
  goi('/api/gio-hang/xoa', 'POST', { productId });
export const layDanhSachDonHang = () => goi('/api/don-hang/cua-toi');
export const layChiTietDonHang = maDon => goi(`/api/don-hang/${encodeURIComponent(maDon)}`);

// Yeu cau gui chi thong tin nguoi nhan. San pham/gia lay tu Session va SQL o C#.
export const datHang = thongTin => goi('/api/don-hang/dat', 'POST', thongTin);
