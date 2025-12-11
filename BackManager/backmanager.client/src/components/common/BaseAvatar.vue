<script setup lang="ts">import { computed } from 'vue';

// 组件属性，使用withDefaults一次性定义
const props = withDefaults(defineProps<{
  src?: string | null;
  username: string;
  size?: 'sm' | 'md' | 'lg';
  rounded?: boolean;
}>(), {
  size: 'md',
  rounded: false
});

// 根据尺寸计算样式
const avatarSize = computed(() => {
  switch (props.size) {
    case 'sm':
      return 'w-8 h-8';
    case 'lg':
      return 'w-12 h-12';
    case 'md':
    default:
      return 'w-10 h-10';
  }
});

// 计算头像边框半径
const avatarRadius = computed(() => {
  return props.rounded ? 'rounded-full' : 'rounded';
});

// 生成默认头像文字（用户名首字母）
const initials = computed(() => {
  return props.username
    .split(' ')
    .map(word => word[0])
    .join('')
    .toUpperCase()
    .slice(0, 2);
});

// 生成随机背景色
const avatarBgColor = computed(() => {
  // 根据用户名生成一致的哈希值
  const hash = props.username.split('').reduce((acc, char) => {
    return char.charCodeAt(0) + ((acc << 5) - acc);
  }, 0);

  // 使用哈希值生成RGB颜色
  const color = Math.abs(hash) % 16777215;
  const r = Math.floor(color / 65536);
  const g = Math.floor((color % 65536) / 256);
  const b = color % 256;

  // 返回RGB颜色字符串
  return `rgb(${r}, ${g}, ${b})`;
});</script>

<template>
  <div class="flex items-center justify-center text-white font-medium"
       :class="[avatarSize, avatarRadius]"
       :style="{
      backgroundColor: props.src ? 'transparent' : avatarBgColor,
      overflow: 'hidden'
    }">
    <img v-if="props.src"
         :src="props.src"
         :alt="props.username"
         class="w-full h-full object-cover" />
    <span v-else>{{ initials }}</span>
  </div>
</template>
