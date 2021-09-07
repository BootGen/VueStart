<template>
  <transition name="fade">
    <div class="row" v-if="show">
      <div class="col-6 d-flex p-0 error-alert-container align-items-center justify-content-end">
        <div class="alert m-2 alert-danger d-flex align-items-center">
          <button type="button" class="btn-close p-2" aria-label="Close" @click="show=false"></button>
          {{ errorMsg }}
        </div>
        <vueuen class="vueuen-error" type="error"></vueuen>
      </div>
    </div>
  </transition>
</template>

<script>
import { defineComponent, ref } from 'vue';
import Vueuen from '../components/Vueuen.vue'

export default defineComponent({
  name: 'CodeMirror',
  components: { Vueuen },
  setup() {
    const show = ref(true);
    return { show };
  },
  props: {
    errorMsg: String
  },
});
</script>

<style>
  .error-alert-container {
    position: absolute;
    bottom: 0;
    right: 50%;
    z-index: 99;
  }

.fade-enter-active, .fade-leave-active {
  transition: opacity .5s ease;
}

.fade-enter-from, .fade-leave-to {
  opacity: 0;
}
/*
  .fade-enter {
    opacity: 0;
  }
  .fade-enter-active, .fade-leave-active {
    transition: opacity 0.5s ease-out;
  }
  .fade-leave-to {
    opacity: 0;
  }*/
</style>