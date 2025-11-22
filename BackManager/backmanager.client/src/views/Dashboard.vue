<script setup lang="ts">
defineOptions({
  name: 'DashboardView'
});
import { Users, MessageSquare, ShieldAlert, Cpu } from 'lucide-vue-next';

const stats = [
  { title: '总用户数', value: '1,284', change: '+12%', icon: Users, color: 'bg-blue-500' },
  { title: '今日消息', value: '45.2k', change: '+5.4%', icon: MessageSquare, color: 'bg-green-500' },
  { title: '待审核', value: '7', change: '-2', icon: ShieldAlert, color: 'bg-orange-500' },
  { title: '系统负载', value: '24%', change: '正常', icon: Cpu, color: 'bg-purple-500' },
];
</script>

<template>
  <div class="space-y-6">
    <h1 class="text-2xl font-bold text-gray-800">仪表盘</h1>

    <!-- Stats Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div v-for="stat in stats" :key="stat.title" class="bg-white rounded-xl border border-gray-200 p-5 shadow-sm hover:shadow-md transition-shadow">
        <div class="flex items-center justify-between mb-4">
          <div :class="[stat.color, 'p-2 rounded-lg text-white bg-opacity-90']">
            <component :is="stat.icon" class="w-6 h-6" />
          </div>
          <span class="text-sm font-medium"
            :class="stat.change.startsWith('+') ? 'text-green-600' : 'text-gray-500'">
            {{ stat.change }}
          </span>
        </div>
        <div>
          <p class="text-sm text-gray-500 font-medium">{{ stat.title }}</p>
          <p class="text-2xl font-bold text-gray-900 mt-1">{{ stat.value }}</p>
        </div>
      </div>
    </div>

    <!-- Recent Activity Section (Placeholder) -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <div class="lg:col-span-2 bg-white rounded-xl border border-gray-200 p-6 shadow-sm">
        <h3 class="text-lg font-semibold text-gray-800 mb-4">消息流量趋势</h3>
        <div class="h-64 bg-gray-50 rounded-lg flex items-center justify-center text-gray-400 border border-dashed border-gray-300">
          Chart.js / ECharts 图表占位符
        </div>
      </div>

      <div class="bg-white rounded-xl border border-gray-200 p-6 shadow-sm">
        <h3 class="text-lg font-semibold text-gray-800 mb-4">快速控制</h3>
        <div class="space-y-3">
          <button class="w-full py-2 px-4 bg-indigo-600 hover:bg-indigo-700 text-white rounded-lg text-sm font-medium transition-colors">
            重启机器人核心
          </button>
          <button class="w-full py-2 px-4 bg-white border border-gray-300 hover:bg-gray-50 text-gray-700 rounded-lg text-sm font-medium transition-colors">
            清理缓存
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
