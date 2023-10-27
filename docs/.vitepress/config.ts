import { defineConfig } from 'vitepress'
import { getApiSidebar } from './apitoc'

// https://vitepress.vuejs.org/config/app-configs
export default defineConfig({
  lang: 'zh-CN',
  title: 'Satori.NET',
  description: 'Satori 协议 .NET SDK',
  base: '/Satori.NET/',

  themeConfig: {
    sidebar: {
      '/api/': [{ text: 'API Reference', items: getApiSidebar() }],
    },
    nav: [
      { text: '主页', link: '/' },
      { text: 'API', link: '/api/' },
    ],
    socialLinks: [
      { icon: 'github', link: 'https://github.com/bsdayo/Satori.NET' },
    ],
  },
})
