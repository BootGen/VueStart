<template>
  <div class="col-12 h-100 container">
    codemirror
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';


export default defineComponent({
  name: 'CodeMirror',
  components: {  },
  setup() {
    const value = ref('');
    const cmOptions = {
        theme: 'material',
        tabSize: 2,
        mode: 'text/javascript',
        lineNumbers: true,
        line: true,
        readOnly: false,
      };
    return { value, cmOptions }
  },
});
</script>

<style>
  .cm{
    margin: 0!important;
    padding: 0!important;
    position: relative;
    height: 100%;
  }
  .vue-codemirror{
    width: 100%;
    height: calc(100% - 40px);
    margin-bottom: 0!important;
    padding-bottom: 0!important;
  }
  .CodeMirror-scroll{
    overflow: auto!important;
    margin: 0!important;
    padding: 0!important;
  }
  .CodeMirror{
    height: 100%;
    z-index: 0;
  }
</style>