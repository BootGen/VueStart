<template>
  <div class="d-flex justify-content-center align-items-center">
    <div class="jumbo-text-full">
      <div class="d-flex align-items-center justify-content-center">
        <img class="vue_logo" alt="vue" :src="require(`../assets/vue_logo.webp`)">
        <p class="title">ue Start!</p>
        <img class="vuecoon img-fluid px-3" alt="Vuecoon" :src="require(`../assets/vuecoon_${vuecoonState}.webp`)">
      </div>
      <p class="lead text-center">
        An online tool that generates UI components for Vue.js developers. Input some JSON data, chose a template, download the code and use it in any project.
      </p>
    </div>
  </div>
</template>


<script>
import { defineComponent } from 'vue';
export default defineComponent({
  name: 'Landing',
  props: {
    vuecoonState: String
  },
});
</script>

<style scoped>
  .vuecoon {
    width: min(29vw, 12rem);
  }
  .vue_logo {
    width: 3rem;
    height: 3rem;
  }
  .title {
    margin-left: -3px;
    font-size: 1.9rem;
    margin-top: 0;
    margin-bottom: .5rem;
    font-weight: 500;
    line-height: 1.2;
  }
</style>