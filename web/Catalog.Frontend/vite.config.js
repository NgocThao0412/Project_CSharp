import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    host: '0.0.0.0',
    port: 5173,
    strictPort: true,
    proxy: {
      '/api': { target: 'http://127.0.0.1:5080', changeOrigin: true },
    },
  },
  build: {
    // dotnet run ở cổng 5080 có thể phục vụ React khi đã build frontend.
    outDir: '../Catalog.Web/wwwroot',
    emptyOutDir: false,
  },
});
