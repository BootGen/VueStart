<template>
  <div class="col-12 h-100 container p-0">
    <vueuen-alert v-if="showErrorMsg" :errorMsg="errorMsg" @close="showErrorMsg = false"></vueuen-alert>
    <textarea class="col-12 h-100" id="editor"></textarea>
  </div>
</template>

<script>
import axios from 'axios';
import CodeMirror from 'codemirror/lib/codemirror';
import 'codemirror/lib/codemirror.css';
import 'codemirror/mode/javascript/javascript.js';
import { defineComponent, onMounted, ref } from 'vue';
import VueuenAlert from '../components/VueuenAlert.vue';
import { getJsonLineNumber } from '../utils/Helper';
import { prettyPrint, validateJson } from '../utils/PrettyPrint';

export default defineComponent({
  name: 'CodeMirror',
  components: { VueuenAlert },
  setup() {
    const errorMsg = ref('');
    const showErrorMsg = ref(false);
    const cmEditor = ref(null);

    onMounted(async () => {
      const json = await getProjectContentFromServer('example_input');
      saveToLocalStorage(json);
      let debouncedCheckJson = debounce(checkJson, 1000);
      const editor = CodeMirror.fromTextArea(document.getElementById('editor'), {
        mode: "text/javascript",
        lineNumbers: true,
        tabSize: 2,
        autoCloseTags: true,
        value: document.getElementById('editor').innerHtml
      });
      editor.on('change', cm => {
        saveToLocalStorage(cm.getValue());
        debouncedCheckJson(cm);
      });
      window.addEventListener('storage', () => {
        editor.setValue(prettyPrint(localStorage.getItem('json').toString()));
      });
      editor.setValue(json);
      checkJson(editor);
      cmEditor.value = editor;
    })

    const getProjectContentFromServer = async function(name) {
      const data = (await axios.get(`/${name}.json`, {responseType: 'text'})).data;
      if (typeof data === 'string')
        return data;
      return JSON.stringify(data);
    }
    function debounce (func, wait) {
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
    const lineToColor = function(line, color) {
      cmEditor.value.markText({line: line, ch: 0}, {line: line+1, ch: 0}, {css: `background-color:${color};`});
    }
    const unsetHighlight = function() {
      if(cmEditor.value){
        cmEditor.value.markText({line: 0, ch: 0}, {line: getJsonLineNumber(cmEditor.value.getValue()), ch: 0}, {css: `background-color:unset;`});
      }
    }
    const checkJson = function(cm) {
      const json = cm.getValue();
      const cursorPosition = cm.getCursor();
      const newValue = prettyPrint(json);
      unsetHighlight();
      errorMsg.value = '';
      showErrorMsg.value = false;
      const result = validateJson(json);
      if(result.error){
        showErrorMsg.value = true;
        errorMsg.value = result.message;
        lineToColor(result.line, 'red');
      }else if (json != newValue) {
        cm.setValue(newValue);
        cm.setCursor(cursorPosition);
      }
    }
    const saveToLocalStorage = function(newValue) {
      try {
        let obj = JSON.parse(newValue);
        let minimized = JSON.stringify(obj);
        let oldValue = localStorage.getItem('json');
        if (minimized != oldValue) {
          localStorage.setItem('json', minimized);
        }
      } catch (e) {
        console.log(e)
      }
    }
    return { errorMsg, showErrorMsg }
  }
});
</script>

<style>
  .CodeMirror {
    height: 100%;
    background-color: #f1f1f1;
    padding: 10px;
  }

  .alert-bottom{
      position: fixed;
      bottom: 5px;
      left:2%;
      width: 96%;
      z-index: 9;
  }
</style>