<script setup lang="ts">defineOptions({
  name: 'HeaderComponent'
});
import { ref } from 'vue';
import { Search, Bell, ChevronDown } from 'lucide-vue-next';

const searchQuery = ref('');</script>

<template>
  <header class="bg-white h-16 border-b border-gray-200 flex justify-between items-center px-6 shadow-sm z-20 shrink-0">
    <!-- Breadcrumb Placeholder -->
    <div class="flex items-center">
      <h2 class="text-lg font-semibold text-gray-800">控制台</h2>
    </div>

    <!-- Search -->
    <div class="flex-1 max-w-lg mx-12 hidden md:block">
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

    <!-- Actions -->
    <div class="flex items-center space-x-3">
      <button class="p-2 text-gray-400 hover:text-gray-600 rounded-full hover:bg-gray-100 transition-colors">
        <Bell class="w-5 h-5" />
      </button>
      <div class="h-6 w-px bg-gray-200 mx-1"></div>
      <div class="flex items-center cursor-pointer hover:opacity-80">
        <div class="w-8 h-8 rounded-full bg-indigo-100 flex items-center justify-center text-indigo-700 font-bold text-xs">
          AD
        </div>
        <span class="ml-2 text-sm font-medium text-gray-700 hidden md:block">Administrator</span>
        <ChevronDown class="ml-2 h-4 w-4 text-gray-400" />
      </div>
    </div>
  </header>
</template>
