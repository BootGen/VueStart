<template>
  <div class="col-12 h-100 p-0">
    <div class="col-12 h-100" id="editor"></div>
    <alert class="aler-msg" :class="{ 'show': showErrorMsg, 'hide': !showErrorMsg }" :errorMsg="errorMsg" @close="showErrorMsg = false"></alert>
  </div>
</template>

<script>
import { EditorState, basicSetup } from "@codemirror/basic-setup"
import { EditorView, keymap, Decoration /*, Range*/ } from "@codemirror/view"
import { indentWithTab } from "@codemirror/commands"
import { json } from "@codemirror/lang-json"
import {/*StateField,*/ StateEffect} from "@codemirror/state"


import { defineComponent, onMounted, ref } from 'vue';
import Alert from './Alert.vue';
import { /*getJsonLineNumber,*/ debounce } from '../utils/Helper';
import { prettyPrint, validateJson } from '../utils/PrettyPrint';
import { useStore } from 'vuex'

export default defineComponent({
  name: 'CodeMirror',
  components: { Alert },
  props: {
    modelValue: String,
  },
  emits: ['update:modelValue'],
  setup(props, context) {
    const errorMsg = ref('');
    const showErrorMsg = ref(false);
    const store = useStore();

    onMounted(async () => {
      let debouncedCheckJson = debounce(checkJson, 1000);

      const editor = new EditorView({
        state: EditorState.create({
          extensions: [
            basicSetup,
            EditorView.updateListener.of((cm) => {
              if (cm.docChanged) {
                context.emit('update:modelValue', cm.state.doc.toString());
                debouncedCheckJson(editor);
              }
            }),
            keymap.of([indentWithTab]),
            json()
          ]
        }),
        parent: document.getElementById('editor')
      })
      editor.dispatch({
        changes: {from: 0, insert: prettyPrint(localStorage.getItem('json'))}
      })

      const addMarks = StateEffect.define()
      const strikeMark = Decoration.mark({
        attributes: {style: "color: yellow"}
      })
      editor.dispatch({
        effects: addMarks.of([strikeMark.range(1, 4)])
      })
    })

    function lineToColor(line, color) {
      console.log(line, color)
      /*if(cmEditor.value){
        cmEditor.value.markText({line: line, ch: 0}, {line: line+1, ch: 0}, {css: `background-color:${color};`});
      }*/
    }
    function unsetHighlight() {
      /*if(cmEditor.value){
        cmEditor.value.markText({line: 0, ch: 0}, {line: getJsonLineNumber(cmEditor.value.getValue()), ch: 0}, {css: `background-color:unset;`});
      }*/
    }
    function checkJson(cm) {
      const json = cm.state.doc.toString();
      const cursorPosition = cm.state.selection.main.head;
      const newValue = prettyPrint(json);
      unsetHighlight();
      store.commit('setType', 'default')
      showErrorMsg.value = false;
      const result = validateJson(json);
      console.log(result)
      if(result.error){
        showErrorMsg.value = true;
        errorMsg.value = result.message;
        lineToColor(result.line, 'rgba(255, 0, 0, 0.4)');
        store.commit('setType', 'error')
        console.log('setError', store.state.vuecoonType)
      }else if (json != newValue) {
        cm.dispatch({ changes: {from: 0, to: cm.state.doc.length, insert: newValue} })
        cm.dispatch({ selection: {anchor: cursorPosition} })
        console.log("setChange")
      }
    }
    return { errorMsg, showErrorMsg }
  }
});
</script>

<style>
  .cm-editor {
    height: 100%;
  }


  .CodeMirror {
    height: 100%;
    background-color: rgb(9, 26, 55);
    color: white;
    padding: 10px;
  }
  .CodeMirror-gutters {
    background-color: unset;
  }
  .cm-string {
    color: #61C7FD!important;
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
  @media (max-width: 992px) {
    .aler-msg {
      margin-left: unset;
      justify-content: center;
    }
    
  }
</style>