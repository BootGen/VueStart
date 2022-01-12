<template>
  <div class="alert m-2 alert-info d-flex align-items-center" v-if="!hideTips">
    {{ tempTipMsg }}
    <button type="button" class="btn p-2" aria-label="Hide" @click="$emit('hide')"><span class="bi bi-bell-slash px-2" aria-hidden="true"></span></button>
  </div>
</template>

<script>
import { defineComponent, ref, watchEffect } from 'vue';
export default defineComponent({
  name: 'Tip',
  props: {
    tipMsg: String
  },
  emits: ['hide'],
  setup(props) {
    const tempTipMsg = ref(null);
    const hideTips = ref(localStorage.getItem("hideTips") === "true");

    watchEffect(() => {
      if(props.tipMsg) {
        tempTipMsg.value = props.tipMsg;
      }
    });

    function hide() {
      localStorage.setItem("hideTips", "true");
      hideTips.value = true;
    }

    return { tempTipMsg, hide, hideTips }
  }
});
</script>

<style scoped>
  .alert {
    left: 50%!important;
    transform: translate(-50%,-40%)!important;
  }
  @media (max-width: 992px) {
    .alert {
      left: unset!important;
      transform: unset!important;
      width: -webkit-fill-available;
    }
  }
</style>>