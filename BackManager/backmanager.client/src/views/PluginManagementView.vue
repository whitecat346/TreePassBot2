<template>
    <el-card shadow="never">
        <template #header>
            <div class="card-header">
                <span>插件管理</span>
                <!-- 新增：上传插件组件 -->
                <el-upload
                    action="http://localhost:5000/api/plugins/upload"
                    name="pluginFile"
                    :show-file-list="false"
                    :on-success="handleUploadSuccess"
                    :on-error="handleUploadError"
                    :before-upload="beforeUpload"
                    accept=".dll"
                >
                    <el-button type="primary">上传新插件</el-button>
                </el-upload>
            </div>
        </template>

        <!-- 表格部分与之前相同 -->
        <el-table :data="plugins" v-loading="loading" style="width: 100%">
            <el-table-column prop="name" label="插件名称" width="220" />
            <el-table-column prop="description" label="功能描述" />
            <el-table-column label="状态" width="120">
                <template #default="{ row }">
                    <el-switch
                        v-model="row.isEnabled"
                        @change="handleToggle(row)"
                    />
                </template>
            </el-table-column>
        </el-table>
    </el-card>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { getPlugins, togglePlugin, type Plugin } from '@services/api'
import { ElMessage, type UploadProps } from 'element-plus';

const loading = ref(true);
const plugins = ref<Plugin[]>([]);

const fetchPlugins = async () => {
    try {
        loading.value = true;
        const response = await getPlugins();
        plugins.value = response.data;
    } catch (error) {
        ElMessage.error('获取插件列表失败');
    } finally {
        loading.value = false;
    }
};

const handleToggle = async (plugin: Plugin) => {
    try {
        await togglePlugin(plugin.id);
        ElMessage.success(`插件 '${plugin.name}' 状态已更新`);
    } catch (error) {
        // 如果API调用失败，则将开关恢复到原始状态
        plugin.isEnabled = !plugin.isEnabled;
        ElMessage.error('更新插件状态失败');
    }
};

/**
 * 上传成功后的回调
 */
const handleUploadSuccess = (response: any, uploadFile: any) => {
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

onMounted(fetchPlugins);
</script>

<style scoped>
.card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
}
</style>
