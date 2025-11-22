import { createRouter, createWebHistory } from 'vue-router';
import Users from '../views/Users.vue';
import Plugins from '../views/Plugins.vue';

// 简单的 Dashboard 占位
const Dashboard = { template: '<div class="text-2xl font-bold">Dashboard Component (Pending)</div>' };

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', component: Dashboard },
    { path: '/users', component: Users },
    { path: '/plugins', component: Plugins },
    // 其他路由...
  ]
});

export default router;
