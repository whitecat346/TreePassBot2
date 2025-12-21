<template>
  <div class="dashboard-container">
    <!-- 页面标题 -->
    <h1 class="text-2xl font-bold text-gray-900 mb-6">仪表盘</h1>

    <!-- 机器人状态卡片 -->
    <el-card shadow="never" class="mb-6">
      <template #header>
        <div class="card-header">
          <span class="text-lg font-medium">机器人状态</span>
        </div>
      </template>

      <div v-if="loading" v-loading="loading" style="height: 150px;"></div>
      <div v-else>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- 基本状态 -->
          <div>
            <el-descriptions :column="1" border>
              <el-descriptions-item label="当前状态">
                <el-tag :type="botStatus.status === 'Running' ? 'success' : botStatus.status === 'Error' ? 'danger' : 'warning'">
                  {{ botStatus.status }}
                </el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="启动时间">
                {{ botStatus.startTime ? botStatus.startTime : 'N/A' }}
              </el-descriptions-item>
              <el-descriptions-item label="版本">
                {{ botStatus.version || 'N/A' }}
              </el-descriptions-item>
              <el-descriptions-item label="协议">
                {{ botStatus.protocol || 'N/A' }}
              </el-descriptions-item>
            </el-descriptions>

            <!-- 控制按钮 -->
            <div class="mt-4 flex space-x-3">
              <el-button type="primary" @click="handleStart" :disabled="botStatus.status === 'Running'">
                启动
              </el-button>
              <el-button type="danger" @click="handleStop" :disabled="botStatus.status !== 'Running'">
                停止
              </el-button>
            </div>
          </div>

          <!-- 服务器状态 -->
          <div>
            <h3 class="text-base font-medium text-gray-900 mb-3">服务器运行状况</h3>
            <el-descriptions :column="1" border>
              <el-descriptions-item label="CPU使用率">
                <div class="flex items-center">
                  <el-progress :percentage="serverStatus.cpuUsage"
                               :stroke-width="8"
                               :show-text="false"
                               class="flex-1 mr-3"
                               :color="getProgressColor(serverStatus.cpuUsage)" />
                  <span class="text-sm">{{ serverStatus.cpuUsage }}%</span>
                </div>
              </el-descriptions-item>
              <el-descriptions-item label="内存使用率">
                <div class="flex items-center">
                  <el-progress :percentage="serverStatus.memoryUsage"
                               :stroke-width="8"
                               :show-text="false"
                               class="flex-1 mr-3"
                               :color="getProgressColor(serverStatus.memoryUsage)" />
                  <span class="text-sm">{{ serverStatus.memoryUsage }}%</span>
                </div>
              </el-descriptions-item>
              <el-descriptions-item label="磁盘使用率">
                <div class="flex items-center">
                  <el-progress :percentage="serverStatus.diskUsage"
                               :stroke-width="8"
                               :show-text="false"
                               class="flex-1 mr-3"
                               :color="getProgressColor(serverStatus.diskUsage)" />
                  <span class="text-sm">{{ serverStatus.diskUsage }}%</span>
                </div>
              </el-descriptions-item>
              <el-descriptions-item label="运行时间">
                {{ serverStatus.uptime || 'N/A' }}
              </el-descriptions-item>
            </el-descriptions>
          </div>
        </div>
      </div>
    </el-card>

    <!-- 机器人配置信息 -->
    <el-card shadow="never" class="mb-6">
      <template #header>
        <div class="card-header">
          <span class="text-lg font-medium">机器人配置信息</span>
        </div>
      </template>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <el-card shadow="hover" class="border border-gray-200">
          <div class="text-center">
            <div class="text-3xl font-bold text-indigo-600 mb-2">{{ botConfig.groupCount }}</div>
            <div class="text-sm text-gray-500">所在群组数</div>
          </div>
        </el-card>

        <el-card shadow="hover" class="border border-gray-200">
          <div class="text-center">
            <div class="text-3xl font-bold text-green-600 mb-2">{{ botConfig.totalMemberCount }}</div>
            <div class="text-sm text-gray-500">总成员数</div>
          </div>
        </el-card>

        <el-card shadow="hover" class="border border-gray-200">
          <div class="text-center">
            <div class="text-3xl font-bold text-blue-600 mb-2">{{ botConfig.enabledPlugins }}</div>
            <div class="text-sm text-gray-500">启用插件数</div>
          </div>
        </el-card>
      </div>

      <!-- 详细配置 -->
      <div class="mt-6">
        <h3 class="text-base font-medium text-gray-900 mb-3">详细配置</h3>
        <el-descriptions :column="2" border>
          <el-descriptions-item label="机器人名称">
            {{ botConfig.name || 'TreePassBot' }}
          </el-descriptions-item>
          <el-descriptions-item label="连接方式">
            <el-tag type="info">WebSocket</el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="协议版本">
            OneBot 11
          </el-descriptions-item>
          <el-descriptions-item label="服务端">
            {{ botConfig.server || 'napcat' }}
          </el-descriptions-item>
        </el-descriptions>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">defineOptions({
  name: 'DashboardView'
});
import { ref, onMounted, onUnmounted } from 'vue';
import {
  getBotStatus,
  startBot,
  stopBot,
  getServerStatus,
  getTotalGroupCount,
  getTotalMemberCount,
  getEnabledPluginCount,
  type ServerStatus,
  type BotStatus,
} from '@/services/api';
import { ElMessage } from 'element-plus';

// 响应式数据
const loading = ref(true);
const botStatus = ref<BotStatus>({
  status: 'Stopped',
  startTime: 'Unknown',
  version: '1.0.0',
  protocol: 'OneBot 11'
});

const serverStatus = ref<ServerStatus>({
  cpuUsage: 0,
  memoryUsage: 0,
  diskUsage: 0,
  uptime: '0 days 00:00:00',
  timestamp: "1970-01-01T00:00:00Z"
});

// 定时器ID
const timerId = ref<number | null>(null);

// 机器人配置信息（模拟数据，后续从API获取）
const botConfig = ref({
  name: 'TreePassBot',
  groupCount: 0,
  totalMemberCount: 0,
  enabledPlugins: 0,
  server: 'napcat'
});

// 获取进度条颜色
const getProgressColor = (percentage: number) => {
  if (percentage < 50) return '#67c23a';
  if (percentage < 80) return '#e6a23c';
  return '#f56c6c';
};

// 获取机器人状态
const fetchBotStatus = async () => {
  try {
    const response = await getBotStatus();
    if (response.data.success) {
      botStatus.value = response.data.data;
    } else {
      // 当API返回success为false时，抛出错误
      throw new Error(response.data.message || '获取机器人状态失败');
    }
  } catch {
    ElMessage.error('获取机器人状态失败');
  }
};

const fetchBotConfig = async () => {
  try {
      const memberCount = await getTotalMemberCount();
      const groupCount = await getTotalGroupCount();
      const enabledPlugins = await getEnabledPluginCount();
    if (memberCount.data.success && groupCount.data.success && enabledPlugins.data.success) {
      botConfig.value = {
        name: 'TreePassBot',
        groupCount: groupCount.data.data,
        totalMemberCount: memberCount.data.data,
        enabledPlugins: enabledPlugins.data.data,
        server: 'napcat'
      };
    } else {
      // 当API返回success为false时，抛出错误
      throw new Error('获取群组数或成员数或插件数失败');
    }
  } catch {
    ElMessage.error('获取机器人配置失败');
  }
}

// 获取服务器状态
const fetchServerStatus = async () => {
  try {
    const response = await getServerStatus();
    if (response.data.success) {
      serverStatus.value = response.data.data;
    } else {
      // 当API返回success为false时，抛出错误
      throw new Error(response.data.message || '获取服务器状态失败');
    }
  } catch {
    ElMessage.error('获取服务器状态失败');
  }
};

// 启动机器人
const handleStart = async () => {
  try {
    const response = await startBot();
    if (response.data.success) {
      ElMessage.success('机器人启动成功');
      fetchBotStatus();
      fetchBotConfig();
    } else {
      // 当API返回success为false时，抛出错误
      throw new Error(response.data.message || '启动失败');
    }
  } catch {
    ElMessage.error('启动失败');
  }
};

// 停止机器人
const handleStop = async () => {
  try {
    const response = await stopBot();
    if (response.data.success) {
      ElMessage.success('机器人已停止');
      fetchBotStatus();
      fetchBotConfig();
    } else {
      // 当API返回success为false时，抛出错误
      throw new Error(response.data.message || '停止失败');
    }
  } catch {
    ElMessage.error('停止失败');
  }
};

// 初始化数据
const initData = async () => {
  try {
    loading.value = true;
    // 并行请求数据，使用Promise.allSettled确保所有请求完成后再设置loading为false
    await Promise.allSettled([
      fetchBotStatus(),
      fetchServerStatus(),
      fetchBotConfig()
    ]);
  } catch (err) {
    console.error('初始化数据失败:', err);
  } finally {
    loading.value = false;
  }
};

// 组件挂载时初始化
onMounted(() => {
  initData();

  // 启动定时刷新（每10秒）
  timerId.value = window.setInterval(() => {
    fetchBotStatus();
    fetchServerStatus();
    fetchBotConfig();
  }, 10000);
});

// 组件卸载时清理定时器
onUnmounted(() => {
  if (timerId.value) {
    window.clearInterval(timerId.value);
    timerId.value = null;
  }
});</script>

<style scoped>
  .dashboard-container {
    width: 100%;
  }

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
</style>
