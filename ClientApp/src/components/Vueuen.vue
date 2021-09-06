<template>
  <div class="container">
    <img v-if="type == 'default'" alt="Vueuen default" :src="require('../assets/vueuen.webp')">
    <img v-if="type == 'loading'" alt="Vueuen loading" :src="require('../assets/vueuen_loading.webp')">
    <img v-if="type == 'error'" alt="Vueuen error" :src="require('../assets/vueuen_error.webp')">
  </div>
</template>

<script>
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'Vueuen',
  props: {
    type: String
  },
});
</script>