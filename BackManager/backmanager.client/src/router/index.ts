import { createRouter, createWebHistory } from 'vue-router'
import DashboardView from '../views/DashboardView.vue'

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
      component: () => import('../views/UserManagementView.vue'),
      meta: {
        title: '用户管理',
        icon: 'Users'
      }
    },
    {
      path: '/audits',
      name: 'audits',
      component: () => import('../views/AuditView.vue'),
      meta: {
        title: '审核记录',
        icon: 'FileText'
      }
    },
    {
      path: '/plugins',
      name: 'plugins',
      component: () => import('../views/PluginManagementView.vue'),
      meta: {
        title: '插件中心',
        icon: 'Plug'
      }
    },
    {
      path: '/logs',
      name: 'logs',
      component: () => import('../views/MessageLogView.vue'),
      meta: {
        title: '消息日志',
        icon: 'Database'
      }
    }
  ]
})

export default router
