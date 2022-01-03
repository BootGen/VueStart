<template>
  <div class="col-12 d-flex error-alert-container"  v-if="show">
    <div class="alert m-2 alert-danger d-flex align-items-center">
      {{ errorMsg }}
      <button type="button" class="btn p-2" aria-label="Close" @click="show=false"><span class="bi bi-x-lg px-2" aria-hidden="true"></span></button>
    </div>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
export default defineComponent({
  name: 'Alert',
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
    z-index: 999;
  }
  @media (max-width: 992px) {
    .error-alert-container {
      position: fixed;
      z-index: 999;
    }    
  }
</style>