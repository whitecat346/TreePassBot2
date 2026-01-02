<script setup lang="ts">import Sidebar from './components/Sidebar.vue';
import Header from './components/Header.vue';
import { ref, onMounted, watch } from 'vue';

// 侧边栏折叠状态
const isSidebarCollapsed = ref(false);
// 是否是移动端
const isMobile = ref(false);
// 侧边栏显示状态（移动端）
const isSidebarVisible = ref(false);

// 监听窗口大小变化
const handleResize = () => {
  isMobile.value = window.innerWidth < 768;
  // 在桌面端，默认展开侧边栏
  if (!isMobile.value) {
    isSidebarVisible.value = true;
  } else {
    // 在移动端，默认隐藏侧边栏
    isSidebarVisible.value = false;
  }
};

// 切换侧边栏显示状态
const toggleSidebar = () => {
  if (isMobile.value) {
    isSidebarVisible.value = !isSidebarVisible.value;
  } else {
    isSidebarCollapsed.value = !isSidebarCollapsed.value;
  }
};

// 关闭侧边栏
const closeSidebar = () => {
  if (isMobile.value) {
    isSidebarVisible.value = false;
  }
};

onMounted(() => {
  handleResize();
  window.addEventListener('resize', handleResize);
});

// 导出给子组件使用
defineExpose({
  isSidebarCollapsed,
  isMobile,
  isSidebarVisible,
  toggleSidebar,
  closeSidebar
});</script>

<template>
  <div class="flex h-screen bg-gray-50 font-sans text-gray-900 overflow-hidden">
    <!-- 侧边栏 -->
    <Sidebar 
      :is-collapsed="isSidebarCollapsed" 
      :is-mobile="isMobile" 
      :is-visible="isSidebarVisible" 
      @toggle="toggleSidebar" 
      @close="closeSidebar" 
    />
    
    <!-- 主内容区 -->
    <div class="flex-1 flex flex-col min-w-0">
      <Header @toggle-sidebar="toggleSidebar" :is-sidebar-collapsed="isSidebarCollapsed" />
      <main class="flex-1 overflow-auto p-4 md:p-6 relative" @click="closeSidebar">
        <router-view />
      </main>
    </div>
  </div>
</template>

<style>
/* 移除Transition相关样式 */
</style>
