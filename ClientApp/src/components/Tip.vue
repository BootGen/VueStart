<template>
  <div class="alert m-2 alert-info d-flex align-items-center tip-msg" :class="{ 'show': !hideTips, 'hide' : hideTips }">
    {{ tipMessage }}
    <button type="button" class="btn p-2" aria-label="Hide" @click="$emit('hide')"><span class="bi bi-bell-slash px-2" aria-hidden="true"></span></button>
  </div>
</template>

<script>
import { defineComponent, ref, computed, watchEffect } from 'vue';
export default defineComponent({
  name: 'Tip',
  props: {
    modified: Boolean,
    generated: Boolean,
    typeChanged: Boolean
  },
  emits: ['hide'],
  setup(props) {
    const hideTips = computed(() => {
      if (localStorage.getItem("hideTips") === "true")
        return true;
      if (tipIdx.value >= tips.length)
        return true;
      return !showTip.value;
    });
    const showTip = ref(true);
    let tipIdx = ref(parseInt(localStorage.getItem('tipIdx')) || 0);
    const tips = [
      'Try to edit the JSON data on the left side, and see the changes in the application on the right side',
      'If you make structural changes to the JSON data, the application is automatically regenerated.',
      'Try out multiple application types and layouts with the buttons in the bottom right corner'
    ];
    const tipMessage = ref('');
    if (tipIdx.value < tips.length)
      tipMessage.value = tips[tipIdx.value];

    function hide() {
      localStorage.setItem("hideTips", "true");
    }

    watchEffect(() => {
      switch (tipIdx.value) {
        case 0:
          if (props.modified) {
            tipIdx.value = 1;
          }
          break;
        case 1:
          if (props.generated) {
            tipIdx.value = 2;
          }
          break;
        case 2:
          if (props.typeChanged) {
            tipIdx.value = 3;
          }
          break;
      }
      if (tipIdx.value < tips.length)
        tipMessage.value = tips[tipIdx.value];
      localStorage.setItem('tipIdx', tipIdx.value.toString());
    });

    return { tipMessage, hide, hideTips }
  }
});
</script>

<style scoped>
  .tip-msg{
    bottom: 0;
    justify-content: center!important;
    position: fixed;
    z-index: 999;
    transition: all 1s ease-in-out;
  }
  .tip-msg.show{
    opacity: 1;
    visibility: visible;
  }
  .tip-msg.hide{
    opacity: 0;
    visibility: hidden;
  }
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