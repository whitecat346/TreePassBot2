<template>
  <div class="message-log-container">
    <!-- 页面标题 -->
    <h1 class="text-2xl font-bold text-gray-900 mb-6">消息日志</h1>

    <!-- 筛选和搜索区域 -->
    <el-card shadow="never" class="mb-6">
      <div class="filter-container flex flex-wrap gap-4 items-center">
        <div class="flex items-center space-x-2">
          <span class="text-sm font-medium text-gray-700">群聊：</span>
          <el-select
            v-model="filterGroupId"
            placeholder="全部"
            clearable
            size="small"
            class="w-64"
          >
            <el-option
              v-for="group in groups"
              :key="group.id"
              :label="group.name"
              :value="group.id"
            />
          </el-select>
        </div>

        <div class="flex items-center space-x-2">
          <span class="text-sm font-medium text-gray-700">包含撤回消息：</span>
          <el-switch
            v-model="filterIncludeRecalled"
            size="small"
          />
        </div>

        <div class="flex items-center space-x-2 flex-1 md:flex-none">
          <span class="text-sm font-medium text-gray-700">搜索：</span>
          <el-input
            v-model="filterKeyword"
            placeholder="消息内容/用户名/用户ID"
            clearable
            size="small"
            prefix-icon="Search"
            class="w-full md:w-64"
            @keydown.enter="handleSearch"
          />
          <el-button
            type="primary"
            size="small"
            class="ml-2"
            @click="handleSearch"
          >
            搜索
          </el-button>
          <el-button
            size="small"
            class="ml-2"
            @click="handleReset"
          >
            重置
          </el-button>
        </div>
      </div>
    </el-card>

    <!-- 消息列表 -->
    <el-card shadow="never">
      <!-- 消息列表头部 -->
      <div class="message-list-header flex justify-between items-center pb-4 border-b border-gray-200 mb-4">
        <h3 class="text-lg font-medium text-gray-900">
          消息日志 (共 {{ filteredMessages.length }} 条)
        </h3>
        <div class="text-sm text-gray-500">
          显示：
          <el-radio-group
            v-model="displayMode"
            size="small"
            class="ml-2"
          >
            <el-radio-button label="all">全部</el-radio-button>
            <el-radio-button label="recalled">仅撤回</el-radio-button>
            <el-radio-button label="normal">仅正常</el-radio-button>
          </el-radio-group>
        </div>
      </div>

      <!-- 消息列表内容 -->
      <div v-if="loading" class="p-8">
        <div class="flex items-center justify-center">
          <div class="animate-spin rounded-full h-10 w-10 border-b-2 border-indigo-600"></div>
          <span class="ml-3 text-gray-500">加载中...</span>
        </div>
      </div>

      <div v-else-if="filteredMessages.length > 0" class="discord-style-messages">
        <!-- 按日期分组显示消息 -->
        <div
          v-for="(dateGroup, date) in groupedMessages"
          :key="date"
          class="mb-8"
        >
          <!-- 日期分隔线 -->
          <div class="flex items-center justify-center mb-4">
            <div class="h-px bg-gray-200 flex-1"></div>
            <span class="px-4 py-1 text-xs font-medium text-gray-500 bg-gray-100 rounded-full mx-4">
              {{ formatDate(date) }}
            </span>
            <div class="h-px bg-gray-200 flex-1"></div>
          </div>

          <!-- 该日期下的消息 -->
          <div class="space-y-4">
            <div
              v-for="message in dateGroup"
              :key="message.id"
              class="message-item flex gap-3"
              :class="{ 'recalled-message': message.isRecalled }"
            >
              <!-- 头像 -->
              <div class="message-avatar flex-shrink-0">
                <Avatar
                  :src="null"
                  :username="message.username"
                  size="sm"
                  rounded
                />
              </div>

              <!-- 消息内容 -->
              <div class="message-content flex-1">
                <!-- 消息头部：用户名和时间 -->
                <div class="message-header flex items-center space-x-2 mb-1">
                  <span class="message-username font-medium">{{ message.username }}</span>
                  <span class="message-time text-xs text-gray-500">
                    {{ formatTime(message.sendTime) }}
                  </span>

                  <!-- 撤回标记 -->
                  <el-tag
                    v-if="message.isRecalled"
                    size="small"
                    type="danger"
                    class="ml-2"
                  >
                    已撤回
                  </el-tag>
                </div>

                <!-- 消息文本 -->
                <div class="message-text">
                  <template v-if="message.isRecalled">
                    <span class="text-gray-500 italic">
                      此消息已被撤回
                      <span v-if="message.recalledBy" class="ml-1">
                        (操作者：{{ message.recalledBy }})
                      </span>
                    </span>
                  </template>
                  <template v-else>
                    <div class="whitespace-pre-wrap break-words">
                      {{ message.content }}
                    </div>
                  </template>
                </div>

                <!-- 消息元信息 -->
                <div class="message-meta mt-2 flex items-center space-x-4 text-xs text-gray-500">
                  <span>ID: {{ message.id }}</span>
                  <span>群聊: {{ message.groupName }}</span>
                  <span>用户ID: {{ message.userId }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 空状态 -->
      <div v-else class="p-12">
        <EmptyState
          title="暂无消息日志"
          description="当前没有消息日志，或搜索条件过于严格"
          icon="Database"
        />
      </div>

      <!-- 分页 -->
      <div v-if="!loading && filteredMessages.length > 0" class="p-4 border-t border-gray-200 mt-6">
        <el-pagination
          v-model:current-page="currentPage"
          v-model:page-size="pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="messages.length"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handlePageSizeChange"
          @current-change="handlePageChange"
        />
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import {
  getMessageLogs,
  type MessageLog
} from '@/services/api';
import { ElMessage } from 'element-plus';

// 导入通用组件
import Avatar from '../components/common/BaseAvatar.vue';
import EmptyState from '../components/common/EmptyState.vue';

// 响应式数据
const messages = ref<MessageLog[]>([]);
const loading = ref(true);
const currentPage = ref(1);
const pageSize = ref(20);

// 筛选条件
const filterGroupId = ref('');
const filterIncludeRecalled = ref(true);
const filterKeyword = ref('');
const displayMode = ref<'all' | 'recalled' | 'normal'>('all');

// 模拟群组数据（实际应从API获取）
const groups = ref([
  { id: '1', name: '群组1' },
  { id: '2', name: '群组2' },
  { id: '3', name: '群组3' }
]);

// 按日期分组的消息
const groupedMessages = computed(() => {
  return filteredMessages.value.reduce((groups, message) => {
    const date = new Date(message.sendTime).toISOString().split('T')[0] || 'unknown';
    if (!groups[date]) {
      groups[date] = [];
    }
    groups[date].push(message);
    return groups;
  }, {} as Record<string, MessageLog[]>);
});

// 过滤后的消息
const filteredMessages = computed(() => {
  return messages.value.filter(message => {
    // 群组筛选
    if (filterGroupId.value && message.groupId !== filterGroupId.value) {
      return false;
    }

    // 撤回状态筛选
    if (displayMode.value === 'recalled' && !message.isRecalled) {
      return false;
    }
    if (displayMode.value === 'normal' && message.isRecalled) {
      return false;
    }

    // 关键词筛选
    if (filterKeyword.value) {
      const keyword = filterKeyword.value.toLowerCase();
      return (
        message.content.toLowerCase().includes(keyword) ||
        message.username.toLowerCase().includes(keyword) ||
        message.userId.includes(keyword) ||
        message.groupName.toLowerCase().includes(keyword)
      );
    }

    return true;
  });
});

// 格式化日期
const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  return date.toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit'
  });
};

// 格式化时间
const formatTime = (dateString: string) => {
  const date = new Date(dateString);
  return date.toLocaleTimeString('zh-CN', {
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  });
};

// 获取消息日志
const fetchMessageLogs = async () => {
  try {
    loading.value = true;

    // 构建请求参数
    const params = {
      groupId: filterGroupId.value,
      startTime: '',
      endTime: '',
      page: currentPage.value,
      pageSize: pageSize.value
    };

    const response = await getMessageLogs(params);
    if (response.data.success) {
      messages.value = response.data.data.items;
      // 实际项目中应该从API返回的total字段获取
      // messageTotal.value = response.data.data.total;
    }
  } catch (error) {
    ElMessage.error('获取消息日志失败');
    console.error('获取消息日志失败:', error);
  } finally {
    loading.value = false;
  }
};

// 处理搜索
const handleSearch = () => {
  currentPage.value = 1;
  fetchMessageLogs();
};

// 处理重置
const handleReset = () => {
  filterGroupId.value = '';
  filterIncludeRecalled.value = true;
  filterKeyword.value = '';
  displayMode.value = 'all';
  currentPage.value = 1;
  fetchMessageLogs();
};

// 处理分页大小变化
const handlePageSizeChange = (size: number) => {
  pageSize.value = size;
  currentPage.value = 1;
  fetchMessageLogs();
};

// 处理页码变化
const handlePageChange = (page: number) => {
  currentPage.value = page;
  fetchMessageLogs();
};

// 组件挂载时初始化
onMounted(() => {
  fetchMessageLogs();
});
</script>

<style scoped>
.message-log-container {
  width: 100%;
}

.filter-container {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
  align-items: center;
}

.message-list-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.discord-style-messages {
  max-height: calc(100vh - 400px);
  overflow-y: auto;
  padding-right: 8px;
}

.message-item {
  padding: 8px;
  border-radius: 8px;
  transition: background-color 0.2s;
}

.message-item:hover {
  background-color: #f9fafb;
}

.message-item.recalled-message {
  opacity: 0.7;
}

.message-avatar {
  flex-shrink: 0;
}

.message-content {
  flex: 1;
  min-width: 0;
}

.message-header {
  display: flex;
  align-items: center;
}

.message-username {
  font-weight: 600;
  color: #1f2937;
}

.message-time {
  color: #6b7280;
  font-size: 12px;
}

.message-text {
  color: #374151;
  line-height: 1.5;
  margin-bottom: 4px;
}

.message-meta {
  display: flex;
  align-items: center;
  color: #9ca3af;
  font-size: 12px;
}

/* 滚动条样式 */
.discord-style-messages::-webkit-scrollbar {
  width: 6px;
}

.discord-style-messages::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 3px;
}

.discord-style-messages::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 3px;
}

.discord-style-messages::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}
</style>
