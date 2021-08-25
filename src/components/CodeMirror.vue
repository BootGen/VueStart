<template>
  <div class="col-12 h-100 container">
    <textarea class="col-12 h-100" id="editor"></textarea>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
import { prettyPrint } from '../utils/PrettyPrint';
import CodeMirror from 'codemirror/lib/codemirror';
import 'codemirror/lib/codemirror.css';
import 'codemirror/mode/javascript/javascript.js';

export default defineComponent({
  name: 'CodeMirror',
  setup() {
    const editor = ref(null)
    return { editor }
  },
  mounted(){
    let func = this.debounce(function(cm) {
      const oldValue = cm.getValue()
      const newValue = prettyPrint(cm.getValue());
      if (oldValue != newValue) {
        cm.setValue(newValue);
      }
    }, 1000);
    this.editor = CodeMirror.fromTextArea(document.getElementById('editor'), {
      mode: "text/javascript",
      lineNumbers: true,
      autoCloseTags: true,
      tabSize: 2,
    }).on('change', cm => {
      func(cm);
    });
  },
  methods: {
    debounce (func, wait) {
      let timeout;
      return function executedFunction(...args) {
        const later = () => {
          clearTimeout(timeout);
          func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
      };
    }
  }
});
</script>

<style>
  .CodeMirror {
    height: 100%;
  }
</style>