<template>
  <div class="alert alert-warning d-flex fixed-bottom error-alert-container justify-content-center align-items-center mx-2">
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
    z-index: 999;
  }
</style>