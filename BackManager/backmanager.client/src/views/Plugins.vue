<script setup lang="ts">
defineOptions({
  name: 'PluginsView'
});
import { ref, onMounted } from 'vue';
import { Play, Square, Settings, AlertTriangle, Box } from 'lucide-vue-next';
import type { Plugin } from '@/types'; // 使用前面定义的 Plugin 接口
import apiClient from '@/api/client';

const plugins = ref<Plugin[]>([]);
const loading = ref(true);

// 加载插件列表
const fetchPlugins = async () => {
  loading.value = true;
  try {
    // const res = await apiClient.get<Plugin[]>('/plugins');
    // plugins.value = res.data;

    // 模拟数据
    await new Promise(r => setTimeout(r, 500));
    plugins.value = [
      { id: 'com.tp.signin', name: '每日签到', version: '1.0.0', status: 'Running', author: 'TreePass', description: '群组签到积分系统' },
      { id: 'com.tp.audit', name: '入群审核', version: '2.1.0', status: 'Running', author: 'TreePass', description: '核心审核模块' },
      { id: 'com.tp.game', name: '文字RPG', version: '0.5.1', status: 'Crashed', author: 'Community', description: '测试版游戏插件', errorCount: 5 },
      { id: 'com.tp.cleaner', name: '消息清理', version: '1.0.0', status: 'Stopped', author: 'Admin', description: '定时清理过期消息' },
    ];
  } finally {
    loading.value = false;
  }
};

onMounted(fetchPlugins);

// 控制逻辑
const togglePlugin = async (plugin: Plugin) => {
  if (plugin.status === 'Running') {
    // Stop / Unload
    // await apiClient.post(`/plugins/${plugin.id}/unload`);
    plugin.status = 'Stopped';
  } else {
    // Start / Load
    // await apiClient.post(`/plugins/${plugin.id}/load`);
    plugin.status = 'Running';
    plugin.errorCount = 0;
  }
};

const getStatusBadge = (status: string) => {
  switch (status) {
    case 'Running': return 'bg-green-50 text-green-700 border-green-200';
    case 'Crashed': return 'bg-red-50 text-red-700 border-red-200';
    case 'Stopped': return 'bg-gray-100 text-gray-600 border-gray-200';
    default: return 'bg-gray-50 text-gray-500';
  }
};
</script>

<template>
  <div>
    <div class="mb-8">
      <h1 class="text-2xl font-bold text-gray-900 flex items-center">
        <Box class="w-7 h-7 mr-3 text-indigo-600" />
        插件中心
      </h1>
      <p class="text-gray-500 text-sm mt-1 ml-10">管理 AssemblyLoadContext 隔离环境中的功能模块。</p>
    </div>

    <div v-if="loading" class="text-center py-20 text-gray-400">加载中...</div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
      <div
        v-for="plugin in plugins"
        :key="plugin.id"
        class="bg-white rounded-xl border border-gray-200 p-5 shadow-sm hover:shadow-md transition-all duration-300 relative group flex flex-col h-full"
      >
        <!-- 顶部状态条 -->
        <div class="absolute top-0 left-0 w-1.5 h-full rounded-l-xl transition-colors"
          :class="plugin.status === 'Running' ? 'bg-green-500' : plugin.status === 'Crashed' ? 'bg-red-500' : 'bg-gray-300'">
        </div>

        <div class="flex justify-between items-start mb-4 pl-3">
          <div class="flex items-center">
            <div class="w-10 h-10 rounded-lg bg-indigo-50 text-indigo-600 flex items-center justify-center mr-3 text-lg font-bold border border-indigo-100">
              {{ plugin.name[0] }}
            </div>
            <div>
              <h3 class="font-bold text-gray-900 text-base leading-tight">{{ plugin.name }}</h3>
              <p class="text-xs text-gray-400 font-mono mt-0.5">{{ plugin.version }}</p>
            </div>
          </div>

          <span :class="['px-2.5 py-1 rounded-md text-xs font-bold border', getStatusBadge(plugin.status)]">
            {{ plugin.status }}
          </span>
        </div>

        <div class="pl-3 mb-4 flex-1">
          <p class="text-sm text-gray-600 line-clamp-2">{{ plugin.description }}</p>
          <p class="text-xs text-gray-400 mt-2">ID: {{ plugin.id }}</p>

          <!-- 错误提示 -->
          <div v-if="plugin.status === 'Crashed'" class="mt-3 flex items-center text-xs text-red-600 bg-red-50 p-2 rounded border border-red-100">
            <AlertTriangle class="w-3 h-3 mr-1.5" />
            检测到 {{ plugin.errorCount }} 次异常，已自动熔断
          </div>
        </div>

        <div class="flex items-center justify-between pt-4 border-t border-gray-100 pl-3 mt-auto">
          <button class="text-gray-400 hover:text-indigo-600 transition-colors p-1.5 hover:bg-gray-50 rounded">
            <Settings class="w-4 h-4" />
          </button>

          <button
            @click="togglePlugin(plugin)"
            class="flex items-center px-3 py-1.5 text-xs font-medium rounded-lg transition-colors border shadow-sm"
            :class="plugin.status === 'Running'
              ? 'text-red-600 bg-white border-red-200 hover:bg-red-50'
              : 'text-green-600 bg-white border-green-200 hover:bg-green-50'"
          >
            <component :is="plugin.status === 'Running' ? Square : Play" class="w-3 h-3 mr-1.5 fill-current" />
            {{ plugin.status === 'Running' ? '停止' : '启动' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
