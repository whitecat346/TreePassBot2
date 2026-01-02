<script setup lang="ts">defineOptions({
  name: 'HeaderComponent'
});
import { ref } from 'vue';
import { Search, ChevronDown, Menu } from 'lucide-vue-next';

// 组件属性
const props = defineProps<{
  isSidebarCollapsed: boolean;
}>();

// 事件定义
const emit = defineEmits<{
  (e: 'toggle-sidebar'): void;
}>();

const searchQuery = ref('');

// 切换侧边栏显示状态
const handleToggleSidebar = () => {
  emit('toggle-sidebar');
};</script>

<template>
  <header class="bg-white h-16 border-b border-gray-200 flex justify-between items-center px-4 md:px-6 shadow-sm z-20 shrink-0">
    <!-- 左侧：菜单按钮和面包屑 -->
    <div class="flex items-center space-x-3">
      <!-- 菜单按钮（移动端和桌面端折叠时显示） -->
      <button
        class="p-2 text-gray-400 hover:text-gray-600 rounded-full hover:bg-gray-100 transition-colors"
        @click="handleToggleSidebar"
      >
        <Menu class="w-5 h-5" />
      </button>

      <!-- Breadcrumb Placeholder -->
      <div class="flex items-center">
        <h2 class="text-base md:text-lg font-semibold text-gray-800">控制台</h2>
      </div>
    </div>

    <!-- 中间：搜索框 -->
    <div class="hidden md:flex-1 md:max-w-lg md:mx-12">
      <div class="relative group">
        <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
          <Search class="h-4 w-4 text-gray-400 group-focus-within:text-indigo-500 transition-colors" />
        </div>
        <input v-model="searchQuery"
               type="text"
               class="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-lg leading-5 bg-gray-50 text-gray-900 placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 sm:text-sm transition-all"
               placeholder="全局搜索 (Ctrl+K)" />
      </div>
    </div>

    <!-- 右侧：操作按钮 -->
    <div class="flex items-center space-x-3">
      <!-- 移动端搜索按钮 -->
      <button class="p-2 text-gray-400 hover:text-gray-600 rounded-full hover:bg-gray-100 transition-colors md:hidden">
        <Search class="w-5 h-5" />
      </button>

      <!-- 用户信息 -->
      <div class="flex items-center cursor-pointer hover:opacity-80">
        <div class="w-8 h-8 rounded-full bg-indigo-100 flex items-center justify-center text-indigo-700 font-bold text-xs">
          AD
        </div>
        <span class="ml-2 text-sm font-medium text-gray-700 hidden sm:block">Administrator</span>
        <ChevronDown class="ml-2 h-4 w-4 text-gray-400 hidden sm:block" />
      </div>
    </div>
  </header>
</template>
