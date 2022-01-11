<template>
  <div class="alert m-2 alert-info d-flex align-items-center">
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

    watchEffect(() => {
      if(props.tipMsg) {
        tempTipMsg.value = props.tipMsg;
      }
    });

    return { tempTipMsg }
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