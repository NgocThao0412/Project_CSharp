import React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import SiteLayout from './GUI/ThanhPhan/BoCucTrang.jsx';
import HomePage from './GUI/TrangChu.jsx';
import ProductsPage from './GUI/DanhSachSanPham.jsx';
import ProductDetailsPage from './GUI/ChiTietSanPham.jsx';
import NotFoundPage from './GUI/KhongTimThay.jsx';
import GioHang from './GUI/GioHang.jsx';
import TaiKhoan from './GUI/TaiKhoan.jsx';
import TraCuuDonHang from './GUI/TraCuuDonHang.jsx';
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
          <Route path="tai-khoan/dang-nhap" element={<TaiKhoan />} />
          <Route path="tai-khoan/dang-ky" element={<TaiKhoan />} />
          <Route path="don-hang/tra-cuu" element={<TraCuuDonHang />} />
          <Route path="*" element={<NotFoundPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  </React.StrictMode>
);
