export default function ShopIcon({ name, size = 20, strokeWidth = 1.9, ...props }) {
  const shared = { fill: 'none', stroke: 'currentColor', strokeWidth, strokeLinecap: 'round', strokeLinejoin: 'round' };
  const paths = {
    search: <><circle cx="10.8" cy="10.8" r="6.9" /><path d="m16.1 16.1 5 5" /></>,
    home: <><path d="m3 10 9-7 9 7v10a1 1 0 0 1-1 1h-5v-7H9v7H4a1 1 0 0 1-1-1z" /></>,
    box: <><path d="m12 2 9 5-9 5-9-5z" /><path d="M3 7v10l9 5 9-5V7" /><path d="M12 12v10" /></>,
    newspaper: <><rect x="4" y="3" width="16" height="18" rx="2" /><path d="M8 7h8M8 11h8M8 15h5" /></>,
    phone: <path d="M7.1 2.8 4.6 4.3c-1.6 1-1.9 2.8-1.2 4.8 2.1 6 6.8 10.8 12.8 12.8 2 .7 3.8.3 4.8-1.2l1.5-2.5-4.6-3.1-2.2 2.1a17.2 17.2 0 0 1-7.1-7.1l2.1-2.2-3.1-4.6Z" />,
    cart: <><circle cx="9" cy="20" r="1" /><circle cx="19" cy="20" r="1" /><path d="M2 3h2l3.1 13h12.1l2.4-9H5" /></>,
    user: <><circle cx="12" cy="8" r="4" /><path d="M4 21v-2a8 8 0 0 1 16 0v2" /></>,
    logout: <><path d="M10 3H5a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h5" /><path d="M13 8l5 4-5 4M8 12h10" /></>,
    list: <><path d="M9 6h13M9 12h13M9 18h13" /><circle cx="4" cy="6" r="1" /><circle cx="4" cy="12" r="1" /><circle cx="4" cy="18" r="1" /></>,
    filter: <><path d="M3 5h18l-7 8v6l-4 2v-8z" /></>,
    reset: <><path d="M3 11a9 9 0 1 1 2.5 6.5" /><path d="M3 4v7h7" /></>,
    heart: <path d="M20.8 4.8a5.4 5.4 0 0 0-7.6 0L12 6l-1.2-1.2a5.4 5.4 0 0 0-7.6 7.6L12 21l8.8-8.6a5.4 5.4 0 0 0 0-7.6Z" />,
    star: <path d="m12 2.5 3 6.3 7 .9-5 4.9 1.2 7-6.2-3.3-6.2 3.3 1.2-7-5-4.9 7-.9z" />,
    chevron: <path d="m7 10 5 5 5-5" />,
    left: <path d="m15 18-6-6 6-6" />,
    right: <path d="m9 18 6-6-6-6" />,
    check: <path d="m5 12 5 5 9-9" />,
    grid: <><rect x="3" y="3" width="7" height="7" rx="1" /><rect x="14" y="3" width="7" height="7" rx="1" /><rect x="3" y="14" width="7" height="7" rx="1" /><rect x="14" y="14" width="7" height="7" rx="1" /></>,
  };
  return <svg width={size} height={size} viewBox="0 0 24 24" aria-hidden="true" {...props} {...shared}>{paths[name] ?? paths.box}</svg>;
}
