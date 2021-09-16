<template>
  <div class="col-12 h-100 container p-0">
    <vueuen-alert v-if="showErrorMsg" :errorMsg="errorMsg" @close="showErrorMsg = false"></vueuen-alert>
    <textarea class="col-12 h-100" id="editor"></textarea>
  </div>
</template>

<script>
import { defineComponent, ref, onMounted } from 'vue';
import { prettyPrint, validateJson } from '../utils/PrettyPrint';
import CodeMirror from 'codemirror/lib/codemirror';
import 'codemirror/lib/codemirror.css';
import 'codemirror/mode/javascript/javascript.js';
import axios from 'axios';
import VueuenAlert from '../components/VueuenAlert.vue'

export default defineComponent({
  name: 'CodeMirror',
  components: { VueuenAlert },
  setup() {
    let errorMsg = ref('');
    let showErrorMsg = ref(false);

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
    })

    async function getProjectContentFromServer (name) {
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
    function lineToColor (line, color){
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
    }
    function unsetHighlight (){
      const e  = document.getElementsByClassName('CodeMirror-line');
      for(let i = 0; i < e.length; i++){
        e[i].setAttribute('style', 'background-color: unset;');
      }
    }
    function checkJson (cm) {
      const json = cm.getValue();
      const cursorPosition = cm.getCursor();
      const newValue = prettyPrint(json);
      unsetHighlight();
      errorMsg = '';
      showErrorMsg = false;
      const result = validateJson(json);
      if(result.error){
        showErrorMsg = true;
        errorMsg = result.message;
        lineToColor(result.line, 'red');
      }else if (json != newValue) {
        cm.setValue(newValue);
        cm.setCursor(cursorPosition);
      }
    }
    function saveToLocalStorage (newValue) {
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