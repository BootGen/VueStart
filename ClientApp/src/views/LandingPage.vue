<template>
  <div class="container-fluid mx-auto">
    <div class="d-inline-flex align-center justify-content-center">
        <div class="row">
          <h1>Start Vue</h1>
          <p class="lead">
            Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.
          </p>
          <router-link class="pl-2" to="/editor">Editor</router-link>
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
