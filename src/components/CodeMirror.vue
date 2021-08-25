<template>
  <div class="col-12 h-100 container">
    <textarea class="col-12 h-100" id="editor"></textarea>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
import { prettyPrint, validateJson } from '../utils/PrettyPrint';
import CodeMirror from 'codemirror/lib/codemirror';
import 'codemirror/lib/codemirror.css';
import 'codemirror/mode/javascript/javascript.js';

export default defineComponent({
  name: 'CodeMirror',
  setup() {
    const editor = ref(null);
    const cmError = ref({});
    return { editor, cmError }
  },
  mounted(){
    let debouncedCheckJson = this.debounce(this.checkJson, 1000);
    this.editor = CodeMirror.fromTextArea(document.getElementById('editor'), {
      mode: "text/javascript",
      lineNumbers: true,
      autoCloseTags: true,
      tabSize: 2,
    }).on('change', cm => {
      debouncedCheckJson(cm);
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
    },
    lineToColor(line, color){
      const list = document.getElementsByClassName('CodeMirror-linenumber');
        for (let i = 0; i < list.length; i++) {
          const lineNumberElement = list[i];
          const textVal = lineNumberElement.textContent;
          if (textVal) {
            const lineNum = parseInt(textVal, 10);
            if (line + 1 === lineNum) {
              const lineElement = lineNumberElement.parentElement?.parentElement?.querySelector('.CodeMirror-line');
              if (lineElement)
                lineElement.setAttribute('style', `background-color:${color};`);
            }
          }
        }
    },
    unsetHighlight: function (){
      const e  = document.getElementsByClassName('CodeMirror-line');
      for(let i = 0; i < e.length; i++){
        e[i].setAttribute('style', 'background-color: unset;');
      }
    },
    checkJson(cm) {
      const oldValue = cm.getValue();
      const newValue = prettyPrint(cm.getValue());
        this.unsetHighlight()
      if(validateJson(newValue).error){
        this.lineToColor(validateJson(newValue).line, 'red');
      }else if (oldValue != newValue) {
        cm.setValue(newValue);
      }
    }
  }
});
</script>

<style>
  .CodeMirror {
    height: 100%;
  }
</style>