<template>
  <div class="d-flex flex-column align-items-center mt-3">
    <div class="d-flex align-items-center justify-content-center">
      <img class="vue_logo" alt="vue" :src="require(`../assets/vue_logo.webp`)">
      <p class="title">ue Start!</p>
    </div>
    <img class="vuecoon img-fluid px-3 mt-4" alt="Vuecoon" :src="require(`../assets/vuecoon_${vuecoonState}.webp`)">
    <p class="logos mt-4">
      <img src="../assets/vue_logo.webp"> <img src="../assets/bootstrap.webp"> <img src="../assets/tailwind.webp"> <img src="../assets/css.webp">
    </p>
    <span class="lead text-center">
      An online tool that generates UI components for Vue.js developers. Input some JSON data, chose a template, download the code and use it in any project.
    </span>
    <p class="text-center small-text"><em>For the full feature set please visit this page from a desktop browser.</em></p>
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
    width: min(50vw, 15rem);
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
  .logos > img {
    height: 4rem;
  }
</style>