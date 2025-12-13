import { createRouter, createWebHistory } from 'vue-router'
import DashboardView from '../views/DashboardView.vue'
import UserManagementView from '../views/UserManagementView.vue'
import AuditView from '../views/AuditView.vue'
import PluginManagementView from '../views/PluginManagementView.vue'
import MessageLogView from '../views/MessageLogView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'dashboard',
      component: DashboardView,
      meta: {
        title: '仪表盘',
        icon: 'LayoutDashboard'
      }
    },
    {
      path: '/users',
      name: 'users',
      component: UserManagementView,
      meta: {
        title: '用户管理',
        icon: 'Users'
      }
    },
    {
      path: '/audits',
      name: 'audits',
      component: AuditView,
      meta: {
        title: '审核记录',
        icon: 'FileText'
      }
    },
    {
      path: '/plugins',
      name: 'plugins',
      component: PluginManagementView,
      meta: {
        title: '插件中心',
        icon: 'Plug'
      }
    },
    {
      path: '/logs',
      name: 'logs',
      component: MessageLogView,
      meta: {
        title: '消息日志',
        icon: 'Database'
      }
    }
  ]
})

export default router
