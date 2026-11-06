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
