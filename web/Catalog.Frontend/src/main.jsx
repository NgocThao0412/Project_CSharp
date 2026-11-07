import React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import SiteLayout from './GUI/ThanhPhan/BoCucTrang.jsx';
import HomePage from './GUI/TrangChu.jsx';
import ProductsPage from './GUI/DanhSachSanPham.jsx';
import ProductDetailsPage from './GUI/ChiTietSanPham.jsx';
import NotFoundPage from './GUI/KhongTimThay.jsx';
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
          <Route path="*" element={<NotFoundPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  </React.StrictMode>
);
