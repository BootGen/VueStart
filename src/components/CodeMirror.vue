<template>
  <div class="col-12 h-100 container">
    <textarea class="col-12 h-100" id="editor"></textarea>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
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
    let func = this.debounce(function() {
        console.log('change');
      }, 1000);
    this.editor = CodeMirror.fromTextArea(document.getElementById('editor'), {
      mode: "text/javascript",
      lineNumbers: true,
      autoCloseTags: true,
      tabSize: 2,
    }).on('change', cm => {
      console.log(cm.getValue());
      func()
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