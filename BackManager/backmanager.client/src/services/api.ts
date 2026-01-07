// src/services/api.ts
import axios from 'axios';
import type { AxiosError, AxiosInstance, AxiosResponse } from 'axios';
import type { InternalAxiosRequestConfig } from 'axios';

// 创建axios实例
const apiClient: AxiosInstance = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 5000, // 请求超时时间
});

// 统一响应类型
export interface ApiResponse<T = unknown> {
  success: boolean;
  message: string;
  data: T;
  code: number;
}

// 请求拦截器
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // 确保headers属性已初始化
    if (!config.headers) {
      config.headers = new axios.AxiosHeaders();
    }

    // 可以在这里添加token等认证信息
    // const token = localStorage.getItem('token');
    // if (token) {
    //     config.headers.Authorization = `Bearer ${token}`;
    // }

    return config;
  },
  (error: AxiosError) => {
    return Promise.reject(error);
  }
);

// 响应拦截器
apiClient.interceptors.response.use(
  (response: AxiosResponse<ApiResponse>) => {
    // 统一处理响应数据
    return response;
  },
  (error: AxiosError<ApiResponse>) => {
    // 统一处理错误
    let errorMessage = '请求失败，请稍后重试';

    if (error.response) {
      // 服务器返回了错误状态码
      const { data, status } = error.response;

      if (data?.message) {
        errorMessage = data.message;
      } else {
        // 根据状态码返回不同的错误信息
        switch (status) {
          case 400:
            errorMessage = '请求参数错误';
            break;
          case 401:
            errorMessage = '未授权，请重新登录';
            // 可以在这里处理跳转到登录页
            break;
          case 403:
            errorMessage = '拒绝访问';
            break;
          case 404:
            errorMessage = '请求的资源不存在';
            break;
          case 500:
            errorMessage = '服务器内部错误';
            break;
          case 502:
            errorMessage = '网关错误';
            break;
          case 503:
            errorMessage = '服务器繁忙，请稍后重试';
            break;
          case 504:
            errorMessage = '网关超时';
            break;
          default:
            errorMessage = `请求失败，状态码：${status}`;
        }
      }
    } else if (error.request) {
      // 请求已发送，但没有收到响应
      errorMessage = '服务器无响应，请检查网络连接';
    } else {
      // 请求配置错误
      errorMessage = error.message || '请求失败';
    }

    // 可以在这里添加全局错误提示
    console.error('API Error:', errorMessage);

    // 返回标准化的错误响应
    return Promise.reject({
      success: false,
      message: errorMessage,
      data: null,
      code: error.response?.status || -1,
    });
  }
);

// 机器人状态 API
export interface BotStatus {
  status: 'Running' | 'Stopped' | 'Error';
  startTime: string | null;
  version: string;
  protocol: string;
}

// 服务器状态 API
export interface ServerStatus {
  cpuUsage: number;
  memoryUsage: number;
  diskUsage: number;
  uptime: string;
  timestamp: string;
}

// 插件 API
export type PluginStatus = 'Running' | 'Warning' | 'Error' | 'Disabled';

export interface Plugin {
  id: string;
  name: string;
  description: string;
  isEnabled: boolean;
  status: PluginStatus;
  version: string;
}

// 用户管理 API
export interface Group {
  groupId: string;
  name: string;
  memberCount: number;
  ownerId: number;
}

export interface GroupMember {
  groupId: string;
  userId: string;
  username: string;
  nickname: string;
  role: 'Owner' | 'Admin' | 'Member' | 'Bot';
  joinedAt: string;
}

// 审核记录 API
export type AuditStatus = 'Pending' | 'Approved' | 'Rejected';

export interface AuditRecord {
  id: string;
  userId: string;
  username: string;
  groupId: string;
  groupName: string;
  status: AuditStatus;
  verificationCode: string | null;
  enteredGroup: boolean;
  createdAt: string;
  processedAt: string | null;
  processedBy: string | null;
}

// 消息日志 API
export interface MessageLog {
  id: string;
  groupId: string;
  groupName: string;
  userId: string;
  username: string;
  content: string;
  sendAt: string;
  isRecalled: boolean;
  recalledBy: string | null;
  recalledAt: string | null;
}

export interface MessageLogResponse {
  items: MessageLog[];
  hasMore: boolean;
  nextCursor: string | null;
  total: number;
}

// 机器人状态 API
export const getBotStatus = () => apiClient.get<ApiResponse<BotStatus>>('/bot/status');
export const startBot = () => apiClient.post<ApiResponse<void>>('/bot/start');
export const stopBot = () => apiClient.post<ApiResponse<void>>('/bot/stop');
export const getTotalMemberCount = () => apiClient.get<ApiResponse<number>>('/groups/totalMemberCount');
export const getTotalGroupCount = () => apiClient.get<ApiResponse<number>>('/groups/totalGroupCount');
export const getEnabledPluginCount = () => apiClient.get<ApiResponse<number>>('/plugins/enabledCount');

// 服务器状态 API
export const getServerStatus = () => apiClient.get<ApiResponse<ServerStatus>>('/server/status');

// 插件 API
export const getPlugins = () => apiClient.get<ApiResponse<Plugin[]>>('/plugins');
export const togglePlugin = (pluginId: string) => apiClient.post<ApiResponse<void>>(`/plugins/${pluginId}/toggle`);
export const uploadPlugin = (formData: FormData) => apiClient.post<ApiResponse<Plugin>>('/plugins/upload', formData, {
  headers: {
    'Content-Type': 'multipart/form-data',
  },
});

// 用户管理 API
export const getGroups = () => apiClient.get<ApiResponse<Group[]>>('/groups');
export const getGroupMemberCount = (groupId: string) => apiClient.get<ApiResponse<{ count: number }>>(`/groups/${groupId}/count`);
export const getGroupMembers = (groupId: string, params?: {
  limit: number;
  skip: number;
}) => apiClient.get<ApiResponse<GroupMember[]>>(`/groups/${groupId}/members`, { params });

// 审核记录 API
export const getAuditRecords = () => apiClient.get<ApiResponse<AuditRecord[]>>('/audits');
export const approveAudit = (auditId: string) => apiClient.post<ApiResponse<void>>(`/audits/${auditId}/approve`);
export const rejectAudit = (auditId: string) => apiClient.post<ApiResponse<void>>(`/audits/${auditId}/reject`);
export const resetAuditStatus = (auditId: string) => apiClient.post<ApiResponse<void>>(`/audits/${auditId}/reset`);
export const resendVerificationCode = (auditId: string) => apiClient.post<ApiResponse<void>>(`/audits/${auditId}/resend-code`);
export const getEnteredGroupStatus = (auditId: string) => apiClient.get<ApiResponse<{ enteredGroup: boolean }>>(`/audits/${auditId}/entered-group`);
export const updateAuditRecordStatus = (auditId: string) => apiClient.get<ApiResponse<AuditRecord>>(`/audits/${auditId}`);

// 消息日志 API
export const getMessageLogs = (params?: {
  groupId?: string;
  startTime?: string;
  endTime?: string;
  beforeId?: string; // 基于ID的分页
  limit?: number;
}) => apiClient.get<ApiResponse<MessageLogResponse>>('/messages', { params });
export const getMessageDetail = (messageId: string) => apiClient.get<ApiResponse<MessageLog>>(`/messages/${messageId}`);

export default apiClient;
