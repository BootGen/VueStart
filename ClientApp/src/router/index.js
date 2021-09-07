import { createRouter, createWebHistory } from 'vue-router'
import Editor from '../views/Editor.vue'
import LandingPage from '../views/LandingPage.vue'


  const routes = [
  {
    path: '/',
    name: 'Landing Page',
    component: LandingPage
  },
  {
    path: '/editor',
    name: 'Editor',
    component: Editor
  }
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

router.beforeEach((to, from, next) => {
  document.title = `StartVue - ${to.name}`;
  next();
});

export default router