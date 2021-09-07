<template>
  <div class="row">
    <div class="col-6 d-flex p-0 error-alert-container align-items-center justify-content-end">
      <div class="alert m-2 alert-danger d-flex align-items-center">
        <button type="button" class="btn-close p-2" aria-label="Close" @click="$emit('close')"></button>
        {{ errorMsg }}
      </div>
      <vueuen class="vueuen-error" type="error"></vueuen>
    </div>
  </div>
</template>

<script>
import { defineComponent } from 'vue';
import Vueuen from '../components/Vueuen.vue'

export default defineComponent({
  name: 'CodeMirror',
  components: { Vueuen },
  props: {
    errorMsg: String
  },
});
</script>

<style>
  .error-alert-container{
    position: absolute;
    bottom: 0;
    right: 50%;
    z-index: 99;
  }
  .vueuen-error{
    display: contents;
  }
</style>