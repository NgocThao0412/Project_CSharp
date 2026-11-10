import { useEffect, useState } from 'react';

export function useApi(path) {
  const [state, setState] = useState({ data: null, loading: true, error: null });

  useEffect(() => {
    const controller = new AbortController();
    setState({ data: null, loading: true, error: null });

    fetch(path, { signal: controller.signal, headers: { Accept: 'application/json' } })
      .then(async response => {
        if (!response.ok) {
          throw new Error(response.status === 404 ? 'Không tìm thấy sản phẩm.' : `Không tải được dữ liệu (HTTP ${response.status}).`);
        }
        return response.json();
      })
      .then(data => setState({ data, loading: false, error: null }))
      .catch(error => {
        if (error.name !== 'AbortError') {
          setState({ data: null, loading: false, error: error.message });
        }
      });

    return () => controller.abort();
  }, [path]);

  return state;
}

export const money = price => `${new Intl.NumberFormat('vi-VN').format(price)} ₫`;

async function goiTaiKhoan(path, thongTin) {
  const response = await fetch(path, {
    method: thongTin === undefined ? 'GET' : 'POST',
    credentials: 'same-origin',
    headers: { Accept: 'application/json', ...(thongTin === undefined ? {} : { 'Content-Type': 'application/json' }) },
    ...(thongTin === undefined ? {} : { body: JSON.stringify(thongTin) }),
  });
  const result = await response.json().catch(() => null);
  if (!response.ok) {
    const loiTruong = result?.errors && Object.values(result.errors).flat().find(Boolean);
    throw new Error(result?.message || result?.detail || loiTruong || `Không xử lý được yêu cầu (HTTP ${response.status}).`);
  }
  return result;
}

export const trangThaiTaiKhoan = () => goiTaiKhoan('/api/tai-khoan/trang-thai');
export const dangNhapTaiKhoan = thongTin => goiTaiKhoan('/api/tai-khoan/dang-nhap', thongTin);
export const dangKyTaiKhoan = thongTin => goiTaiKhoan('/api/tai-khoan/dang-ky', thongTin);
export const dangXuatTaiKhoan = () => goiTaiKhoan('/api/tai-khoan/dang-xuat', {});
