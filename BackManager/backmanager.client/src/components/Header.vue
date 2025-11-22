<!-- src/components/Header.vue -->
<script setup>
import { ref } from 'vue';
import { Search, Bell, HelpCircle, ChevronDown } from 'lucide-vue-next';

const searchQuery = ref('');
const hasNotifications = ref(true); // 模拟有未读通知
</script>

<template>
  <header class="bg-white h-16 border-b border-gray-200 flex justify-between items-center px-6 shadow-sm z-20">
    <!-- 左侧：面包屑 / 标题 -->
    <div class="flex items-center">
      <h2 class="text-lg font-semibold text-gray-800 tracking-tight">
        <!-- 这里可以根据路由动态显示，暂时写死 -->
        控制台
      </h2>
      <span class="mx-3 text-gray-300">/</span>
      <span class="text-sm text-gray-500">概览</span>
    </div>

    <!-- 中间：搜索框 (类似 NocoDB 的顶部搜索) -->
    <div class="flex-1 max-w-lg mx-12 hidden md:block">
      <div class="relative group">
        <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
          <Search class="h-4 w-4 text-gray-400 group-focus-within:text-indigo-500 transition-colors" />
        </div>
        <input
          v-model="searchQuery"
          type="text"
          class="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-lg leading-5 bg-gray-50 text-gray-900 placeholder-gray-500 focus:outline-none focus:bg-white focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm transition-all"
          placeholder="搜索用户、日志或插件..."
        />
      </div>
    </div>

    <!-- 右侧：功能区 -->
    <div class="flex items-center space-x-4">
      <!-- 帮助文档 -->
      <button class="p-2 text-gray-400 hover:text-gray-600 rounded-full hover:bg-gray-100 transition-colors">
        <HelpCircle class="w-5 h-5" />
      </button>

      <!-- 通知 -->
      <button class="p-2 text-gray-400 hover:text-gray-600 rounded-full hover:bg-gray-100 transition-colors relative">
        <Bell class="w-5 h-5" />
        <span v-if="hasNotifications" class="absolute top-2 right-2 block h-2 w-2 rounded-full bg-red-500 ring-2 ring-white"></span>
      </button>

      <!-- 分割线 -->
      <div class="h-6 w-px bg-gray-200 mx-2"></div>

      <!-- 用户头像下拉菜单 -->
      <div class="flex items-center cursor-pointer hover:opacity-80 transition-opacity">
        <img
          class="h-8 w-8 rounded-full bg-gray-300 object-cover border border-gray-200"
          src="https://api.dicebear.com/7.x/avataaars/svg?seed=Felix"
          alt="Admin Avatar"
        />
        <div class="ml-3 hidden md:block text-left">
          <p class="text-sm font-medium text-gray-700 group-hover:text-gray-900">Admin</p>
          <p class="text-xs font-medium text-gray-500">超级管理员</p>
        </div>
        <ChevronDown class="ml-2 h-4 w-4 text-gray-400" />
      </div>
    </div>
  </header>
</template>
