import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import inject from '@rollup/plugin-inject'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), inject({
    $: 'jquery', // this caused warnings for all my scss files that had $variable
    jQuery: 'jquery',
  })],
  optimizeDeps: {
    include: ['jquery'],
  },
  server: {
    proxy: {
      "/api": {
        ws: true,
        changeOrigin: true,
        target: "http://ec2-63-32-159-120.eu-west-1.compute.amazonaws.com:5001"
      }
    }
  }
})
