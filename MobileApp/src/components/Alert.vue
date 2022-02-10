<template>
  <div class="d-flex alert alert-warning error-alert-container align-items-center m-2">
    <div class="text-center">
      {{ lastErrorMessage }} <br>
      <button type="button" class="btn btn-warning" aria-label="Fix" @click="$emit('fixData')" v-if="isFixable">Fix it!</button>
    </div>
    <button type="button" class="btn p-2" aria-label="Close" @click="$emit('close')"><span class="bi bi-x-lg px-2" aria-hidden="true"></span></button>
  </div>
</template>

<script>
import { defineComponent, ref, watchEffect } from 'vue';
export default defineComponent({
  name: 'Alert',
  props: {
    errorMessage: String,
    isFixable: Boolean
  },
  emits: ['close', 'fixData'],
  setup(props) {
    let lastErrorMessage = ref(props.errorMessage)
    watchEffect(() => {
      if(props.errorMessage) {
        lastErrorMessage.value = props.errorMessage;
      }
    });
    return { lastErrorMessage }
  }
});
</script>

<style>
  .error-alert-container {
    position: fixed;
    bottom: 0;
    justify-content: center!important;
    z-index: 999;
  }
</style>