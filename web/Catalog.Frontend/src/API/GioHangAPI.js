async function goi(url, method = 'GET', data = null) {
  const response = await fetch(url, {
    method,
    credentials: 'same-origin',
    headers: { Accept: 'application/json', ...(data ? { 'Content-Type': 'application/json' } : {}) },
    ...(data ? { body: JSON.stringify(data) } : {}),
  });
  const result = await response.json().catch(() => null);
  if (!response.ok) {
    throw new Error(result?.message || result?.detail || `Không xử lý được yêu cầu (HTTP ${response.status}).`);
  }
  return result;
}

export const layGioHang = () => goi('/api/gio-hang');
export const themGioHang = (productId, quantity = 1) =>
  goi('/api/gio-hang/them', 'POST', { productId, quantity });
export const capNhatGioHang = (productId, quantity) =>
  goi('/api/gio-hang/cap-nhat', 'POST', { productId, quantity });
export const xoaGioHang = productId =>
  goi('/api/gio-hang/xoa', 'POST', { productId });
export const traCuuDonHang = (maDon, soDienThoai) =>
  goi('/api/don-hang/tra-cuu', 'POST', { maDon: Number(maDon), soDienThoai });
