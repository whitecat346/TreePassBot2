<script setup lang="ts">
defineOptions({
  name: 'UsersView'
});
import { ref, onMounted } from 'vue';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import { Filter, Plus, MoreHorizontal, Download } from 'lucide-vue-next';
import type { User } from '@/types'; // 引用我们定义的类型
import apiClient from '@/api/client';

// 状态定义
const users = ref<User[]>([]);
const loading = ref(true);
const selectedUsers = ref<User[]>([]);

// 模拟加载数据 (实际应调用 apiClient.get('/users'))
onMounted(async () => {
  loading.value = true;
  try {
    // const res = await apiClient.get<User[]>('/users');
    // users.value = res.data;

    // 模拟数据
    await new Promise(r => setTimeout(r, 800));
    users.value = [
      { id: '1', qqId: 123456789, nickname: 'Makabaka', role: 'Admin', lastSeenAt: '2025-11-22T10:00:00Z', createdAt: '2025-01-01', status: 'Active' } as any,
      { id: '2', qqId: 987654321, nickname: 'Spammer', role: 'User', lastSeenAt: '2025-11-20T10:00:00Z', createdAt: '2025-02-01', status: 'Banned' } as any,
    ];
  } finally {
    loading.value = false;
  }
});

const getStatusColor = (status: string) => {
  return status === 'Active'
    ? 'bg-green-50 text-green-700 border-green-200 ring-green-500/20'
    : 'bg-red-50 text-red-700 border-red-200 ring-red-500/20';
};
</script>

<template>
  <div class="h-full flex flex-col">
    <!-- Header Actions -->
    <div class="flex justify-between items-center mb-5 shrink-0">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">用户管理</h1>
        <p class="text-sm text-gray-500 mt-1">管理所有已注册的 QQ 用户及其权限。</p>
      </div>
      <div class="flex space-x-3">
        <button class="flex items-center px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 shadow-sm transition-all">
          <Filter class="w-4 h-4 mr-2" /> 筛选
        </button>
        <button class="flex items-center px-4 py-2 text-sm font-medium text-white bg-indigo-600 rounded-lg hover:bg-indigo-700 shadow-sm shadow-indigo-200 transition-all">
          <Plus class="w-4 h-4 mr-2" /> 新增用户
        </button>
      </div>
    </div>

    <!-- Data Table Area -->
    <div class="flex-1 bg-white border border-gray-200 rounded-xl shadow-sm overflow-hidden flex flex-col">
      <DataTable
        :value="users"
        v-model:selection="selectedUsers"
        dataKey="id"
        :loading="loading"
        scrollable
        scrollHeight="flex"
        tableStyle="min-width: 50rem"
        :pt="{
          headerRow: { class: 'bg-gray-50 border-b border-gray-100' },
          headerCell: { class: 'py-3.5 px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider text-left' },
          bodyRow: { class: 'hover:bg-gray-50/80 transition-colors border-b border-gray-100 last:border-0' },
          bodyCell: { class: 'py-3 px-4 text-sm text-gray-700' }
        }"
      >
        <Column selectionMode="multiple" headerStyle="width: 3rem"></Column>

        <Column field="qqId" header="QQ ID" sortable>
          <template #body="{ data }">
            <span class="font-mono text-gray-600 bg-gray-100 px-1.5 py-0.5 rounded text-xs">{{ data.qqId }}</span>
          </template>
        </Column>

        <Column field="nickname" header="昵称" sortable>
          <template #body="{ data }">
            <div class="flex items-center">
              <div class="w-8 h-8 rounded-full bg-gradient-to-br from-indigo-400 to-purple-500 text-white flex items-center justify-center text-xs font-bold mr-3 shadow-sm">
                {{ data.nickname?.[0]?.toUpperCase() }}
              </div>
              <span class="font-medium text-gray-900">{{ data.nickname }}</span>
            </div>
          </template>
        </Column>

        <Column field="role" header="角色" sortable>
          <template #body="{ data }">
             <span class="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-gray-100 text-gray-800 border border-gray-200">
               {{ data.role }}
             </span>
          </template>
        </Column>

        <Column field="status" header="状态">
          <template #body="{ data }">
            <span :class="['inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ring-1 ring-inset', getStatusColor(data.status)]">
              <span class="w-1.5 h-1.5 rounded-full bg-current mr-1.5 opacity-60"></span>
              {{ data.status }}
            </span>
          </template>
        </Column>

        <Column field="lastSeenAt" header="最后活跃" sortable>
           <template #body="{ data }">
             <span class="text-gray-500">{{ new Date(data.lastSeenAt).toLocaleString() }}</span>
           </template>
        </Column>

        <Column header="操作" style="width: 4rem; text-align: center">
          <template #body>
            <button class="p-1.5 text-gray-400 hover:text-gray-600 hover:bg-gray-100 rounded-md transition-colors">
              <MoreHorizontal class="w-4 h-4" />
            </button>
          </template>
        </Column>
      </DataTable>
    </div>

    <!-- Footer -->
    <div class="h-12 border-t border-gray-200 bg-gray-50 flex items-center justify-between px-6 shrink-0">
      <span class="text-xs text-gray-500">共 {{ users.length }} 条记录</span>
      <!-- Pagination placeholder -->
    </div>
  </div>
</template>
