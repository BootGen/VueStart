<template>
  <div class="alert m-2 alert-info d-flex align-items-center tip-msg" :class="{ 'show': !hideTips, 'hide' : hideTips }">
    {{ tipMessage }}
    <button type="button" class="btn p-2" aria-label="Hide" @click="hide"><span class="bi bi-bell-slash px-2" aria-hidden="true"></span></button>
  </div>
</template>

<script>
import { defineComponent, ref, computed, watchEffect } from 'vue';
import {debounce} from "@/utils/Helper";
export default defineComponent({
  name: 'Tip',
  props: {
    modified: Boolean,
    generated: Boolean,
    typeChanged: Boolean,
    downloaded: Boolean
  },
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
      'Try out multiple application types and layouts with the buttons in the bottom right corner.',
      'When you are done, click the download button in the bottom right corner.'
    ];
    const tipMessage = ref('');
    if (tipIdx.value < tips.length)
      tipMessage.value = tips[tipIdx.value];

    function hide() {
      localStorage.setItem("hideTips", "true");
      showTip.value = false;
    }

    const showTipMessage = debounce(function() {
      tipMessage.value = tips[tipIdx.value];
      showTip.value = true;
    }, 1000);

    watchEffect(() => {
      const modified = props.modified;
      const generated = props.generated;
      const typeChanged = props.typeChanged;
      const downloaded = props.downloaded;
      if (tipIdx.value === 0 && modified) {
        tipIdx.value = 1;
      }
      if (tipIdx.value === 1 && generated) {
        tipIdx.value = 2;
      }
      if (tipIdx.value === 2 && typeChanged) {
        tipIdx.value = 3;
      }
      if (tipIdx.value === 3 && downloaded) {
        tipIdx.value = 4;
      }
      localStorage.setItem('tipIdx', tipIdx.value.toString());
      showTip.value = false;
      if (tipIdx.value < tips.length)
        showTipMessage();
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
</style>