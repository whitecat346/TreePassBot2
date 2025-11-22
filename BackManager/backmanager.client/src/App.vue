<script setup>
import Sidebar from './components/Sidebar.vue';
import Header from './components/Header.vue';
</script>

<template>
  <div class="flex h-screen bg-gray-50 font-sans text-gray-900">
    <!-- 左侧导航栏 -->
    <Sidebar />

    <div class="flex-1 flex flex-col min-w-0 overflow-hidden">
      <!-- 顶部栏 -->
      <Header />

      <!-- 主内容区域 -->
      <main class="flex-1 overflow-auto p-6">
        <router-view v-slot="{ Component }">
          <transition name="fade" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </main>
    </div>
  </div>
</template>

<style>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.15s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
