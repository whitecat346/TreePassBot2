import { createRouter, createWebHistory } from 'vue-router'
import DashboardView from '../views/DashboardView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'dashboard',
      component: DashboardView
    },
    {
      path: '/plugins',
      name: 'plugins',
      component: () => import('../views/PluginManagementView.vue')
    }
    // {
    //   path: '/messages',
    //   name: 'messages',
    //   // ... 消息查看组件
    //   component: () => import('../views/MessageView.vue')
    // },
    // {
    //   path: '/audits',
    //   name: 'audits',
    //   // ... 审核信息查看组件
    //   component: () => import('../views/AuditView.vue')
    // }
  ]
})

export default router
