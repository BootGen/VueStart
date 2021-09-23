<template>
  <div class="col-12 h-100 container p-0">
    <textarea class="col-12 h-100" id="editor"></textarea>
    <vueuen-alert class="aler-msg" :class="{ 'show': showErrorMsg, 'hide': !showErrorMsg }" :errorMsg="errorMsg" @close="showErrorMsg = false"></vueuen-alert>
  </div>
</template>

<script>
import CodeMirror from 'codemirror/lib/codemirror';
import 'codemirror/lib/codemirror.css';
import 'codemirror/mode/javascript/javascript.js';
import { defineComponent, onMounted, ref, watchEffect } from 'vue';
import VueuenAlert from '../components/VueuenAlert.vue';
import { getJsonLineNumber, debounce } from '../utils/Helper';
import { prettyPrint, validateJson } from '../utils/PrettyPrint';
import { useStore } from 'vuex'

export default defineComponent({
  name: 'CodeMirror',
  components: { VueuenAlert },
  props: {
    modelValue: String,
  },
  emits: ['update:modelValue'],
  setup(props, context) {
    const errorMsg = ref('');
    const showErrorMsg = ref(false);
    const cmEditor = ref(null);
    const store = useStore();

    onMounted(async () => {
      let debouncedCheckJson = debounce(checkJson, 1000);
      const editor = CodeMirror.fromTextArea(document.getElementById('editor'), {
        mode: "text/javascript",
        lineNumbers: true,
        tabSize: 2,
        autoCloseTags: true,
        value: document.getElementById('editor').innerHtml
      });
      editor.on('change', cm => {
        context.emit('update:modelValue', cm.getValue())
        debouncedCheckJson(cm);
      });
      watchEffect(() => {
        const json = editor.getValue();
        if (json != props.modelValue) {
          editor.setValue(props.modelValue);
          checkJson(editor);
        }
      })
      cmEditor.value = editor;
    })

    function lineToColor(line, color) {
      if(cmEditor.value){
        cmEditor.value.markText({line: line, ch: 0}, {line: line+1, ch: 0}, {css: `background-color:${color};`});
      }
    }
    function unsetHighlight() {
      if(cmEditor.value){
        cmEditor.value.markText({line: 0, ch: 0}, {line: getJsonLineNumber(cmEditor.value.getValue()), ch: 0}, {css: `background-color:unset;`});
      }
    }
    function checkJson(cm) {
      const json = cm.getValue();
      const cursorPosition = cm.getCursor();
      const newValue = prettyPrint(json);
      unsetHighlight();
      store.commit('setType', 'default')
      showErrorMsg.value = false;
      const result = validateJson(json);
      if(result.error){
        showErrorMsg.value = true;
        errorMsg.value = result.message;
        lineToColor(result.line, 'red');
        store.commit('setType', 'error')
      }else if (json != newValue) {
        cm.setValue(newValue);
        cm.setCursor(cursorPosition);
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
  .aler-msg{
    margin-left: 20px;
    transition: all 0.5s;
  }
  .aler-msg.show{
    opacity: 1;
    visibility: visible;
  }
  .aler-msg.hide{
    opacity: 0;
    visibility: hidden;
  }
</style>