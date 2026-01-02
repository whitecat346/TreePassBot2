<script setup lang="ts">import { ref, computed, onMounted, onUnmounted } from 'vue';
import EmptyState from './EmptyState.vue';

// 泛型数据类型
type TableData = Record<string, unknown>;

// 列配置类型
export interface TableColumn {
  prop: string;
  label: string;
  width?: number | string;
  sortable?: boolean;
  align?: 'left' | 'center' | 'right';
  formatter?: (row: TableData, column: TableColumn) => unknown;
  slotName?: string;
}

// 组件属性，使用withDefaults一次性定义
const props = withDefaults(defineProps<{
  columns: TableColumn[];
  data: TableData[];
  loading?: boolean;
  bordered?: boolean;
  showPagination?: boolean;
  showSearch?: boolean;
  searchPlaceholder?: string;
  pageSizeOptions?: number[];
  initialPageSize?: number;
  total?: number;
  title?: string;
}>(), {
  loading: false,
  bordered: false,
  showPagination: true,
  showSearch: false,
  searchPlaceholder: '搜索...',
  pageSizeOptions: () => [10, 20, 50, 100],
  initialPageSize: 20,
  total: 0
});

// 事件定义
const emit = defineEmits<{
  (e: 'page-change', page: number, pageSize: number): void;
  (e: 'sort-change', prop: string, order: 'asc' | 'desc'): void;
  (e: 'search', keyword: string): void;
}>();

// 响应式数据
const currentPage = ref(1);
const pageSize = ref(props.initialPageSize);
const searchKeyword = ref('');
const sortProp = ref<string>('');
const sortOrder = ref<'asc' | 'desc'>('asc');
// 响应式变量，用于判断是否为移动端
const isMobile = ref(false);

// 计算总页数
const totalPages = computed(() => {
  return Math.ceil(props.total / pageSize.value);
});

// 处理窗口大小变化
const handleResize = () => {
  isMobile.value = window.innerWidth < 768;
};

// 处理页码变化
const handlePageChange = (page: number) => {
  currentPage.value = page;
  emit('page-change', page, pageSize.value);
};

// 处理每页条数变化
const handlePageSizeChange = (size: number) => {
  pageSize.value = size;
  currentPage.value = 1;
  emit('page-change', 1, size);
};

// 处理排序变化
const handleSortChange = (prop: string, order: 'asc' | 'desc') => {
  sortProp.value = prop;
  sortOrder.value = order;
  emit('sort-change', prop, order);
};

// 处理搜索
const handleSearch = () => {
  currentPage.value = 1;
  emit('search', searchKeyword.value);
};

// 处理搜索框回车键
const handleSearchKeydown = (event: KeyboardEvent) => {
  if (event.key === 'Enter') {
    handleSearch();
  }
};

// 组件挂载时添加窗口大小监听
onMounted(() => {
  handleResize();
  window.addEventListener('resize', handleResize);
});

// 组件卸载时移除窗口大小监听
onUnmounted(() => {
  window.removeEventListener('resize', handleResize);
});</script>

<template>
  <div class="base-table-container w-full">
    <!-- 标题和搜索 -->
    <div v-if="props.title || props.showSearch" class="flex flex-col sm:flex-row items-start sm:items-center justify-between mb-4 gap-3">
      <h3 v-if="props.title" class="text-base sm:text-lg font-medium text-gray-900">{{ props.title }}</h3>

      <div v-if="props.showSearch" class="w-full sm:w-auto flex items-center w-full">
        <input v-model="searchKeyword"
               @keydown="handleSearchKeydown"
               placeholder="{{ props.searchPlaceholder }}"
               class="pl-10 pr-4 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm w-full"
               :class="{ 'text-xs': isMobile }" />
        <button @click="handleSearch"
                class="ml-2 inline-flex items-center px-3 py-2 border border-transparent text-sm leading-4 font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 whitespace-nowrap">
          搜索
        </button>
      </div>
    </div>

    <!-- 表格 -->
    <div class="overflow-x-auto rounded-lg border border-gray-200 bg-white shadow-sm">
      <table class="min-w-full divide-y divide-gray-200 text-sm sm:text-base"
             :class="{ 'border border-gray-200': props.bordered }">
        <!-- 表头 -->
        <thead class="bg-gray-50">
          <tr>
            <th v-for="column in props.columns"
                :key="column.prop"
                :width="column.width"
                :class="[
                'px-4 sm:px-6 py-3 text-left text-xs sm:text-sm font-medium text-gray-500 uppercase tracking-wider whitespace-nowrap',
                column.align ? `text-${column.align}` : 'text-left'
              ]">
              <div class="flex items-center">
                {{ column.label }}

                <!-- 排序图标 -->
                <span v-if="column.sortable" class="ml-1 cursor-pointer" @click="handleSortChange(column.prop, sortOrder === 'asc' ? 'desc' : 'asc')">
                  <svg v-if="sortProp === column.prop"
                       :class="sortOrder === 'asc' ? 'rotate-180' : ''"
                       class="w-3 h-3 sm:w-4 sm:h-4 text-gray-400"
                       fill="none"
                       stroke="currentColor"
                       viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 15l7-7 7 7" />
                  </svg>
                  <svg v-else
                       class="w-3 h-3 sm:w-4 sm:h-4 text-gray-400"
                       fill="none"
                       stroke="currentColor"
                       viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 16V4m0 0L3 8m4-4l4 4m6 0v12m0 0l4-4m-4 4l-4-4" />
                  </svg>
                </span>
              </div>
            </th>
          </tr>
        </thead>

        <!-- 表体 -->
        <tbody class="bg-white divide-y divide-gray-200">
          <template v-if="props.loading">
            <!-- 加载状态 -->
            <tr>
              <td :colspan="props.columns.length" class="px-4 sm:px-6 py-8 text-center">
                <div class="flex items-center justify-center">
                  <div class="animate-spin rounded-full h-6 w-6 sm:h-8 sm:w-8 border-b-2 border-indigo-600"></div>
                  <span class="ml-2 text-sm sm:text-base text-gray-500">加载中...</span>
                </div>
              </td>
            </tr>
          </template>

          <template v-else-if="props.data && props.data.length > 0">
            <!-- 数据行 -->
            <tr v-for="(row, index) in props.data" :key="index" class="hover:bg-gray-50 transition-colors">
              <td v-for="column in props.columns"
                  :key="column.prop"
                  :class="[
                  'px-4 sm:px-6 py-3 sm:py-4 whitespace-nowrap text-sm text-gray-900',
                  column.align ? `text-${column.align}` : 'text-left'
                ]">
                <!-- 使用插槽渲染 -->
                <template v-if="column.slotName">
                  <slot :name="column.slotName" :row="row" :column="column" :index="index"></slot>
                </template>
                <!-- 使用格式化函数渲染 -->
                <template v-else-if="column.formatter">
                  {{ column.formatter(row, column) }}
                </template>
                <!-- 默认渲染 -->
                <template v-else>
                  {{ row[column.prop] }}
                </template>
              </td>
            </tr>
          </template>

          <template v-else>
            <!-- 空状态 -->
            <tr>
              <td :colspan="props.columns.length" class="px-4 sm:px-6 py-10">
                <EmptyState />
              </td>
            </tr>
          </template>
        </tbody>
      </table>
    </div>

    <!-- 分页 -->
    <div v-if="props.showPagination && props.total > 0" class="flex flex-col sm:flex-row items-start sm:items-center justify-between mt-4 gap-3">
      <div class="text-xs sm:text-sm text-gray-500 whitespace-nowrap">
        显示 {{ ((currentPage - 1) * pageSize) + 1 }} 到 {{ Math.min(currentPage * pageSize, props.total) }} 条，共 {{ props.total }} 条
      </div>

      <div class="flex items-center space-x-2 flex-wrap">
        <!-- 页码选择器 -->
        <select v-model="pageSize"
                @change="handlePageSizeChange(pageSize)"
                class="text-xs sm:text-sm border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 h-8 sm:h-9 px-2">
          <option v-for="option in props.pageSizeOptions" :key="option" :value="option">
            {{ option }} 条/页
          </option>
        </select>

        <!-- 分页按钮 -->
        <div class="flex items-center space-x-1">
          <button @click="handlePageChange(1)"
                  :disabled="currentPage === 1"
                  class="px-2 sm:px-3 py-1.5 sm:py-2 border border-gray-300 rounded-md text-xs sm:text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed whitespace-nowrap">
            首页
          </button>

          <button @click="handlePageChange(currentPage - 1)"
                  :disabled="currentPage === 1"
                  class="px-2 sm:px-3 py-1.5 sm:py-2 border border-gray-300 rounded-md text-xs sm:text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed whitespace-nowrap">
            上一页
          </button>

          <span class="px-2 sm:px-3 py-1.5 sm:py-2 border border-gray-300 rounded-md text-xs sm:text-sm font-medium text-gray-700 bg-white whitespace-nowrap">
            {{ currentPage }} / {{ totalPages }}
          </span>

          <button @click="handlePageChange(currentPage + 1)"
                  :disabled="currentPage === totalPages"
                  class="px-2 sm:px-3 py-1.5 sm:py-2 border border-gray-300 rounded-md text-xs sm:text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed whitespace-nowrap">
            下一页
          </button>

          <button @click="handlePageChange(totalPages)"
                  :disabled="currentPage === totalPages"
                  class="px-2 sm:px-3 py-1.5 sm:py-2 border border-gray-300 rounded-md text-xs sm:text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed whitespace-nowrap">
            末页
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
  .base-table-container {
    width: 100%;
  }

  /* 优化表格在移动端的显示 */
  @media (max-width: 640px) {
    table {
      font-size: 12px;
    }

    /* 确保表格容器可以水平滚动 */
    .overflow-x-auto {
      -webkit-overflow-scrolling: touch;
      scrollbar-width: thin;
    }

    /* 优化滚动条样式 */
    .overflow-x-auto::-webkit-scrollbar {
      height: 4px;
    }

    .overflow-x-auto::-webkit-scrollbar-track {
      background: #f1f1f1;
    }

    .overflow-x-auto::-webkit-scrollbar-thumb {
      background: #888;
      border-radius: 2px;
    }

    .overflow-x-auto::-webkit-scrollbar-thumb:hover {
      background: #555;
    }
  }
</style>

<!-- 添加全局样式块，确保所有响应式类都被包含在构建结果中 -->
<style>
/* 明确包含所有常用的响应式类，确保生产构建生成这些类 */
/* 移动优先设计，确保在小屏幕上正常显示 */
/* 这些类将被Tailwind CSS处理并包含在最终的CSS文件中 */
.sm\:flex {}
.md\:flex {}
.lg\:flex {}
.xl\:flex {}
.2xl\:flex {}

.sm\:block {}
.md\:block {}
.lg\:block {}
.xl\:block {}
.2xl\:block {}

.sm\:hidden {}
.md\:hidden {}
.lg\:hidden {}
.xl\:hidden {}
.2xl\:hidden {}

.sm\:grid {}
.md\:grid {}
.lg\:grid {}
.xl\:grid {}
.2xl\:grid {}

.sm\:w-full {}
.md\:w-full {}
.lg\:w-full {}
.xl\:w-full {}
.2xl\:w-full {}

.sm\:w-1\/2 {}
.md\:w-1\/2 {}
.lg\:w-1\/2 {}
.xl\:w-1\/2 {}
.2xl\:w-1\/2 {}

.sm\:w-1\/3 {}
.md\:w-1\/3 {}
.lg\:w-1\/3 {}
.xl\:w-1\/3 {}
.2xl\:w-1\/3 {}

.sm\:w-1\/4 {}
.md\:w-1\/4 {}
.lg\:w-1\/4 {}
.xl\:w-1\/4 {}
.2xl\:w-1\/4 {}

.sm\:p-4 {}
.md\:p-4 {}
.lg\:p-4 {}
.xl\:p-4 {}
.2xl\:p-4 {}

.sm\:p-6 {}
.md\:p-6 {}
.lg\:p-6 {}
.xl\:p-6 {}
.2xl\:p-6 {}

.sm\:text-sm {}
.md\:text-sm {}
.lg\:text-sm {}
.xl\:text-sm {}
.2xl\:text-sm {}

.sm\:text-base {}
.md\:text-base {}
.lg\:text-base {}
.xl\:text-base {}
.2xl\:text-base {}

.sm\:text-lg {}
.md\:text-lg {}
.lg\:text-lg {}
.xl\:text-lg {}
.2xl\:text-lg {}

.sm\:gap-3 {}
.md\:gap-3 {}
.lg\:gap-3 {}
.xl\:gap-3 {}
.2xl\:gap-3 {}

.sm\:gap-4 {}
.md\:gap-4 {}
.lg\:gap-4 {}
.xl\:gap-4 {}
.2xl\:gap-4 {}

.sm\:gap-6 {}
.md\:gap-6 {}
.lg\:gap-6 {}
.xl\:gap-6 {}
.2xl\:gap-6 {}

.sm\:justify-between {}
.md\:justify-between {}
.lg\:justify-between {}
.xl\:justify-between {}
.2xl\:justify-between {}

.sm\:items-center {}
.md\:items-center {}
.lg\:items-center {}
.xl\:items-center {}
.2xl\:items-center {}

.sm\:items-start {}
.md\:items-start {}
.lg\:items-start {}
.xl\:items-start {}
.2xl\:items-start {}

.sm\:mx-12 {}
.md\:mx-12 {}
.lg\:mx-12 {}
.xl\:mx-12 {}
.2xl\:mx-12 {}

.sm\:mx-auto {}
.md\:mx-auto {}
.lg\:mx-auto {}
.xl\:mx-auto {}
.2xl\:mx-auto {}

.sm\:max-w-lg {}
.md\:max-w-lg {}
.lg\:max-w-lg {}
.xl\:max-w-lg {}
.2xl\:max-w-lg {}

.sm\:max-w-xl {}
.md\:max-w-xl {}
.lg\:max-w-xl {}
.xl\:max-w-xl {}
.2xl\:max-w-xl {}

.sm\:w-40 {}
.md\:w-40 {}
.lg\:w-40 {}
.xl\:w-40 {}
.2xl\:w-40 {}

.sm\:w-64 {}
.md\:w-64 {}
.lg\:w-64 {}
.xl\:w-64 {}
.2xl\:w-64 {}

.sm\:w-80 {}
.md\:w-80 {}
.lg\:w-80 {}
.xl\:w-80 {}
.2xl\:w-80 {}
</style>
