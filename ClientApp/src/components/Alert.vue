<template>
  <div class="col-12 d-flex error-alert-container">
    <div class="alert m-2 alert-danger d-flex align-items-center">
      <div>
        {{ errorMsg }}
        <button type="button" class="btn fix-btn" aria-label="Fix" @click="$emit('fixData')" v-if="fixableData">fix</button>
      </div>
      <button type="button" class="btn p-2" aria-label="Close" @click="$emit('close')"><span class="bi bi-x-lg px-2" aria-hidden="true"></span></button>
    </div>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
export default defineComponent({
  name: 'Alert',
  props: {
    errorMsg: String,
    fixableData: Boolean
  },
  emits: ['close', 'fixData']
});
</script>

<style>
  .error-alert-container {
    position: absolute;
    bottom: 0;
    justify-content: center!important;
    z-index: 999;
  }
  .fix-btn {
    color: #842029;
    text-decoration: underline;
    padding: unset;
    padding-left: 0.5rem;
  }
  .fix-btn:hover {
    color: #b42733;
  }
  @media (max-width: 992px) {
    .error-alert-container {
      position: fixed;
      z-index: 999;
    }    
  }
</style>