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
      '/images': { target: 'http://127.0.0.1:5080', changeOrigin: true },
    },
  },
  build: {
    // React build output is served by ASP.NET Core.
    outDir: '../Catalog.Web/wwwroot',
    emptyOutDir: false,
  },
});
