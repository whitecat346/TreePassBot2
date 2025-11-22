<script setup>
import { LayoutDashboard, Users, FileText, Plug, Settings, Database } from 'lucide-vue-next';
import { useRoute } from 'vue-router';

const route = useRoute();

const menuItems = [
  { name: '仪表盘', path: '/', icon: LayoutDashboard },
  { name: '用户管理', path: '/users', icon: Users },
  { name: '审核记录', path: '/audits', icon: FileText },
  { name: '插件中心', path: '/plugins', icon: Plug },
  { name: '消息日志', path: '/logs', icon: Database },
];
</script>

<template>
  <aside class="w-64 bg-white border-r border-gray-200 flex flex-col shadow-sm z-10">
    <!-- Logo -->
    <div class="h-14 flex items-center px-6 border-b border-gray-100">
      <div class="w-8 h-8 bg-indigo-600 rounded-lg flex items-center justify-center text-white font-bold mr-3">
        TP
      </div>
      <span class="font-bold text-lg text-gray-800 tracking-tight">TreePassBot</span>
    </div>

    <!-- Navigation -->
    <nav class="flex-1 px-3 py-4 space-y-1">
      <router-link
        v-for="item in menuItems"
        :key="item.path"
        :to="item.path"
        class="group flex items-center px-3 py-2 text-sm font-medium rounded-md transition-colors"
        :class="[
          route.path === item.path
            ? 'bg-indigo-50 text-indigo-600'
            : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
        ]"
      >
        <component
          :is="item.icon"
          class="mr-3 h-5 w-5 flex-shrink-0"
          :class="route.path === item.path ? 'text-indigo-600' : 'text-gray-400 group-hover:text-gray-500'"
        />
        {{ item.name }}
      </router-link>
    </nav>

    <!-- Footer -->
    <div class="p-4 border-t border-gray-100">
      <div class="flex items-center">
        <div class="w-2 h-2 rounded-full bg-green-500 mr-2"></div>
        <span class="text-xs text-gray-500">系统状态: 在线</span>
      </div>
    </div>
  </aside>
</template>
