<script setup lang="ts">
defineOptions({
  name: 'SidebarComponent'
});

import { useRoute, RouterLink } from 'vue-router';
import { LayoutDashboard, Users, FileText, Plug, Database } from 'lucide-vue-next';

const route = useRoute();

const menuItems = [
  { name: '仪表盘', path: '/', icon: LayoutDashboard },
  { name: '用户管理', path: '/users', icon: Users },
  { name: '审核记录', path: '/audits', icon: FileText },
  { name: '插件中心', path: '/plugins', icon: Plug },
  { name: '消息日志', path: '/logs', icon: Database },
];

const isActive = (path: string) => route.path === path;
</script>

<template>
  <aside class="w-64 bg-white border-r border-gray-200 flex flex-col z-10 h-full">
    <!-- Logo -->
    <div class="h-16 flex items-center px-6 border-b border-gray-100 shrink-0">
      <div class="w-8 h-8 bg-indigo-600 rounded-lg flex items-center justify-center text-white font-bold mr-3 shadow-sm">
        TP
      </div>
      <span class="font-bold text-lg text-gray-800 tracking-tight">TreePassBot</span>
    </div>

    <!-- Navigation -->
    <nav class="flex-1 px-3 py-4 space-y-1 overflow-y-auto">
      <RouterLink
        v-for="item in menuItems"
        :key="item.path"
        :to="item.path"
        class="group flex items-center px-3 py-2.5 text-sm font-medium rounded-lg transition-all duration-200"
        :class="[
          isActive(item.path)
            ? 'bg-indigo-50 text-indigo-600 shadow-sm'
            : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
        ]"
      >
        <component
          :is="item.icon"
          class="mr-3 h-5 w-5 flex-shrink-0 transition-colors"
          :class="isActive(item.path) ? 'text-indigo-600' : 'text-gray-400 group-hover:text-gray-500'"
        />
        {{ item.name }}
      </RouterLink>
    </nav>

    <!-- Footer -->
    <div class="p-4 border-t border-gray-100 shrink-0">
      <div class="flex items-center bg-gray-50 rounded-lg p-3">
        <div class="relative">
          <div class="w-2 h-2 rounded-full bg-green-500"></div>
          <div class="w-2 h-2 rounded-full bg-green-500 absolute top-0 left-0 animate-ping"></div>
        </div>
        <span class="ml-2 text-xs font-medium text-gray-500">系统状态: 正常运行</span>
      </div>
    </div>
  </aside>
</template>
