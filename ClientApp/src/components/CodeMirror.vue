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
import {StateField, StateEffect} from "@codemirror/state"


import { defineComponent, onMounted, ref, watchEffect } from 'vue';
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
    let editor = null;
    const showErrorMsg = ref(false);
    const store = useStore();

    const underlineMark = Decoration.mark({class: "cm-underline"})
    const addUnderline = StateEffect.define();

    let emptyField = null;
    let underlineChanged = false;
    const underlineField = StateField.define({
      create() {
        return Decoration.none
      },
      update(underlines, tr) {
        if (!emptyField)
          emptyField = underlines;
        if (underlineChanged) {
          underlineChanged = false;
          underlines = emptyField.map(tr.changes);
          for (let e of tr.effects)
            if (e.is(addUnderline)) {
              underlines = underlines.update({
                add: [underlineMark.range(e.value.from, e.value.to)]
              })
            }
        }
        return underlines
      },
      provide: f => EditorView.decorations.from(f)
    })

    const underlineTheme = EditorView.baseTheme({
      ".cm-underline": { textDecoration: "underline 3px red" }
    })
    
    onMounted(async () => {
      let debouncedCheckJson = debounce(checkJson, 1000);

      editor = new EditorView({
        state: EditorState.create({
          extensions: [
            basicSetup,
            EditorView.theme({
              "&": {
                color: "#61c7fd",
                backgroundColor: "#27203c"
              },
              ".ͼd": {
                color: "rgba(255, 255, 255, 0.9)"
              },
              ".cm-content": {
                caretColor: "#61c7fd"
              },
              "&.cm-focused .cm-cursor": {
                borderLeftColor: "#61c7fd"
              },
              "&.cm-focused .cm-selectionBackground, ::selection": {
                backgroundColor: "rgba(97, 199, 253, 0.2)"
              },
              ".cm-selectionMatch": {
                backgroundColor: "rgba(97, 199, 253, 0.1)"
              },
              ".cm-activeLine, .cm-activeLineGutter": {
                backgroundColor: "rgba(97, 199, 253, 0.1)"
              },
              ".cm-gutters": {
                backgroundColor: "#27203c",
                color: "#ddd",
                border: "none"
              }
            }, {dark: true}),
            EditorView.updateListener.of((cm) => {
              if (cm.docChanged) {
                const value = editor.state.doc.toString();
                if (props.modelValue != value) {
                  context.emit('update:modelValue', value);
                  debouncedCheckJson();
                }
              }
            }),
            keymap.of([indentWithTab]),
            json()
          ]
        }),
        parent: document.getElementById('editor')
      })
      function setEditorValue() {
        if (props.modelValue != editor.state.doc.toString()) {
          editor.dispatch({
            changes: {from: 0, to: editor.state.doc.length, insert: prettyPrint(props.modelValue)}
          })
        }
      }
      watchEffect(setEditorValue);
      editor.dispatch({effects: [StateEffect.appendConfig.of([underlineField, underlineTheme])]});
      setEditorValue();
    })
    function checkJson() {
      const json = editor.state.doc.toString();
      const cursorPosition = editor.state.selection.main.head;
      const newValue = prettyPrint(json);
      store.commit('setType', 'default')
      showErrorMsg.value = false;
      const result = validateJson(json);
      underlineChanged = true;
      if(result.error){
        showErrorMsg.value = true;
        errorMsg.value = result.message;
        if (result.from > 0 && result.to > 0)
          editor.dispatch({effects: [addUnderline.of({ from: result.from, to: result.to })]});
      } else if (json != newValue) {
        editor.dispatch({ changes: {from: 0, to: editor.state.doc.length, insert: newValue} })
      }
      editor.dispatch({ selection: {anchor: cursorPosition} })
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
    margin-right: unset;
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