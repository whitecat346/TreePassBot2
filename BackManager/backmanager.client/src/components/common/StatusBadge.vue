<script setup lang="ts">
import { computed } from 'vue';

// 状态类型定义
export type StatusType = 'success' | 'warning' | 'error' | 'info' | 'pending';

// 组件属性
const props = defineProps<{
  status: StatusType;
  text: string;
}>();

// 根据状态计算样式类
const badgeClass = computed(() => {
  const baseClass = 'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium';

  switch (props.status) {
    case 'success':
      return `${baseClass} bg-green-100 text-green-800`;
    case 'warning':
      return `${baseClass} bg-yellow-100 text-yellow-800`;
    case 'error':
      return `${baseClass} bg-red-100 text-red-800`;
    case 'info':
      return `${baseClass} bg-blue-100 text-blue-800`;
    case 'pending':
      return `${baseClass} bg-gray-100 text-gray-800`;
    default:
      return baseClass;
  }
});
</script>

<template>
  <span :class="badgeClass">
    {{ text }}
  </span>
</template>
