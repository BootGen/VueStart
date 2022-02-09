<template>
  <iframe id="frameA" class="h-100 w-100" :class="{hidden: !frameA}" :src="urlA" title="CodeSharp"></iframe>
  <iframe id="frameB" class="h-100 w-100" :class="{hidden: frameA}" :src="urlB" title="CodeSharp"></iframe>
</template>

<script>
import { defineComponent, watch, ref, onMounted } from 'vue';

export default defineComponent({
  name: 'BrowserFrame',
  props: {
    modelValue: String,
  },
  setup(props) {
    const frameA = ref(true);
    const urlA = ref(props.modelValue);
    const urlB = ref("");
    onMounted(function(){
      watch(props, function(){
        if (frameA.value) {
          if (urlA.value === props.modelValue)
            return;
          urlB.value = props.modelValue
          const onLoad = function() {
              document.getElementById('frameB').onload = undefined;
              frameA.value = false;
          }
          document.getElementById('frameB').onload = onLoad;
        } else {
          if (urlB.value === props.modelValue)
            return;
          urlA.value = props.modelValue
          const onLoad = function() {
              document.getElementById('frameA').onload = undefined;
              frameA.value = true;
          }
          document.getElementById('frameA').onload = onLoad;
        }
      })
    })

    return { frameA, urlA, urlB}
  }
});
</script>

<style scoped>
.hidden {
  display: none;
}
</style>
