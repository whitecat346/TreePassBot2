// 通用响应结构
export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

// --- 用户实体 (对应 C# BotUser) ---
export enum UserRole {
  User = 'User',
  Auditor = 'Auditor',
  Admin = 'Admin',
  Owner = 'Owner'
}

export interface User {
  id: string;        // Guid
  qqId: number;      // ulong -> number
  nickname: string;
  role: UserRole;
  lastSeenAt: string; // DateTimeOffset -> ISO string
  createdAt: string;
  status: 'Active' | 'Banned'; // 前端计算属性
}

// --- 插件实体 (对应 C# PluginMeta + Supervisor Status) ---
export interface Plugin {
  id: string;
  name: string;
  version: string;
  author: string;
  description: string;
  status: 'Running' | 'Stopped' | 'Crashed';
  errorCount?: number;
}

// --- 审核请求实体 (对应 C# AuditRequest) ---
export enum AuditStatus {
  Pending = 0,
  Approved = 1,
  Rejected = 2,
  Expired = 3
}

export interface AuditRequest {
  id: string;
  requestQqId: number;
  targetGroupId: number;
  passcode: string;
  status: AuditStatus;
  formData: Record<string, any>; // JSONB
  createdAt: string;
}
