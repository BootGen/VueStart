<template>
  <div class="col-12 h-100 p-0">
    <button @click="underline(4, 9)">underline</button>
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
    const editor = ref('');
    const showErrorMsg = ref(false);
    const store = useStore();

    const underlineMark = Decoration.mark({class: "cm-underline"})
    const addUnderline = StateEffect.define();

    const underlineField = StateField.define({
      create() {
        return Decoration.none
      },
      update(underlines, tr) {
        underlines = underlines.map(tr.changes)
        for (let e of tr.effects) if (e.is(addUnderline)) {
          underlines = underlines.update({
            add: [underlineMark.range(e.value.from, e.value.to)]
          })
        }
        return underlines
      },
      provide: f => EditorView.decorations.from(f)
    })

    const underlineTheme = EditorView.baseTheme({
      ".cm-underline": { textDecoration: "underline 3px red" }
    })

    function underline(from, to) {
      if (!editor.value.state.field(underlineField, false))
        editor.value.dispatch({effects: [StateEffect.appendConfig.of([underlineField, underlineTheme])]})
      editor.value.dispatch({effects: [addUnderline.of({from: from, to: to})]})
      return true
    }
    onMounted(async () => {
      let debouncedCheckJson = debounce(checkJson, 1000);

      editor.value = new EditorView({
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
                const value = editor.value.state.doc.toString();
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
        if (props.modelValue != editor.value.state.doc.toString()) {
          editor.value.dispatch({
            changes: {from: 0, to: editor.value.state.doc.length, insert: prettyPrint(props.modelValue)}
          })
        }
      }
      watchEffect(setEditorValue);
      setEditorValue();
    })
    function checkJson() {
      const json = editor.value.state.doc.toString();
      const cursorPosition = editor.value.state.selection.main.head;
      const newValue = prettyPrint(json);
      store.commit('setType', 'default')
      showErrorMsg.value = false;
      const result = validateJson(json);
      if(result.error){
        showErrorMsg.value = true;
        errorMsg.value = result.message;
        underline(result.from, result.to);
        store.commit('setType', 'error')
      }else if (json != newValue) {
        editor.value.dispatch({ changes: {from: 0, to: editor.value.state.doc.length, insert: newValue} })
        editor.value.dispatch({ selection: {anchor: cursorPosition} })
      }
    }
    return { errorMsg, showErrorMsg, underline, editor }
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