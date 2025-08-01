import { defineConfig } from 'vite';
import angular from '@analogjs/vite-plugin-angular';

export default defineConfig({
  plugins: [angular()],
  server: {
    host: '0.0.0.0',
    port: 4200,
    strictPort: true,
    proxy: {
      '/api': {
        target: 'https://localhost:5001',
        changeOrigin: true,
        secure: false
      },
      '/hubs': {
        target: 'https://localhost:5001',
        changeOrigin: true,
        secure: false,
        ws: true
      }
    }
  }
});