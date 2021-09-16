import { createRouter, createWebHistory } from 'vue-router';
import LandingPage from '../views/LandingPage.vue';

  const routes = [
  {
    path: '/',
    name: 'Landing Page',
    component: LandingPage
  }
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

router.beforeEach((to, from, next) => {
  document.title = `StartVue`;
  next();
});

export default router