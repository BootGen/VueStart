<template>
  <iframe id="frameA" class="w-100" :class="{hidden: !frameA}" :src="urlA" title="CodeSharp"></iframe>
  <iframe id="frameB" class="w-100" :class="{hidden: frameA}" :src="urlB" title="CodeSharp"></iframe>
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
      let iframeA = document.querySelector('#frameA');
      let iframeB = document.querySelector('#frameB');
      
      watch(props, function(){
        if (frameA.value) {
          if (urlA.value === props.modelValue)
            return;
          urlB.value = props.modelValue
          const onLoad = function() {
              document.getElementById('frameB').onload = undefined;
              frameA.value = false;
              setTimeout(()=> {
              iframeB.style.height = iframeB.contentDocument.body.scrollHeight + 40 + 'px';
              }, 1000);
          }
          document.getElementById('frameB').onload = onLoad;
        } else {
          if (urlB.value === props.modelValue)
            return;
          urlA.value = props.modelValue
          const onLoad = function() {
              document.getElementById('frameA').onload = undefined;
              frameA.value = true;
              setTimeout(()=> {
              iframeA.style.height = iframeA.contentDocument.body.scrollHeight + 40 + 'px';
              }, 1000);
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
