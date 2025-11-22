import axios from 'axios';

// 自动根据环境判断前缀，配合 Vite Proxy 使用
const apiClient = axios.create({
  baseURL: '/api',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// 响应拦截器 (统一处理错误)
apiClient.interceptors.response.use(
  (response) => response.data,
  (error) => {
    console.error('API Error:', error.response?.data || error.message);
    // 这里可以集成 Toast 提示
    return Promise.reject(error);
  }
);

export default apiClient;
