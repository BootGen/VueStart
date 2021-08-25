<template>
  <div class="col-12 h-100 container">
    <textarea class="col-12 h-100" id="editor"></textarea>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
import CodeMirror from "codemirror/lib/codemirror";
import "codemirror/lib/codemirror.css";
import "codemirror/mode/javascript/javascript.js";

export default defineComponent({
  name: 'CodeMirror',
  components: {  },
  setup() {
    const value = ref('codemirror');
    const cmOptions = {
      theme: 'material',
      tabSize: 2,
      mode: 'text/javascript',
      lineNumbers: true,
      line: true,
      readOnly: false,
    };
    const editor = null
    return { value, cmOptions, editor }
  },
  mounted(){
    this.editor = CodeMirror.fromTextArea(document.getElementById('editor'), {
      lineNumbers: true
    });
  }
});
</script>

<style>
  .CodeMirror {
    height: 100%;
  }
</style>