import { createApp } from 'vue';
import { createPinia } from 'pinia';
import PrimeVue from 'primevue/config';
import Aura from '@primevue/themes/aura'; // PrimeVue v4 新主题
import App from './App.vue';
import router from './router/index.ts';
import './style.css'; // Tailwind

const app = createApp(App);

app.use(createPinia());
app.use(router);
app.use(PrimeVue, {
  theme: {
    preset: Aura,
    options: {
      darkModeSelector: '.fake-dark-mode', // 强制亮色，除非手动切换
    }
  }
});

app.mount('#app');
