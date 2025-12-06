<script setup lang="ts">
// 组件属性，使用withDefaults一次性定义
const props = withDefaults(defineProps<{
  title?: string;
  description?: string;
  icon?: string;
  actionText?: string;
  onAction?: () => void;
}>(), {
  title: '暂无数据',
  description: '当前没有可用数据',
  icon: 'Database'
});
</script>

<template>
  <div class="flex flex-col items-center justify-center py-12 px-4 text-center">
    <!-- 图标 -->
    <div class="text-gray-400 mb-4">
      <slot name="icon">
        <!-- 默认图标占位 -->
        <div class="w-16 h-16 flex items-center justify-center rounded-full bg-gray-100">
          <span class="text-2xl">{{ props.icon }}</span>
        </div>
      </slot>
    </div>

    <!-- 标题 -->
    <h3 class="text-lg font-medium text-gray-900 mb-1">
      {{ props.title }}
    </h3>

    <!-- 描述 -->
    <p class="text-sm text-gray-500 mb-6 max-w-sm">
      {{ props.description }}
    </p>

    <!-- 操作按钮 -->
    <slot name="action">
      <button
        v-if="props.actionText && props.onAction"
        @click="props.onAction"
        class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
      >
        {{ props.actionText }}
      </button>
    </slot>
  </div>
</template>
