import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-vue';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';

// Certificate generation logic - keep existing functionality
const certificateName = "backmanager.client";
const certFilePath = "";
const keyFilePath = "";

if (process.env.NODE_ENV !== 'production') {

    const baseFolder =
        process.env.APPDATA !== undefined && process.env.APPDATA !== ''
            ? `${process.env.APPDATA}/ASP.NET/https`
            : `${process.env.HOME}/.aspnet/https`;


    const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
    const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

    if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
        // 这里可能会因为找不到 dotnet 命令报错，但现在有了外层的 if，构建时就不会进来了
        if (0 !== child_process.spawnSync('dotnet', [
            'dev-certs',
            'https',
            '--export-path',
            certFilePath,
            '--format',
            'Pem',
            '--no-password',
        ], { stdio: 'inherit', }).status) {
            throw new Error("Could not create certificate.");
        }
    }
}

// Get API target from environment variables or use default
const apiTarget = env.VITE_API_URL ?? 'https://localhost:7248';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    plugin(),
    {
      name: 'preserve-css-classes',
      enforce: 'post',
      transformIndexHtml(html) {
        // 在HTML中添加一些注释，包含所有响应式类，确保它们被保留
        const responsiveClasses = `
          <!-- Preserve responsive classes -->
          <span class="sm:flex md:flex lg:flex xl:flex 2xl:flex"></span>
          <span class="sm:block md:block lg:block xl:block 2xl:block"></span>
          <span class="sm:hidden md:hidden lg:hidden xl:hidden 2xl:hidden"></span>
          <span class="sm:grid md:grid lg:grid xl:grid 2xl:grid"></span>
          <span class="sm:w-full md:w-full lg:w-full xl:w-full 2xl:w-full"></span>
          <span class="sm:w-1/2 md:w-1/2 lg:w-1/2 xl:w-1/2 2xl:w-1/2"></span>
          <span class="sm:w-1/3 md:w-1/3 lg:w-1/3 xl:w-1/3 2xl:w-1/3"></span>
          <span class="sm:w-1/4 md:w-1/4 lg:w-1/4 xl:w-1/4 2xl:w-1/4"></span>
          <span class="sm:p-4 md:p-4 lg:p-4 xl:p-4 2xl:p-4"></span>
          <span class="sm:p-6 md:p-6 lg:p-6 xl:p-6 2xl:p-6"></span>
          <span class="sm:text-sm md:text-sm lg:text-sm xl:text-sm 2xl:text-sm"></span>
          <span class="sm:text-base md:text-base lg:text-base xl:text-base 2xl:text-base"></span>
          <span class="sm:text-lg md:text-lg lg:text-lg xl:text-lg 2xl:text-lg"></span>
          <span class="sm:text-xl md:text-xl lg:text-xl xl:text-xl 2xl:text-xl"></span>
          <span class="sm:gap-3 md:gap-3 lg:gap-3 xl:gap-3 2xl:gap-3"></span>
          <span class="sm:gap-4 md:gap-4 lg:gap-4 xl:gap-4 2xl:gap-4"></span>
          <span class="sm:gap-6 md:gap-6 lg:gap-6 xl:gap-6 2xl:gap-6"></span>
          <span class="sm:justify-between md:justify-between lg:justify-between xl:justify-between 2xl:justify-between"></span>
          <span class="sm:items-center md:items-center lg:items-center xl:items-center 2xl:items-center"></span>
          <span class="sm:items-start md:items-start lg:items-start xl:items-start 2xl:items-start"></span>
          <span class="sm:mx-12 md:mx-12 lg:mx-12 xl:mx-12 2xl:mx-12"></span>
          <span class="sm:mx-auto md:mx-auto lg:mx-auto xl:mx-auto 2xl:mx-auto"></span>
          <span class="sm:max-w-lg md:max-w-lg lg:max-w-lg xl:max-w-lg 2xl:max-w-lg"></span>
          <span class="sm:max-w-xl md:max-w-xl lg:max-w-xl xl:max-w-xl 2xl:max-w-xl"></span>
          <span class="sm:max-w-2xl md:max-w-2xl lg:max-w-2xl xl:max-w-2xl 2xl:max-w-2xl"></span>
          <span class="sm:w-40 md:w-40 lg:w-40 xl:w-40 2xl:w-40"></span>
          <span class="sm:w-64 md:w-64 lg:w-64 xl:w-64 2xl:w-64"></span>
          <span class="sm:w-80 md:w-80 lg:w-80 xl:w-80 2xl:w-80"></span>
        `;
        return html.replace('</body>', `${responsiveClasses}</body>`);
      }
    }
  ],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src')
    }
  },
  server: {
    proxy: {
      // API proxy configuration
      '^/api': {
        target: apiTarget,
        secure: false,
        changeOrigin: true
      },
      // WebSocket proxy configuration
      '^/ws': {
        target: apiTarget,
        secure: false,
        changeOrigin: true,
        ws: true
      }
    },
    port: 5201,
    https: process.env.NODE_ENV !== 'production' && fs.existsSync(certFilePath) ? {
            key: fs.readFileSync(keyFilePath),
            cert: fs.readFileSync(certFilePath),
        } : undefined,
    // Auto open browser on server start
    open: true
  },
  build: {
    // Production build optimization
    minify: 'terser',
    sourcemap: false,
    // 确保CSS代码不被过度优化，保留所有响应式类
    cssCodeSplit: false,
    rollupOptions: {
      output: {
        manualChunks: {
          // Optimize chunk splitting for better caching
          vue: ['vue', 'vue-router', 'pinia'],
          ui: ['element-plus', 'primevue'],
          chart: ['chart.js', 'vue-chartjs']
        },
        // 确保CSS不被优化掉
        assetFileNames: '[name]-[hash].[ext]',
        chunkFileNames: '[name]-[hash].js',
        entryFileNames: '[name]-[hash].js'
      },
      // 确保CSS中所有类都被保留
      treeshake: false
    }
  }
})
