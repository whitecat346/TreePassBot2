<template>
  <div class="message-log-container">
    <!-- 页面标题 -->
    <h1 class="text-2xl font-bold text-gray-900 mb-6">消息日志</h1>

    <!-- 筛选和搜索区域 -->
    <el-card shadow="never" class="mb-6">
      <div class="filter-container flex flex-wrap gap-4 items-center">
        <div class="flex items-center space-x-2">
          <span class="text-sm font-medium text-gray-700">群聊：</span>
          <el-select v-model="filterGroupId"
                     placeholder="全部"
                     clearable
                     size="small"
                     class="w-64">
            <el-option v-for="group in groups"
                       :key="group.groupId"
                       :label="group.name"
                       :value="group.groupId" />
          </el-select>
        </div>

        <div class="flex items-center space-x-2">
          <span class="text-sm font-medium text-gray-700">包含撤回消息：</span>
          <el-switch v-model="filterIncludeRecalled"
                     size="small" />
        </div>

        <div class="flex items-center space-x-2 flex-1 md:flex-none">
          <span class="text-sm font-medium text-gray-700">搜索：</span>
          <el-input v-model="filterKeyword"
                    placeholder="消息内容/用户名/用户ID"
                    clearable
                    size="small"
                    prefix-icon="Search"
                    class="w-full md:w-64"
                    @keydown.enter="handleSearch" />
          <el-button type="primary"
                     size="small"
                     class="ml-2"
                     @click="handleSearch">
            搜索
          </el-button>
          <el-button size="small"
                     class="ml-2"
                     @click="handleReset">
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
          消息日志 (共 {{ totalMessages }} 条)
        </h3>
        <div class="text-sm text-gray-500">
          显示：
          <el-radio-group v-model="displayMode"
                          size="small"
                          class="ml-2">
            <el-radio-button label="all">全部</el-radio-button>
            <el-radio-button label="recalled">仅撤回</el-radio-button>
            <el-radio-button label="normal">仅正常</el-radio-button>
          </el-radio-group>
        </div>
      </div>

      <!-- 消息列表内容 -->
      <div ref="messageListRef"
           class="discord-style-messages"
           @scroll="handleScroll">
        <!-- 加载更多指示器 -->
        <div v-if="loadingMore"
             class="flex justify-center items-center py-4 text-gray-500">
          <el-icon class="mr-2 animate-spin"><Loading /></el-icon>
          加载更多消息...
        </div>

        <!-- 没有更多消息提示 -->
        <div v-else-if="!hasMore && messages.length > 0"
             class="flex justify-center items-center py-4 text-gray-500 text-sm">
          没有更多历史消息了
        </div>

        <!-- 消息列表 -->
        <div v-if="filteredMessages.length > 0"
             class="messages-container">
          <!-- 按日期分组显示消息 -->
          <div v-for="(dateGroup, date) in groupedMessages"
               :key="date"
               class="mb-8">
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
              <div v-for="message in dateGroup"
                   :key="message.id"
                   class="message-item flex"
                   :class="{ 'recalled-message': message.isRecalled }">
                <!-- 消息内容 -->
                <div class="message-content flex-1">
                  <!-- 消息头部：用户名和时间 -->
                  <div class="message-header flex items-center space-x-2 mb-1">
                    <span class="message-username font-medium">{{ message.username }}</span>
                    <span class="message-time text-xs text-gray-500">
                      {{ formatTime(message.sendAt) }}
                    </span>

                    <!-- 撤回标记 -->
                    <el-tag v-if="message.isRecalled"
                            size="small"
                            type="danger"
                            class="ml-2">
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
        <div v-else-if="!loading"
             class="p-12 text-center">
          <EmptyState title="暂无消息日志"
                      description="当前没有消息日志，或搜索条件过于严格"
                      icon="Database" />
        </div>

        <!-- 加载中状态 -->
        <div v-else
             class="p-8">
          <div class="flex items-center justify-center">
            <el-icon class="animate-spin rounded-full h-10 w-10 border-b-2 border-indigo-600"></el-icon>
            <span class="ml-3 text-gray-500">加载中...</span>
          </div>
        </div>
      </div>

      <!-- 自动滚动到底部按钮 -->
      <div v-if="!isAtBottom && messages.length > 0"
           class="fixed bottom-8 right-8">
        <el-button type="primary"
                   size="small"
                   circle
                   @click="scrollToBottom"
                   :icon="ArrowDownBold">
          回到底部
        </el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">defineOptions({
  name: 'MessageLogView'
});
import { ref, computed, onMounted, watch, nextTick, onBeforeUnmount } from 'vue';
import {
  getMessageLogs,
  getGroups,
  type Group,
  type MessageLog
} from '@/services/api';
import { ElMessage, ElIcon } from 'element-plus';
import { Loading, ArrowDownBold } from '@element-plus/icons-vue';

// 导入通用组件
import EmptyState from '../components/common/EmptyState.vue';

// 响应式数据
const messages = ref<MessageLog[]>([]);
const loading = ref(true);
const loadingMore = ref(false);
const hasMore = ref(true);
const totalMessages = ref(0);
const nextCursor = ref<string | null>(null);
const messageListRef = ref<HTMLElement | null>(null);
const isAtBottom = ref(true);
const autoScrollEnabled = ref(true);
const lastScrollTop = ref(0);

// 筛选条件
const filterGroupId = ref('');
const filterIncludeRecalled = ref(true);
const filterKeyword = ref('');
const displayMode = ref<'all' | 'recalled' | 'normal'>('all');

// 模拟群组数据（实际应从API获取）
const groups = ref<Group[]>([]);

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

// 按日期分组的消息
const groupedMessages = computed(() => {
  return filteredMessages.value.reduce((groups, message) => {
    const date = new Date(message.sendAt).toISOString().split('T')[0] || 'unknown';
    if (!groups[date]) {
      groups[date] = [];
    }
    groups[date].push(message);
    return groups;
  }, {} as Record<string, MessageLog[]>);
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

// 滚动到底部
const scrollToBottom = () => {
  const container = messageListRef.value;
  if (container) {
    container.scrollTop = container.scrollHeight;
    isAtBottom.value = true;
  }
};

// 检查是否在底部
const checkIsAtBottom = () => {
  const container = messageListRef.value;
  if (container) {
    const { scrollTop, scrollHeight, clientHeight } = container;
    isAtBottom.value = scrollHeight - scrollTop - clientHeight < 100; // 100px tolerance
  }
};

// 获取消息日志
const fetchMessageLogs = async (isLoadMore = false) => {
  try {
    // 加载群组列表
    const groupResponse = await getGroups();
    if (groupResponse.data.success) {
      groups.value = groupResponse.data.data;
    } else {
      ElMessage.error('获取群组列表失败');
    }

    if (isLoadMore) {
      if (!hasMore.value || loadingMore.value) return;
      loadingMore.value = true;
    } else {
      loading.value = true;
      messages.value = [];
      nextCursor.value = null;
      hasMore.value = true;
    }

    // 构建请求参数
    const params = {
      groupId: filterGroupId.value,
      startTime: '',
      endTime: '',
      beforeId: isLoadMore && nextCursor.value ? nextCursor.value : undefined,
      limit: 20 // 默认加载20条
    };

    const response = await getMessageLogs(params);
    if (response.data.success) {
      const newMessages = response.data.data.items;

      // 增量加载，避免重复
      if (isLoadMore) {
        // 过滤掉已存在的消息
        const existingIds = new Set(messages.value.map(msg => msg.id));
        const uniqueMessages = newMessages.filter(msg => !existingIds.has(msg.id));
        messages.value = [...uniqueMessages, ...messages.value];
      } else {
        messages.value = newMessages;
      }

      hasMore.value = response.data.data.hasMore;
      nextCursor.value = response.data.data.nextCursor;
      totalMessages.value = response.data.data.total;

      // 如果是初始加载，滚动到底部
      if (!isLoadMore) {
        await nextTick();
        scrollToBottom();
      }
    }
  } catch (error) {
    ElMessage.error('获取消息日志失败');
    console.error('获取消息日志失败:', error);
  } finally {
    loading.value = false;
    loadingMore.value = false;
  }
};

// 处理滚动事件
const handleScroll = () => {
  const container = messageListRef.value;
  if (!container) return;

  const { scrollTop } = container;
  lastScrollTop.value = scrollTop;
  checkIsAtBottom();

  // 滚动到顶部边界，加载更多消息
  if (scrollTop <= 100 && hasMore.value && !loadingMore.value) {
    fetchMessageLogs(true);
  }
};

// 处理搜索
const handleSearch = () => {
  fetchMessageLogs();
};

// 处理重置
const handleReset = () => {
  filterGroupId.value = '';
  filterIncludeRecalled.value = true;
  filterKeyword.value = '';
  displayMode.value = 'all';
  fetchMessageLogs();
};

// 监听消息变化，自动滚动到底部
watch(messages, (newVal, oldVal) => {
  // 只有当自动滚动启用且当前在底部时，才自动滚动
  if (autoScrollEnabled.value && isAtBottom.value && newVal.length > oldVal.length) {
    nextTick(() => scrollToBottom());
  }
});

// 组件挂载时初始化
onMounted(() => {
  fetchMessageLogs();
  // 添加滚动事件监听
  const container = messageListRef.value;
  if (container) {
    container.addEventListener('scroll', handleScroll, { passive: true });
  }
});

// 组件卸载前清理
onBeforeUnmount(() => {
  // 移除滚动事件监听
  const container = messageListRef.value;
  if (container) {
    container.removeEventListener('scroll', handleScroll);
  }
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
