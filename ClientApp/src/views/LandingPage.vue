<template>
  <div class="container h-100">
    <div class="row h-100 justify-content-center align-items-center">
      <div class="col-12">
        <div class="row">
          <h1>Start Vue!</h1>
          <p class="lead text-justify">
            Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.
          </p>
          <router-link class="btn btn-outline-primary rounded-pill" to="/editor">Go!</router-link>
        </div>
      </div>
      <vueuen type="default"></vueuen>
    </div>
  </div>
</template>

<script>
import Vueuen from '../components/Vueuen.vue'

export default {
  components: { Vueuen },
  name: 'LandingPage',
}
</script>

<style scoped>
  .btn{
    width: max-content;
    font-size: 1.5em;
  }
</style>
