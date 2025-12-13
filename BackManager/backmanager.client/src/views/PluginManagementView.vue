<template>
  <el-card shadow="never">
    <template #header>
      <div class="card-header">
        <span>插件管理</span>
        <!-- 新增：上传插件组件 -->
        <el-upload action="http://localhost:5000/api/plugins/upload"
                   name="pluginFile"
                   :show-file-list="false"
                   :on-success="handleUploadSuccess"
                   :on-error="handleUploadError"
                   :before-upload="beforeUpload"
                   accept=".dll">
          <el-button type="primary">上传新插件</el-button>
        </el-upload>
      </div>
    </template>

    <!-- 表格部分 -->
    <el-table :data="plugins" v-loading="loading" style="width: 100%">
      <el-table-column prop="name" label="插件名称" width="220" />
      <el-table-column prop="description" label="功能描述" />
      <el-table-column prop="version" label="版本" width="100" />
      <el-table-column label="状态" width="120">
        <template #default="{ row }">
          <StatusBadge :status="getPluginStatusType(row.status)"
                       :text="getPluginStatusText(row.status)" />
        </template>
      </el-table-column>
      <el-table-column label="操作" width="120">
        <template #default="{ row }">
          <el-switch v-model="row.isEnabled"
                     @change="handleToggle(row)"
                     :disabled="row.status === 'Error'" />
        </template>
      </el-table-column>
      <el-table-column label="操作" width="150" fixed="right">
        <template #default="{ row }">
          <el-button type="primary"
                     size="small"
                     @click="handleViewDetails(row)"
                     :disabled="!row.isEnabled">
            详情
          </el-button>
        </template>
      </el-table-column>
    </el-table>
  </el-card>

  <!-- 插件详情弹窗 -->
  <el-dialog v-model="detailVisible"
             title="插件详情"
             width="600px"
             :before-close="handleCloseDetail">
    <div v-if="selectedPlugin" class="plugin-detail-content">
      <el-descriptions :column="1" border>
        <el-descriptions-item label="插件名称">
          {{ selectedPlugin.name }}
        </el-descriptions-item>
        <el-descriptions-item label="版本">
          {{ selectedPlugin.version }}
        </el-descriptions-item>
        <el-descriptions-item label="功能描述">
          {{ selectedPlugin.description }}
        </el-descriptions-item>
        <el-descriptions-item label="状态">
          <StatusBadge :status="getPluginStatusType(selectedPlugin.status)"
                       :text="getPluginStatusText(selectedPlugin.status)" />
        </el-descriptions-item>
        <el-descriptions-item label="启用状态">
          <el-switch v-model="selectedPlugin.isEnabled"
                     :disabled="true"
                     active-text="已启用"
                     inactive-text="未启用" />
        </el-descriptions-item>
      </el-descriptions>
    </div>

    <template #footer>
      <span class="dialog-footer">
        <el-button @click="handleCloseDetail">关闭</el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">defineOptions({
  name: 'PluginManagementView'
});
import { ref, onMounted } from 'vue';
import { getPlugins, togglePlugin, type Plugin } from '@/services/api'
import { ElMessage, type UploadProps } from 'element-plus';

// 导入通用组件
import StatusBadge from '../components/common/StatusBadge.vue';

const loading = ref(true);
const plugins = ref<Plugin[]>([]);

// 弹窗控制
const detailVisible = ref(false);
const selectedPlugin = ref<Plugin | null>(null);

const fetchPlugins = async () => {
        try {
            loading.value = true;
            const response = await getPlugins();
            if (response.data.success) {
                plugins.value = response.data.data;
            }
        } catch {
            ElMessage.error('获取插件列表失败');
        } finally {
            loading.value = false;
        }
    };

const handleToggle = async (plugin: Plugin) => {
    try {
        await togglePlugin(plugin.id);
        ElMessage.success(`插件 '${plugin.name}' 状态已更新`);
    } catch {
        // 如果API调用失败，则将开关恢复到原始状态
        plugin.isEnabled = !plugin.isEnabled;
        ElMessage.error('更新插件状态失败');
    }
};

/**
 * 根据插件状态获取状态类型
 */
const getPluginStatusType = (status: string) => {
    switch (status) {
        case 'Running':
            return 'success';
        case 'Warning':
            return 'warning';
        case 'Error':
            return 'error';
        case 'Disabled':
            return 'info';
        default:
            return 'info';
    }
};

/**
 * 根据插件状态获取状态文本
 */
const getPluginStatusText = (status: string) => {
    switch (status) {
        case 'Running':
            return '正常运行';
        case 'Warning':
            return '警告';
        case 'Error':
            return '异常';
        case 'Disabled':
            return '未启用';
        default:
            return '未知';
    }
};

/**
 * 查看插件详情
 */
const handleViewDetails = (plugin: Plugin) => {
    selectedPlugin.value = plugin;
    detailVisible.value = true;
};

/**
 * 关闭详情弹窗
 */
const handleCloseDetail = () => {
    detailVisible.value = false;
    // 延迟清空数据，避免弹窗关闭时的闪烁
    setTimeout(() => {
        selectedPlugin.value = null;
    }, 300);
};

/**
 * 上传成功后的回调
 */
const handleUploadSuccess = (response: { name?: string }, uploadFile: { name: string }) => {
    ElMessage.success(`插件 "${response.name || uploadFile.name}" 上传成功！`);
    fetchPlugins(); // 上传成功后，立即刷新插件列表
};

/**
 * 上传失败后的回调
 */
const handleUploadError = (error: Error) => {
    // 尝试解析后端返回的 JSON 错误信息
    try {
        const errorResponse = JSON.parse(error.message);
        ElMessage.error(errorResponse.message || '上传失败，请稍后重试。');
    } catch {
        ElMessage.error('上传失败，服务器无响应或发生未知错误。');
    }
};

/**
 * 上传文件之前的钩子，用于客户端验证
 */
const beforeUpload: UploadProps['beforeUpload'] = (rawFile) => {
    if (!rawFile.name.endsWith('.dll')) {
        ElMessage.error('只允许上传 .dll 格式的插件文件。');
        return false;
    }
    if (rawFile.size / 1024 / 1024 > 20) { // 限制大小为 20MB
        ElMessage.error('插件文件大小不能超过 20MB！');
        return false;
    }
    return true;
};

onMounted(fetchPlugins);</script>

<style scoped>
  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .plugin-detail-content {
    padding: 20px 0;
  }

  .dialog-footer {
    text-align: right;
  }
</style>
