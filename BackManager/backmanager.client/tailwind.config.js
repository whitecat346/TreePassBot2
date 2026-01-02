/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
    // 明确列出所有包含响应式类的文件
    "./src/App.vue",
    "./src/components/**/*.vue",
    "./src/views/**/*.vue",
    "./src/components/Sidebar.vue",
    "./src/components/Header.vue",
    "./src/components/common/BaseTable.vue",
  ],
  // 明确配置所有断点，确保生产构建生成所有响应式类
  theme: {
    screens: {
      'sm': '640px',
      'md': '768px',
      'lg': '1024px',
      'xl': '1280px',
      '2xl': '1536px',
    },
    extend: {
      fontFamily: {
        sans: ['Inter', 'ui-sans-serif', 'system-ui', 'sans-serif'],
      },
    },
  },
  // 确保所有响应式类都被生成
  safelist: [
    // 明确列出项目中实际使用的响应式类
    'sm:flex', 'md:flex', 'lg:flex', 'xl:flex',
    'sm:block', 'md:block', 'lg:block', 'xl:block',
    'sm:hidden', 'md:hidden', 'lg:hidden', 'xl:hidden',
    'sm:grid', 'md:grid', 'lg:grid', 'xl:grid',
    'sm:w-full', 'md:w-full', 'lg:w-full', 'xl:w-full',
    'sm:w-1/2', 'md:w-1/2', 'lg:w-1/2', 'xl:w-1/2',
    'sm:w-1/3', 'md:w-1/3', 'lg:w-1/3', 'xl:w-1/3',
    'sm:p-4', 'md:p-4', 'lg:p-4', 'xl:p-4',
    'sm:p-6', 'md:p-6', 'lg:p-6', 'xl:p-6',
    'sm:text-sm', 'md:text-sm', 'lg:text-sm', 'xl:text-sm',
    'sm:text-base', 'md:text-base', 'lg:text-base', 'xl:text-base',
    'sm:text-lg', 'md:text-lg', 'lg:text-lg', 'xl:text-lg',
    'sm:gap-3', 'md:gap-3', 'lg:gap-3', 'xl:gap-3',
    'sm:gap-4', 'md:gap-4', 'lg:gap-4', 'xl:gap-4',
    'sm:gap-6', 'md:gap-6', 'lg:gap-6', 'xl:gap-6',
    'sm:justify-between', 'md:justify-between', 'lg:justify-between', 'xl:justify-between',
    'sm:items-center', 'md:items-center', 'lg:items-center', 'xl:items-center',
    'sm:items-start', 'md:items-start', 'lg:items-start', 'xl:items-start',
    'sm:mx-12', 'md:mx-12', 'lg:mx-12', 'xl:mx-12',
    'sm:mx-auto', 'md:mx-auto', 'lg:mx-auto', 'xl:mx-auto',
    'sm:max-w-lg', 'md:max-w-lg', 'lg:max-w-lg', 'xl:max-w-lg',
    'sm:max-w-xl', 'md:max-w-xl', 'lg:max-w-xl', 'xl:max-w-xl',
    'sm:w-40', 'md:w-40', 'lg:w-40', 'xl:w-40',
    'sm:w-64', 'md:w-64', 'lg:w-64', 'xl:w-64',
    'sm:w-80', 'md:w-80', 'lg:w-80', 'xl:w-80',
    // 针对表格组件特别添加的响应式类
    'sm:px-6', 'md:px-6', 'lg:px-6', 'xl:px-6',
    'sm:py-4', 'md:py-4', 'lg:py-4', 'xl:py-4',
    'sm:h-8', 'md:h-8', 'lg:h-8', 'xl:h-8',
    'sm:w-8', 'md:w-8', 'lg:w-8', 'xl:w-8',
    // 针对侧边栏组件特别添加的响应式类
    'sm:translate-x-0', 'md:translate-x-0', 'lg:translate-x-0', 'xl:translate-x-0',
    'sm:fixed', 'md:fixed', 'lg:fixed', 'xl:fixed',
    'sm:inset-y-0', 'md:inset-y-0', 'lg:inset-y-0', 'xl:inset-y-0',
    'sm:left-0', 'md:left-0', 'lg:left-0', 'xl:left-0',
    'sm:z-50', 'md:z-50', 'lg:z-50', 'xl:z-50',
  ],
  plugins: []
}
