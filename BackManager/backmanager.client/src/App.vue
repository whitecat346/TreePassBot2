<script setup lang="ts">import Sidebar from './components/Sidebar.vue';
import Header from './components/Header.vue';</script>

<template>
  <div class="flex h-screen bg-gray-50 font-sans text-gray-900 overflow-hidden">
    <Sidebar />
    <div class="flex-1 flex flex-col min-w-0">
      <Header />
      <main class="flex-1 overflow-auto p-6 relative">
        <router-view />
      </main>
    </div>
  </div>
</template>

<style>
/* 移除Transition相关样式 */
</style>
