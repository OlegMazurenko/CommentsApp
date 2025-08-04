import { defineConfig } from 'vite';
import angular from '@analogjs/vite-plugin-angular';

export default defineConfig({
  plugins: [angular()],
  server: {
    host: '0.0.0.0',
    port: 8080,
    strictPort: true,
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false
      },
      '/hubs': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false,
        ws: true
      }
    }
  }
});