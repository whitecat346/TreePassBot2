<script setup>
import { ref, onMounted } from 'vue';
import { Play, Square, AlertTriangle, Settings } from 'lucide-vue-next';
import axios from 'axios';

const plugins = ref([]);

onMounted(async () => {
  try {
    const response = await axios.get('/api/plugins');
    plugins.value = response.data;
  } catch (error) {
    console.error('Failed to fetch plugins:', error);
  }
})

</script>

<template>
  <div>
    <div class="mb-6">
      <h1 class="text-2xl font-bold text-gray-800">插件中心</h1>
      <p class="text-gray-500 text-sm mt-1">管理机器人的功能扩展模块 (AssemblyLoadContext 隔离环境)</p>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
      <div v-for="plugin in plugins" :key="plugin.id"
        class="bg-white rounded-xl border border-gray-200 p-5 shadow-sm hover:shadow-md transition-all relative overflow-hidden group">

        <!-- Status Indicator Strip -->
        <div class="absolute top-0 left-0 w-1 h-full"
          :class="plugin.status === 'Running' ? 'bg-green-500' : plugin.status === 'Crashed' ? 'bg-red-500' : 'bg-gray-300'">
        </div>

        <div class="flex justify-between items-start mb-3 pl-2">
          <div class="flex items-center">
            <div class="w-10 h-10 rounded-lg bg-indigo-50 text-indigo-600 flex items-center justify-center mr-3">
              <span class="font-bold text-lg">{{ plugin.name[0] }}</span>
            </div>
            <div>
              <h3 class="font-bold text-gray-800">{{ plugin.name }}</h3>
              <p class="text-xs text-gray-500 font-mono">{{ plugin.version }} by {{ plugin.author }}</p>
            </div>
          </div>

          <!-- Status Badge -->
          <span class="px-2 py-1 rounded text-xs font-bold border"
            :class="plugin.status === 'Running' ? 'bg-green-50 text-green-700 border-green-200' :
                    plugin.status === 'Crashed' ? 'bg-red-50 text-red-700 border-red-200' : 'bg-gray-50 text-gray-600 border-gray-200'">
            {{ plugin.status }}
          </span>
        </div>

        <p class="text-sm text-gray-600 mb-4 pl-2 line-clamp-2 h-10">
          {{ plugin.description }}
        </p>

        <div class="flex items-center justify-between pt-4 border-t border-gray-100 pl-2">
          <button class="text-gray-400 hover:text-indigo-600 transition-colors">
            <Settings class="w-4 h-4" />
          </button>

          <div class="flex space-x-2">
            <button v-if="plugin.status === 'Running'"
              class="flex items-center px-3 py-1.5 text-xs font-medium text-red-600 bg-red-50 border border-red-100 rounded-lg hover:bg-red-100 transition-colors">
              <Square class="w-3 h-3 mr-1.5 fill-current" /> 停止
            </button>
            <button v-else
              class="flex items-center px-3 py-1.5 text-xs font-medium text-green-600 bg-green-50 border border-green-100 rounded-lg hover:bg-green-100 transition-colors">
              <Play class="w-3 h-3 mr-1.5 fill-current" /> 启动
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
