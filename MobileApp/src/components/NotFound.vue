<template>
  <div class="col-12 notfound-page">
    <div class="d-flex flex-column align-items-center">
      <h1>404</h1>
      <h3>Page not found</h3>
      <p class="mb-3">The page you are looking for does not exist.</p>
      <button class="btn fill-btn rounded-pill m-1 btn-lg" @click="$emit('showEditor')">Back to editor!</button>
    </div>
  </div>
</template>

<script>
import { defineComponent } from 'vue';

export default defineComponent({
  name: "NotFound",
  emits: ['showEditor']
})
</script>

<style scoped>
h1 {
  font-size: 10rem;
  color: #42b983;
  line-height: 1;
  text-shadow: 3px 3px 0 #eee, 6px 6px 0 #17a062;
}
h3 {
  margin-top: 30px;
  margin-bottom: 30px;
  font-size: 3.75rem;
  color: #42b983;
}
.notfound-page {
  margin-top: 30px;
  margin-left: auto;
  margin-right: auto;
  padding: 20px;
}
</style>