<template>
  <div class="container-fluid d-inline-flex h-100 align-items-center m-3">
    <div class="row">
      <h1>Start Vue</h1>
      <p class="lead text-justify">
        Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.
      </p>
      <router-link class="p-2 btn btn-outline-primary" to="/editor">Go to editor</router-link>
    </div>
    <vueuen type="default"></vueuen>
  </div>
</template>

<script>
import Vueuen from '../components/Vueuen.vue'

export default {
  components: { Vueuen },
  name: 'LandingPage',
}
</script>
