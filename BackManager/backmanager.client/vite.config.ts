import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-vue';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';

// Certificate generation logic - keep existing functionality
const baseFolder =
    env.APPDATA !== undefined && env.APPDATA !== ''
        ? `${env.APPDATA}/ASP.NET/https`
        : `${env.HOME}/.aspnet/https`;

const certificateName = "backmanager.client";
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync(baseFolder)) {
    fs.mkdirSync(baseFolder, { recursive: true });
}

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
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

// Get API target from environment variables or use default
const apiTarget = env.VITE_API_URL ?? 'https://localhost:7248';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [plugin()],
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
        https: {
            key: fs.readFileSync(keyFilePath),
            cert: fs.readFileSync(certFilePath),
        },
        // Auto open browser on server start
        open: true
    },
    build: {
        // Production build optimization
        minify: 'terser',
        sourcemap: false,
        rollupOptions: {
            output: {
                manualChunks: {
                    // Optimize chunk splitting for better caching
                    vue: ['vue', 'vue-router', 'pinia'],
                    ui: ['element-plus', 'primevue'],
                    chart: ['chart.js', 'vue-chartjs']
                }
            }
        }
    }
})
