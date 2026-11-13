import React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import SiteLayout from './GUI/ThanhPhan/BoCucTrang.jsx';
import HomePage from './GUI/TrangChu.jsx';
import ProductsPage from './GUI/DanhSachSanPham.jsx';
import ProductDetailsPage from './GUI/ChiTietSanPham.jsx';
import NotFoundPage from './GUI/KhongTimThay.jsx';
import GioHang from './GUI/GioHang.jsx';
import DatHang from './GUI/DatHang.jsx';
import DatHangThanhCong from './GUI/DatHangThanhCong.jsx';
import TaiKhoan from './GUI/TaiKhoan.jsx';
import DonHangCuaToi from './GUI/DonHangCuaToi.jsx';
import './site.css';

createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <BrowserRouter>
      <Routes>
        <Route element={<SiteLayout />}>
          <Route index element={<HomePage />} />
          <Route path="san-pham" element={<ProductsPage />} />
          <Route path="tim-kiem" element={<ProductsPage />} />
          <Route path="san-pham/:id" element={<ProductDetailsPage />} />
          <Route path="gio-hang" element={<GioHang />} />
          <Route path="dat-hang" element={<DatHang />} />
          <Route path="dat-hang/thanh-cong" element={<DatHangThanhCong />} />
          <Route path="tai-khoan/dang-nhap" element={<TaiKhoan />} />
          <Route path="tai-khoan/dang-ky" element={<TaiKhoan />} />
          <Route path="tai-khoan/thong-tin" element={<TaiKhoan />} />
          <Route path="tai-khoan/don-hang" element={<DonHangCuaToi />} />
          <Route path="tai-khoan/don-hang/:maDon" element={<DonHangCuaToi />} />
          <Route path="*" element={<NotFoundPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  </React.StrictMode>
);
