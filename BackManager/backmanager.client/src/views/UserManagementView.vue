<template>
  <div class="user-management-container">
    <!-- 页面标题 -->
    <h1 class="text-2xl font-bold text-gray-900 mb-6">用户管理</h1>

    <!-- 主内容区域 -->
    <div class="flex flex-col md:flex-row gap-6">
      <!-- 左侧群组列表 -->
      <div class="md:w-1/4 bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
        <div class="p-4 border-b border-gray-200">
          <h3 class="text-lg font-medium text-gray-900">群组列表</h3>
          <div class="mt-3">
            <el-input v-model="groupSearchKeyword"
                      placeholder="搜索群组"
                      clearable
                      size="small"
                      prefix-icon="Search"
                      class="w-full" />
          </div>
        </div>

        <div class="overflow-y-auto max-h-[calc(100vh-200px)]">
          <el-tree v-if="!groupLoading"
                   :data="filteredGroups"
                   :props="groupTreeProps"
                   :default-expand-all="true"
                   :expand-on-click-node="false"
                   :filter-node-method="filterGroupNode"
                   @node-click="handleGroupClick"
                   class="p-2">
            <template #default="{ data }">
              <div class="flex items-center justify-between cursor-pointer px-2 py-1 rounded hover:bg-gray-50 transition-colors">
                <div class="flex items-center">
                  <el-icon class="mr-2 text-gray-400"><ChatDotRound /></el-icon>
                  <span>{{ data.name }}</span>
                  <el-tag v-if="data.memberCount"
                          size="small"
                          class="ml-2">
                    {{ data.memberCount }}
                  </el-tag>
                </div>
              </div>
            </template>
          </el-tree>

          <div v-else class="p-4 text-center">
            <el-skeleton :rows="5" animated />
          </div>
        </div>
      </div>

      <!-- 右侧成员列表 -->
      <div class="flex-1 bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
        <div class="p-4 border-b border-gray-200 flex justify-between items-center">
          <h3 class="text-lg font-medium text-gray-900">
            {{ selectedGroup ? `${selectedGroup.name} - 成员列表` : '请选择一个群组' }}
          </h3>

          <div v-if="selectedGroup" class="flex items-center space-x-3">
            <el-input v-model="memberSearchKeyword"
                      placeholder="搜索成员"
                      clearable
                      size="small"
                      prefix-icon="Search"
                      class="w-64" />
          </div>
        </div>

        <!-- 成员列表 -->
        <div v-if="selectedGroup">
          <el-table v-loading="memberLoading"
                    :data="filteredMembers"
                    style="width: 100%"
                    border
                    stripe
                    :header-cell-style="{ background: '#fafafa' }">
            <!-- 用户名列 -->
            <el-table-column prop="username" label="用户名" min-width="120">
              <template #default="{ row }">
                <div class="flex items-center">
                  <span>{{ row.username }}</span>
                  <el-tag v-if="row.nickname && row.nickname !== row.username"
                          size="small"
                          type="info"
                          class="ml-2">
                    {{ row.nickname }}
                  </el-tag>
                </div>
              </template>
            </el-table-column>

            <!-- 身份列 -->
            <el-table-column label="身份" width="120" align="center">
              <template #default="{ row }">
                <StatusBadge :status="getRoleStatus(row.role)"
                             :text="getRoleText(row.role)" />
              </template>
            </el-table-column>

            <!-- 加入时间列 -->
            <el-table-column prop="joinedAt" label="加入时间" width="180" align="center">
              <template #default="{ row }">
                {{ new Date(row.joinedAt).toLocaleString() }}
              </template>
            </el-table-column>

            <!-- 操作列 -->
            <el-table-column label="操作" width="120" align="center">
              <template #default="{ row }">
                <el-button type="primary"
                           size="small"
                           @click="handleViewMember(row)">
                  查看详情
                </el-button>
              </template>
            </el-table-column>
          </el-table>

          <!-- 分页 -->
          <div v-if="memberTotal > 0" class="p-4 border-t border-gray-200">
            <el-pagination v-model:current-page="memberCurrentPage"
                           v-model:page-size="memberPageSize"
                           :page-sizes="[10, 20, 50, 100]"
                           :total="memberTotal"
                           layout="total, sizes, prev, pager, next, jumper"
                           @size-change="handleMemberPageSizeChange"
                           @current-change="handleMemberPageChange" />
          </div>
        </div>

        <!-- 未选择群组时的提示 -->
        <div v-else class="p-12 text-center">
          <EmptyState title="未选择群组"
                      description="请从左侧选择一个群组查看成员信息"
                      icon="Users" />
        </div>
      </div>
    </div>

    <!-- 成员详情弹窗 -->
    <el-dialog v-model="memberDetailVisible"
               title="成员详情"
               width="500px"
               :before-close="handleCloseDetail">
      <div v-if="selectedMember" class="member-detail-content">
        <el-descriptions :column="1" border>
          <el-descriptions-item label="用户ID">
            {{ selectedMember.userId }}
          </el-descriptions-item>
          <el-descriptions-item label="用户名">
            {{ selectedMember.username }}
          </el-descriptions-item>
          <el-descriptions-item label="群昵称">
            {{ selectedMember.nickname || '无' }}
          </el-descriptions-item>
          <el-descriptions-item label="身份">
            <StatusBadge :status="getRoleStatus(selectedMember.role)"
                         :text="getRoleText(selectedMember.role)" />
          </el-descriptions-item>
          <el-descriptions-item label="加入时间">
            {{ new Date(selectedMember.joinedAt).toLocaleString() }}
          </el-descriptions-item>
          <el-descriptions-item label="所在群组">
            {{ selectedGroup?.name || '无' }}
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

<script setup lang="ts">defineOptions({
  name: 'UserManagementView'
});
import { ref, computed, onMounted } from 'vue';
import {
  getGroups,
  getGroupMembers,
  type Group,
  type GroupMember
} from '@/services/api';
import { ElMessage } from 'element-plus';
import { ChatDotRound } from '@element-plus/icons-vue';

// 导入通用组件
import StatusBadge from '../components/common/StatusBadge.vue';
import EmptyState from '../components/common/EmptyState.vue';

// 响应式数据
const groups = ref<Group[]>([]);
const members = ref<GroupMember[]>([]);
const groupLoading = ref(true);
const memberLoading = ref(false);
const selectedGroup = ref<Group | null>(null);
const selectedMember = ref<GroupMember | null>(null);

// 搜索和分页
const groupSearchKeyword = ref('');
const memberSearchKeyword = ref('');
const memberCurrentPage = ref(1);
const memberPageSize = ref(20);
const memberTotal = ref(0);

// 弹窗控制
const memberDetailVisible = ref(false);

// 群组树配置
const groupTreeProps = {
  label: 'name',
  children: null
};

// 过滤后的群组列表
const filteredGroups = computed(() => {
  if (!groupSearchKeyword.value) return groups.value;

  return groups.value.filter(group => {
    return group.name.toLowerCase().includes(groupSearchKeyword.value.toLowerCase());
  });
});

// 过滤后的成员列表
const filteredMembers = computed(() => {
  if (!memberSearchKeyword.value) return members.value;

  return members.value.filter(member => {
    const keyword = memberSearchKeyword.value.toLowerCase();
    return (
      member.username.toLowerCase().includes(keyword) ||
      member.nickname?.toLowerCase().includes(keyword) ||
      member.userId.includes(keyword)
    );
  });
});

// 群组节点过滤方法
const filterGroupNode = (value: string, data: Group) => {
  return data.name.toLowerCase().includes(value.toLowerCase());
};

// 根据成员角色获取状态类型
const getRoleStatus = (role: string) => {
  switch (role) {
    case 'Owner':
      return 'success';
    case 'Admin':
      return 'warning';
    case 'Bot':
      return 'info';
    default:
      return 'pending';
  }
};

// 根据成员角色获取显示文本
const getRoleText = (role: string) => {
  switch (role) {
    case 'Owner':
      return '群主';
    case 'Admin':
      return '管理员';
    case 'Bot':
      return '机器人';
    default:
      return '普通成员';
  }
};

// 获取群组列表
const fetchGroups = async () => {
  try {
    groupLoading.value = true;
    const response = await getGroups();
    if (response.data.success) {
      groups.value = response.data.data;
      // 默认选择第一个群组
      if (groups.value.length > 0) {
        selectedGroup.value = groups.value[0] || null;
        if (selectedGroup.value) {
          fetchGroupMembers(selectedGroup.value.groupId);
        }
      }
    }
  } catch (error) {
    ElMessage.error('获取群组列表失败');
    console.error('获取群组列表失败:', error);
  } finally {
    groupLoading.value = false;
  }
};

// 获取群组成员列表
const fetchGroupMembers = async (groupId: string) => {
  try {
    memberLoading.value = true;
    memberCurrentPage.value = 1;

    // 实际调用时需要传递分页参数
    const response = await getGroupMembers(groupId);
    if (response.data.success) {
      members.value = response.data.data;
      memberTotal.value = response.data.data.length; // 实际项目中应该从API返回的total字段获取
    }
  } catch (error) {
    ElMessage.error('获取成员列表失败');
    console.error('获取成员列表失败:', error);
  } finally {
    memberLoading.value = false;
  }
};

// 处理群组点击
const handleGroupClick = (data: Group) => {
  selectedGroup.value = data;
  fetchGroupMembers(data.groupId);
};

// 处理成员分页大小变化
const handleMemberPageSizeChange = (size: number) => {
  memberPageSize.value = size;
  memberCurrentPage.value = 1;
  if (selectedGroup.value) {
    fetchGroupMembers(selectedGroup.value.groupId);
  }
};

// 处理成员页码变化
const handleMemberPageChange = (page: number) => {
  memberCurrentPage.value = page;
  if (selectedGroup.value) {
    fetchGroupMembers(selectedGroup.value.groupId);
  }
};

// 查看成员详情
const handleViewMember = (member: GroupMember) => {
  selectedMember.value = member;
  memberDetailVisible.value = true;
};

// 关闭成员详情弹窗
const handleCloseDetail = () => {
  memberDetailVisible.value = false;
  // 延迟清空数据，避免弹窗关闭时的闪烁
  setTimeout(() => {
    selectedMember.value = null;
  }, 300);
};

// 组件挂载时初始化
onMounted(() => {
  fetchGroups();
});</script>

<style scoped>
  .user-management-container {
    width: 100%;
  }

  .member-detail-content {
    padding: 10px 0;
  }

  .dialog-footer {
    text-align: right;
  }
</style>
