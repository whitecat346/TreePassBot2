// src/services/api.ts
import axios from 'axios';

const apiClient = axios.create({
    baseURL: 'http://localhost:5001/api',
    headers: {
        'Content-Type': 'application/json',
    },
});

// 类型定义
export interface Plugin {
    id: string;
    name: string;
    description: string;
    isEnabled: boolean;
}

export interface BotStatus {
    status: string;
    startTime: string | null;
}

// 插件 API
export const getPlugins = () => apiClient.get<Plugin[]>('/plugins');
export const togglePlugin = (pluginId: string) => apiClient.post(`/plugins/${pluginId}/toggle`);

// 机器人状态 API
export const getBotStatus = () => apiClient.get<BotStatus>('/bot/status');
export const startBot = () => apiClient.post('/bot/start');
export const stopBot = () => apiClient.post('/bot/stop');
