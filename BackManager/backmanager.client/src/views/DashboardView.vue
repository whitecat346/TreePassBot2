<template>
    <el-card shadow="never">
        <template #header>
            <div class="card-header">
                <span>机器人状态</span>
            </div>
        </template>
        <div v-if="loading" v-loading="loading" style="height: 100px;"></div>
        <div v-else>
            <el-descriptions :column="2" border>
                <el-descriptions-item label="当前状态">
                    <el-tag :type="status?.status === 'Running' ? 'success' : 'danger'">
                        {{ status?.status }}
                    </el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="启动时间">
                    {{ status?.startTime ? new Date(status.startTime).toLocaleString() : 'N/A' }}
                </el-descriptions-item>
            </el-descriptions>
            <div style="margin-top: 20px;">
                <el-button type="primary" @click="handleStart" :disabled="status?.status === 'Running'">启动</el-button>
                <el-button type="danger" @click="handleStop" :disabled="status?.status !== 'Running'">停止</el-button>
            </div>
        </div>
    </el-card>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { getBotStatus, startBot, stopBot, type BotStatus } from '@services/api';
import { ElMessage } from 'element-plus';

const loading = ref(true);
const status = ref<BotStatus | null>(null);

const fetchStatus = async () => {
    try {
        loading.value = true;
        const response = await getBotStatus();
        status.value = response.data;
    } catch (error) {
        ElMessage.error('获取机器人状态失败');
    } finally {
        loading.value = false;
    }
};

const handleStart = async () => {
    try {
        await startBot();
        ElMessage.success('机器人启动成功');
        fetchStatus();
    } catch (error) {
        ElMessage.error('启动失败');
    }
};

const handleStop = async () => {
    try {
        await stopBot();
        ElMessage.success('机器人已停止');
        fetchStatus();
    } catch (error) {
        ElMessage.error('停止失败');
    }
};

onMounted(fetchStatus);
</script>
