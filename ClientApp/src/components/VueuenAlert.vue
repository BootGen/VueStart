<template>
  <div class="row" v-if="show">
    <div class="col-12 d-flex error-alert-container">
      <div class="alert m-2 alert-danger d-flex align-items-center">
        <button type="button" class="btn-close p-2" aria-label="Close" @click="show=false"></button>
        {{ errorMsg }}
      </div>
    </div>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
export default defineComponent({
  name: 'CodeMirror',
  props: {
    errorMsg: String
  },
  setup() {
    const show = ref(true);
    return { show };
  }
});
</script>

<style>
  .error-alert-container {
    position: absolute;
    bottom: 0;
    justify-content: center!important;
    z-index: 9;
  }
</style>