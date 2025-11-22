<script setup>
import { ref, onMounted } from 'vue';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import { Filter, Download, Plus, MoreHorizontal } from 'lucide-vue-next';

// 模拟数据
const users = ref([
  { id: 1, qq: '123456789', nickname: 'Makabaka', role: 'Admin', status: 'Active', lastSeen: '2 mins ago' },
  { id: 2, qq: '987654321', nickname: 'TestUser', role: 'User', status: 'Banned', lastSeen: '1 day ago' },
  // ... 更多数据
]);

const selectedUsers = ref();

const getStatusBadge = (status) => {
  switch(status) {
    case 'Active': return 'bg-green-100 text-green-700 border-green-200';
    case 'Banned': return 'bg-red-100 text-red-700 border-red-200';
    default: return 'bg-gray-100 text-gray-700 border-gray-200';
  }
};
</script>

<template>
  <div class="h-full flex flex-col">
    <!-- Toolbar -->
    <div class="flex justify-between items-center mb-4">
      <div class="flex items-center space-x-2">
        <h1 class="text-xl font-bold text-gray-800 mr-4">用户数据</h1>
        <!-- 视图切换 Tab -->
        <div class="flex bg-gray-100 rounded-lg p-1">
          <button class="px-3 py-1 bg-white shadow-sm rounded-md text-xs font-medium text-gray-800">Grid View</button>
          <button class="px-3 py-1 text-xs font-medium text-gray-500 hover:text-gray-700">Gallery</button>
        </div>
      </div>

      <div class="flex space-x-2">
        <button class="flex items-center px-3 py-1.5 text-sm font-medium text-gray-600 bg-white border border-gray-300 rounded-lg hover:bg-gray-50">
          <Filter class="w-4 h-4 mr-2" /> 筛选
        </button>
        <button class="flex items-center px-3 py-1.5 text-sm font-medium text-white bg-indigo-600 rounded-lg hover:bg-indigo-700 shadow-sm">
          <Plus class="w-4 h-4 mr-2" /> 新增记录
        </button>
      </div>
    </div>

    <!-- Data Grid Container (NocoDB Style) -->
    <div class="flex-1 bg-white border border-gray-200 rounded-lg shadow-sm overflow-hidden flex flex-col">
      <DataTable
        :value="users"
        v-model:selection="selectedUsers"
        dataKey="id"
        scrollable
        scrollHeight="flex"
        :pt="{
          table: { class: 'min-w-full' },
          thead: { class: 'bg-gray-50 border-b border-gray-200' },
          headerCell: { class: 'py-3 px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider text-left' },
          bodyRow: { class: 'hover:bg-gray-50 transition-colors border-b border-gray-100 last:border-0' },
          bodyCell: { class: 'py-2 px-4 text-sm text-gray-700' }
        }"
      >
        <Column selectionMode="multiple" headerStyle="width: 3rem"></Column>

        <Column field="qq" header="QQ ID" sortable>
          <template #body="slotProps">
            <span class="font-mono text-gray-600">{{ slotProps.data.qq }}</span>
          </template>
        </Column>

        <Column field="nickname" header="昵称" sortable>
           <template #body="slotProps">
            <div class="flex items-center">
              <div class="w-6 h-6 rounded-full bg-indigo-100 text-indigo-600 flex items-center justify-center text-xs mr-2">
                {{ slotProps.data.nickname[0] }}
              </div>
              {{ slotProps.data.nickname }}
            </div>
          </template>
        </Column>

        <Column field="role" header="角色">
           <template #body="slotProps">
             <span class="px-2 py-0.5 rounded text-xs font-medium bg-gray-100 text-gray-600 border border-gray-200">
               {{ slotProps.data.role }}
             </span>
           </template>
        </Column>

        <Column field="status" header="状态">
          <template #body="slotProps">
            <span :class="['px-2 py-0.5 rounded-full text-xs font-medium border', getStatusBadge(slotProps.data.status)]">
              {{ slotProps.data.status }}
            </span>
          </template>
        </Column>

        <Column field="lastSeen" header="最后活跃"></Column>

        <Column style="width: 3rem; text-align: center">
          <template #body>
            <button class="text-gray-400 hover:text-gray-600">
              <MoreHorizontal class="w-4 h-4" />
            </button>
          </template>
        </Column>
      </DataTable>
    </div>

    <!-- Pagination Footer -->
    <div class="h-10 border-t border-gray-200 bg-gray-50 flex items-center justify-between px-4">
      <span class="text-xs text-gray-500">显示 1 到 10 条，共 1284 条</span>
      <div class="flex space-x-1">
         <!-- 简单的翻页按钮 -->
         <button class="px-2 py-0.5 text-xs border rounded bg-white text-gray-600">Prev</button>
         <button class="px-2 py-0.5 text-xs border rounded bg-white text-gray-600">Next</button>
      </div>
    </div>
  </div>
</template>
