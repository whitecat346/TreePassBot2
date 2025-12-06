<script setup lang="ts">
import { ref, computed } from 'vue';
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

// 计算总页数
const totalPages = computed(() => {
  return Math.ceil(props.total / pageSize.value);
});

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
</script>

<template>
  <div class="base-table-container">
    <!-- 标题和搜索 -->
    <div v-if="props.title || props.showSearch" class="flex items-center justify-between mb-4">
      <h3 v-if="props.title" class="text-lg font-medium text-gray-900">{{ props.title }}</h3>

      <div v-if="props.showSearch" class="flex items-center">
        <input
          v-model="searchKeyword"
          @keydown="handleSearchKeydown"
          placeholder="{{ props.searchPlaceholder }}"
          class="pl-10 pr-4 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
        />
        <button
          @click="handleSearch"
          class="ml-2 inline-flex items-center px-3 py-2 border border-transparent text-sm leading-4 font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
        >
          搜索
        </button>
      </div>
    </div>

    <!-- 表格 -->
    <div class="overflow-x-auto">
      <table
        class="min-w-full divide-y divide-gray-200"
        :class="{ 'border border-gray-200': props.bordered }"
      >
        <!-- 表头 -->
        <thead class="bg-gray-50">
          <tr>
            <th
              v-for="column in props.columns"
              :key="column.prop"
              :width="column.width"
              :class="[
                'px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider',
                column.align ? `text-${column.align}` : 'text-left'
              ]"
            >
              <div class="flex items-center">
                {{ column.label }}

                <!-- 排序图标 -->
                <span v-if="column.sortable" class="ml-1 cursor-pointer" @click="handleSortChange(column.prop, sortOrder === 'asc' ? 'desc' : 'asc')">
                  <svg
                    v-if="sortProp === column.prop"
                    :class="sortOrder === 'asc' ? 'rotate-180' : ''"
                    class="w-4 h-4 text-gray-400"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 15l7-7 7 7" />
                  </svg>
                  <svg
                    v-else
                    class="w-4 h-4 text-gray-400"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
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
              <td :colspan="props.columns.length" class="px-6 py-12 text-center">
                <div class="flex items-center justify-center">
                  <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600"></div>
                  <span class="ml-2 text-gray-500">加载中...</span>
                </div>
              </td>
            </tr>
          </template>

          <template v-else-if="props.data && props.data.length > 0">
            <!-- 数据行 -->
            <tr v-for="(row, index) in props.data" :key="index" class="hover:bg-gray-50">
              <td
                v-for="column in props.columns"
                :key="column.prop"
                :class="[
                  'px-6 py-4 whitespace-nowrap text-sm text-gray-900',
                  column.align ? `text-${column.align}` : 'text-left'
                ]"
              >
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
              <td :colspan="props.columns.length" class="px-6 py-12">
                <EmptyState />
              </td>
            </tr>
          </template>
        </tbody>
      </table>
    </div>

    <!-- 分页 -->
    <div v-if="props.showPagination && props.total > 0" class="flex items-center justify-between mt-4">
      <div class="text-sm text-gray-500">
        显示 {{ ((currentPage - 1) * pageSize) + 1 }} 到 {{ Math.min(currentPage * pageSize, props.total) }} 条，共 {{ props.total }} 条
      </div>

      <div class="flex items-center space-x-2">
        <!-- 页码选择器 -->
        <select
          v-model="pageSize"
          @change="handlePageSizeChange(pageSize)"
          class="text-sm border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
        >
          <option v-for="option in props.pageSizeOptions" :key="option" :value="option">
            {{ option }} 条/页
          </option>
        </select>

        <!-- 分页按钮 -->
        <button
          @click="handlePageChange(1)"
          :disabled="currentPage === 1"
          class="px-3 py-1 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          首页
        </button>

        <button
          @click="handlePageChange(currentPage - 1)"
          :disabled="currentPage === 1"
          class="px-3 py-1 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          上一页
        </button>

        <span class="px-3 py-1 text-sm font-medium text-gray-700">
          {{ currentPage }} / {{ totalPages }}
        </span>

        <button
          @click="handlePageChange(currentPage + 1)"
          :disabled="currentPage === totalPages"
          class="px-3 py-1 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          下一页
        </button>

        <button
          @click="handlePageChange(totalPages)"
          :disabled="currentPage === totalPages"
          class="px-3 py-1 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          末页
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.base-table-container {
  width: 100%;
}
</style>
