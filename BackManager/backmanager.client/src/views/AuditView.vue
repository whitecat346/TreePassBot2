<template>
  <div class="audit-view-container">
    <!-- 页面标题 -->
    <h1 class="text-2xl font-bold text-gray-900 mb-6">审核记录</h1>

    <!-- 筛选区域 -->
    <el-card shadow="never" class="mb-6">
      <div class="filter-container flex flex-wrap gap-4 items-center">
        <div class="flex items-center space-x-2">
          <span class="text-sm font-medium text-gray-700">状态：</span>
          <el-select
            v-model="filterStatus"
            placeholder="全部"
            clearable
            size="small"
            class="w-40"
          >
            <el-option label="待审核" value="Pending" />
            <el-option label="已通过" value="Approved" />
            <el-option label="已拒绝" value="Rejected" />
          </el-select>
        </div>

        <div class="flex items-center space-x-2">
          <span class="text-sm font-medium text-gray-700">入群状态：</span>
          <el-select
            v-model="filterEnteredGroup"
            placeholder="全部"
            clearable
            size="small"
            class="w-40"
          >
            <el-option label="已入群" :value="true" />
            <el-option label="未入群" :value="false" />
          </el-select>
        </div>

        <div class="flex items-center space-x-2">
          <span class="text-sm font-medium text-gray-700">搜索：</span>
          <el-input
            v-model="filterKeyword"
            placeholder="用户名/用户ID"
            clearable
            size="small"
            prefix-icon="Search"
            class="w-64"
          />
        </div>

        <div class="flex items-center space-x-2">
          <el-button type="primary" size="small" @click="handleSearch">
            搜索
          </el-button>
          <el-button size="small" @click="handleReset">
            重置
          </el-button>
        </div>
      </div>
    </el-card>

    <!-- 审核列表 -->
    <el-card shadow="never">
      <el-table
        v-loading="loading"
        :data="filteredAudits"
        style="width: 100%"
        border
        stripe
        :header-cell-style="{ background: '#fafafa' }"
      >
        <!-- 用户信息列 -->
        <el-table-column label="用户信息" min-width="200">
          <template #default="{ row }">
            <div class="flex items-center">
              <Avatar :src="null" :username="row.username" size="sm" rounded class="mr-3" />
              <div>
                <div class="font-medium">{{ row.username }}</div>
                <div class="text-xs text-gray-500">{{ row.userId }}</div>
              </div>
            </div>
          </template>
        </el-table-column>

        <!-- 群组信息列 -->
        <el-table-column prop="groupName" label="群组" width="180" />

        <!-- 审核状态列 -->
        <el-table-column label="审核状态" width="120" align="center">
          <template #default="{ row }">
            <StatusBadge
              :status="getAuditStatus(row.status)"
              :text="getAuditStatusText(row.status)"
            />
          </template>
        </el-table-column>

        <!-- 验证码列 -->
        <el-table-column label="验证码" width="120" align="center">
          <template #default="{ row }">
            <span v-if="row.verificationCode" class="font-mono">{{ row.verificationCode }}</span>
            <span v-else class="text-gray-400">-</span>
          </template>
        </el-table-column>

        <!-- 入群状态列 -->
        <el-table-column label="入群状态" width="120" align="center">
          <template #default="{ row }">
            <el-switch
              v-model="row.enteredGroup"
              :disabled="true"
              active-text="已入群"
              inactive-text="未入群"
            />
          </template>
        </el-table-column>

        <!-- 创建时间列 -->
        <el-table-column prop="createdAt" label="申请时间" width="180" align="center">
          <template #default="{ row }">
            {{ new Date(row.createdAt).toLocaleString() }}
          </template>
        </el-table-column>

        <!-- 处理时间列 -->
        <el-table-column label="处理时间" width="180" align="center">
          <template #default="{ row }">
            {{ row.processedAt ? new Date(row.processedAt).toLocaleString() : '-' }}
          </template>
        </el-table-column>

        <!-- 处理者列 -->
        <el-table-column prop="processedBy" label="处理者" width="120" align="center" />

        <!-- 操作列 -->
        <el-table-column label="操作" width="180" align="center">
          <template #default="{ row }">
            <div class="flex space-x-2">
              <!-- 审核通过按钮 -->
              <el-button
                v-if="row.status === 'Pending'"
                type="success"
                size="small"
                @click="handleApprove(row)"
              >
                通过
              </el-button>

              <!-- 拒绝审核按钮 -->
              <el-button
                v-if="row.status === 'Pending'"
                type="danger"
                size="small"
                @click="handleReject(row)"
              >
                拒绝
              </el-button>

              <!-- 查看详情按钮 -->
              <el-button
                type="primary"
                size="small"
                @click="handleViewDetail(row)"
              >
                详情
              </el-button>
            </div>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <div v-if="auditTotal > 0" class="p-4 border-t border-gray-200">
        <el-pagination
          v-model:current-page="currentPage"
          v-model:page-size="pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="auditTotal"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handlePageSizeChange"
          @current-change="handlePageChange"
        />
      </div>

      <!-- 空状态 -->
      <div v-if="!loading && filteredAudits.length === 0" class="p-12">
        <EmptyState
          title="暂无审核记录"
          description="当前没有审核记录，或搜索条件过于严格"
          icon="FileText"
        />
      </div>
    </el-card>

    <!-- 审核详情弹窗 -->
    <el-dialog
      v-model="detailVisible"
      title="审核详情"
      width="500px"
      :before-close="handleCloseDetail"
    >
      <div v-if="selectedAudit" class="audit-detail-content">
        <el-descriptions :column="1" border>
          <el-descriptions-item label="用户ID">
            {{ selectedAudit.userId }}
          </el-descriptions-item>
          <el-descriptions-item label="用户名">
            <div class="flex items-center">
              <Avatar :src="null" :username="selectedAudit.username" size="sm" rounded class="mr-2" />
              {{ selectedAudit.username }}
            </div>
          </el-descriptions-item>
          <el-descriptions-item label="申请群组">
            {{ selectedAudit.groupName }}
          </el-descriptions-item>
          <el-descriptions-item label="审核状态">
            <StatusBadge
              :status="getAuditStatus(selectedAudit.status)"
              :text="getAuditStatusText(selectedAudit.status)"
            />
          </el-descriptions-item>
          <el-descriptions-item label="验证码">
            <span v-if="selectedAudit.verificationCode" class="font-mono">{{ selectedAudit.verificationCode }}</span>
            <span v-else class="text-gray-400">-</span>
          </el-descriptions-item>
          <el-descriptions-item label="入群状态">
            <el-tag :type="selectedAudit.enteredGroup ? 'success' : 'info'">
              {{ selectedAudit.enteredGroup ? '已入群' : '未入群' }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="申请时间">
            {{ new Date(selectedAudit.createdAt).toLocaleString() }}
          </el-descriptions-item>
          <el-descriptions-item label="处理时间">
            {{ selectedAudit.processedAt ? new Date(selectedAudit.processedAt).toLocaleString() : '-' }}
          </el-descriptions-item>
          <el-descriptions-item label="处理者">
            {{ selectedAudit.processedBy || '-' }}
          </el-descriptions-item>
        </el-descriptions>
      </div>

      <template #footer>
        <span class="dialog-footer">
          <el-button @click="handleCloseDetail">关闭</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import {
  getAuditRecords,
  approveAudit,
  rejectAudit,
  type AuditRecord,
  type AuditStatus
} from '@/services/api';
import { ElMessage } from 'element-plus';

// 导入通用组件
import Avatar from '../components/common/BaseAvatar.vue';
import StatusBadge from '../components/common/StatusBadge.vue';
import EmptyState from '../components/common/EmptyState.vue';

// 响应式数据
const audits = ref<AuditRecord[]>([]);
const loading = ref(true);
const currentPage = ref(1);
const pageSize = ref(20);
const auditTotal = ref(0);

// 筛选条件
const filterStatus = ref<AuditStatus | ''>('');
const filterEnteredGroup = ref<boolean | ''>('');
const filterKeyword = ref('');

// 弹窗控制
const detailVisible = ref(false);
const selectedAudit = ref<AuditRecord | null>(null);

// 过滤后的审核列表
const filteredAudits = computed(() => {
  return audits.value.filter(audit => {
    // 状态筛选
    if (filterStatus.value && audit.status !== filterStatus.value) {
      return false;
    }

    // 入群状态筛选
    if (filterEnteredGroup.value !== '' && audit.enteredGroup !== filterEnteredGroup.value) {
      return false;
    }

    // 关键词筛选
    if (filterKeyword.value) {
      const keyword = filterKeyword.value.toLowerCase();
      return (
        audit.username.toLowerCase().includes(keyword) ||
        audit.userId.includes(keyword)
      );
    }

    return true;
  });
});

// 根据审核状态获取状态类型
const getAuditStatus = (status: AuditStatus) => {
  switch (status) {
    case 'Approved':
      return 'success';
    case 'Rejected':
      return 'error';
    case 'Pending':
      return 'pending';
    default:
      return 'info';
  }
};

// 根据审核状态获取显示文本
const getAuditStatusText = (status: AuditStatus) => {
  switch (status) {
    case 'Approved':
      return '已通过';
    case 'Rejected':
      return '已拒绝';
    case 'Pending':
      return '待审核';
    default:
      return '未知';
  }
};

// 获取审核记录列表
const fetchAuditRecords = async () => {
  try {
    loading.value = true;

    // 注意：这里的API调用方式需要根据实际后端接口调整
    // 实际调用时需要传递筛选和分页参数
    // const params = {
    //   status: filterStatus.value,
    //   enteredGroup: filterEnteredGroup.value,
    //   keyword: filterKeyword.value,
    //   page: currentPage.value,
    //   pageSize: pageSize.value
    // };

    const response = await getAuditRecords();
    if (response.data.success) {
      audits.value = response.data.data;
      auditTotal.value = response.data.data.length; // 实际项目中应该从API返回的total字段获取
    }
  } catch (error) {
    ElMessage.error('获取审核记录失败');
    console.error('获取审核记录失败:', error);
  } finally {
    loading.value = false;
  }
};

// 处理搜索
const handleSearch = () => {
  currentPage.value = 1;
  fetchAuditRecords();
};

// 处理重置
const handleReset = () => {
  filterStatus.value = '';
  filterEnteredGroup.value = '';
  filterKeyword.value = '';
  currentPage.value = 1;
  fetchAuditRecords();
};

// 处理审核通过
const handleApprove = async (audit: AuditRecord) => {
  try {
    const response = await approveAudit(audit.id);
    if (response.data.success) {
      ElMessage.success('审核通过成功');
      fetchAuditRecords(); // 刷新列表
    }
  } catch (error) {
    ElMessage.error('审核通过失败');
    console.error('审核通过失败:', error);
  }
};

// 处理审核拒绝
const handleReject = async (audit: AuditRecord) => {
  try {
    const response = await rejectAudit(audit.id);
    if (response.data.success) {
      ElMessage.success('拒绝审核成功');
      fetchAuditRecords(); // 刷新列表
    }
  } catch (error) {
    ElMessage.error('拒绝审核失败');
    console.error('拒绝审核失败:', error);
  }
};

// 查看审核详情
const handleViewDetail = (audit: AuditRecord) => {
  selectedAudit.value = audit;
  detailVisible.value = true;
};

// 关闭详情弹窗
const handleCloseDetail = () => {
  detailVisible.value = false;
  // 延迟清空数据，避免弹窗关闭时的闪烁
  setTimeout(() => {
    selectedAudit.value = null;
  }, 300);
};

// 处理分页大小变化
const handlePageSizeChange = (size: number) => {
  pageSize.value = size;
  currentPage.value = 1;
  fetchAuditRecords();
};

// 处理页码变化
const handlePageChange = (page: number) => {
  currentPage.value = page;
  fetchAuditRecords();
};

// 组件挂载时初始化
onMounted(() => {
  fetchAuditRecords();
});
</script>

<style scoped>
.audit-view-container {
  width: 100%;
}

.filter-container {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
  align-items: center;
}

.audit-detail-content {
  padding: 20px 0;
}

.dialog-footer {
  text-align: right;
}
</style>
