<template>
  <div class="d-flex tip-alert-container col-12" v-if="show">
    <div class="alert m-2 alert-info d-flex align-items-center">
      {{ tipMsg }}
      <button type="button" class="btn p-2" aria-label="Hide" @click="hideTips()"><span class="bi bi-bell-slash px-2" aria-hidden="true"></span></button>
    </div>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
export default defineComponent({
  name: 'Tip',
  props: {
    tipMsg: String
  },
  setup() {
    const show = ref(true);

    function hideTips(){
      localStorage.setItem('showTips', false);
      show.value = false;
    }
    return { show, hideTips };
  }
});
</script>

<style>
  .tip-alert-container {
    bottom: 0;
    justify-content: center!important;
    position: fixed;
    z-index: 999;
    
  }
</style>