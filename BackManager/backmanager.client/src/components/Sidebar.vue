<script setup lang="ts">defineOptions({
  name: 'SidebarComponent'
});

import { useRoute, RouterLink } from 'vue-router';
import { LayoutDashboard, Users, FileText, Plug, Database, Menu, X } from 'lucide-vue-next';

// 组件属性
const props = defineProps<{
  isCollapsed: boolean;
  isMobile: boolean;
  isVisible: boolean;
}>();

// 事件定义
const emit = defineEmits<{
  (e: 'toggle'): void;
  (e: 'close'): void;
}>();

const route = useRoute();

const menuItems = [
  { name: '仪表盘', path: '/', icon: LayoutDashboard },
  { name: '用户管理', path: '/users', icon: Users },
  { name: '审核记录', path: '/audits', icon: FileText },
  { name: '插件中心', path: '/plugins', icon: Plug },
  { name: '消息日志', path: '/logs', icon: Database },
];

const isActive = (path: string) => route.path === path;

// 关闭侧边栏（移动端）
const handleCloseSidebar = () => {
  emit('close');
};</script>

<template>
  <!-- 移动端遮罩层 -->
  <div
    v-if="props.isMobile && props.isVisible"
    class="fixed inset-0 bg-black bg-opacity-50 z-10 md:hidden"
    @click="handleCloseSidebar"
  ></div>

  <!-- 侧边栏 -->
  <aside
    class="bg-white border-r border-gray-200 flex flex-col z-20 h-full transition-all duration-300 ease-in-out md:relative fixed md:static"
    :class="{'translate-x-0': isVisible, '-translate-x-full': !isVisible, 'w-64': !isCollapsed, 'w-20': isCollapsed}"
  >
    <!-- Logo和菜单按钮 -->
    <div class="h-16 flex items-center justify-between px-4 md:px-6 border-b border-gray-100 shrink-0">
      <!-- 移动端菜单按钮 -->
      <button
        v-if="isMobile"
        class="p-2 text-gray-500 hover:text-gray-700 rounded-lg hover:bg-gray-100 transition-colors md:hidden"
        @click="handleCloseSidebar"
      >
        <X class="w-5 h-5" />
      </button>

      <!-- Logo -->
      <div class="flex items-center">
        <div class="w-8 h-8 bg-indigo-600 rounded-lg flex items-center justify-center text-white font-bold shadow-sm">
          TP
        </div>
        <span v-if="!isCollapsed" class="font-bold text-lg text-gray-800 tracking-tight ml-3">TreePassBot</span>
      </div>

      <!-- 桌面端折叠按钮 -->
      <button
        v-if="!isMobile"
        class="p-2 text-gray-500 hover:text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
        @click="$emit('toggle')"
      >
        <Menu class="w-5 h-5" />
      </button>
    </div>

    <!-- Navigation -->
    <nav class="flex-1 px-3 py-4 space-y-1 overflow-y-auto">
      <RouterLink
        v-for="item in menuItems"
        :key="item.path"
        :to="item.path"
        class="group flex items-center px-3 py-2.5 text-sm font-medium rounded-lg transition-all duration-200 hover:bg-gray-50"
        :class="[
          isActive(item.path)
            ? 'bg-indigo-50 text-indigo-600 shadow-sm'
            : 'text-gray-600 hover:text-gray-900'
        ]"
        @click="isMobile && handleCloseSidebar()"
      >
        <component
          :is="item.icon"
          class="mr-3 h-5 w-5 flex-shrink-0 transition-colors"
          :class="isActive(item.path) ? 'text-indigo-600' : 'text-gray-400 group-hover:text-gray-500'"
        />
        <span v-if="!isCollapsed" class="transition-all duration-300">
          {{ item.name }}
        </span>
      </RouterLink>
    </nav>

    <!-- Footer -->
    <div v-if="!isCollapsed" class="p-4 border-t border-gray-100 shrink-0">
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

<style scoped>
/* 侧边栏过渡动画 */
aside {
  transition: transform 0.3s ease-in-out, width 0.3s ease-in-out;
}

/* 移动端侧边栏遮罩层过渡 */
.bg-black {
  transition: opacity 0.3s ease-in-out;
}
</style>
