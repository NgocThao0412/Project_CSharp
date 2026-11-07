import { Link } from 'react-router-dom';
import { useApi } from '../api.js';
import ProductCard from '../components/ProductCard.jsx';
import LoadState from '../components/LoadState.jsx';

function BenefitIcon({ name }) {
  const shared = { width: 45, height: 45, viewBox: '0 0 48 48', fill: 'none', stroke: 'currentColor',
    strokeWidth: 2.3, strokeLinecap: 'round', strokeLinejoin: 'round', 'aria-hidden': true };
  if (name === 'truck') return <svg {...shared}>
    <path d="M4 12h23v20H4zM27 19h9l8 9v4H27zM4 27H2" />
    <path d="M4 18H1M8 34a5 5 0 1 0 10 0 5 5 0 0 0-10 0ZM33 34a5 5 0 1 0 10 0 5 5 0 0 0-10 0Z" />
  </svg>;
  if (name === 'shield') return <svg {...shared}>
    <path d="M24 4 40 10v12c0 12-7.5 19-16 22C15.5 41 8 34 8 22V10L24 4Z" />
    <path d="m16 23 5 5 11-12" />
  </svg>;
  if (name === 'tag') return <svg {...shared}>
    <path d="M26 5H14L4 15l24 24 16-16-18-18Z" />
    <circle cx="22" cy="15" r="2.8" />
  </svg>;
  return <svg {...shared}>
    <path d="M7 26v-4C7 11 14 4 24 4s17 7 17 18v4" />
    <rect x="4" y="23" width="7" height="14" rx="3" />
    <rect x="37" y="23" width="7" height="14" rx="3" />
    <path d="M40 38c0 4-6 6-12 6h-3" /><rect x="20" y="40" width="9" height="5" rx="2.5" />
  </svg>;
}

const benefits = [
  { icon: 'truck', title: 'Giao hàng nhanh', detail: 'Toàn quốc' },
  { icon: 'shield', title: 'Bảo hành chính hãng', detail: 'Đổi trả dễ dàng' },
  { icon: 'tag', title: 'Giá tốt mỗi ngày', detail: 'Nhiều ưu đãi hấp dẫn' },
  { icon: 'headset', title: 'Hỗ trợ 24/7', detail: 'Tư vấn tận tình' },
];

export default function HomePage() {
  const { data, loading, error } = useApi('/api/catalog/featured?count=6');

  return <>
    <section className="store-hero" aria-label="Giới thiệu cửa hàng">
      <div className="hero-live-copy">
        <h1>Nâng cấp hiệu năng<br />cho máy tính của bạn</h1>
        <p>Linh kiện chính hãng <span>·</span> Giá tốt <span>·</span> Bảo hành uy tín</p>
      </div>
      <img className="hero-mobile-image" src="/images/hero-parts.webp"
        alt="CPU AMD Ryzen, bộ nhớ RAM RGB và card đồ họa" />
      <div className="hero-slide-markers" aria-label="Banner giới thiệu, hình 1 trong 3">
        <span className="active" /><span /><span />
      </div>
    </section>

    <section className="benefits-section" aria-label="Ưu điểm của cửa hàng">
      <div className="benefits-grid">
        {benefits.map(item => <div className="benefit-card" key={item.title}>
          <div className="benefit-icon"><BenefitIcon name={item.icon} /></div>
          <div className="benefit-copy"><strong>{item.title}</strong><span>{item.detail}</span></div>
        </div>)}
      </div>
    </section>

    <section className="page-container section home-products">
      <div className="section-heading">
        <div><span className="eyebrow blue">DANH MỤC ĐỀ XUẤT</span><h2>Sản phẩm nổi bật</h2></div>
        <Link to="/san-pham">Xem tất cả →</Link>
      </div>
      <LoadState loading={loading} error={error} />
      {!loading && !error && <div className="product-grid">
        {(data ?? []).map(item => <ProductCard key={item.id} product={item} />)}
      </div>}
    </section>
  </>;
}
