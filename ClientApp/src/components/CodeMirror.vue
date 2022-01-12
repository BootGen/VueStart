<template>
  <div class="col-12 h-100 p-0">
    <div class="col-12 h-100" id="editor"></div>
    <alert class="alert-msg" :class="{ 'show': syntaxError, 'hide': !syntaxError }" :errorMessage="syntaxError" :isFixable="false" @close="clearError"></alert>
    <alert class="alert-msg" :class="{ 'show': error && showError, 'hide': !error || !showError }" :errorMessage="error" :isFixable="isFixable" @close="clearError" @fixData="$emit('fixData')" v-if="!syntaxError"></alert>
  </div>
</template>

<script>
import { EditorState, basicSetup } from "@codemirror/basic-setup"
import { EditorView, keymap, Decoration } from "@codemirror/view"
import { indentWithTab } from "@codemirror/commands"
import { json } from "@codemirror/lang-json"
import {StateField, StateEffect} from "@codemirror/state"


import {defineComponent, onMounted, watchEffect, ref, watch} from 'vue';
import Alert from './Alert.vue';
import { debounce } from '../utils/Helper';
import { prettyPrint, validateJson } from '../utils/PrettyPrint';

export default defineComponent({
  name: 'CodeMirror',
  components: { Alert },
  props: {
    modelValue: String,
    error: String,
    isFixable: Boolean
  },
  emits: ['update:modelValue',  'fixData'],
  setup(props, context) {
    let editor = null;
    let syntaxError = ref(null);
    let showError = ref(true);

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
      let debouncedSetErrorState = debounce(setErrorState, 1000);

      editor = new EditorView({
        state: EditorState.create({
          extensions: [
            basicSetup,
            EditorView.theme({
              "&": {
                color: "#42b983",
                backgroundColor: "#34495E"
              },
              ".ͼd": {
                color: "rgba(255, 255, 255, 0.9)"
              },
              ".ͼc": {
                  color: "#8cd6b5"
              },
              ".cm-content": {
                caretColor: "#42b983"
              },
              "&.cm-focused .cm-cursor": {
                borderLeftColor: "#42b983"
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
                backgroundColor: "#34495E",
                color: "#ddd",
                border: "none"
              }
            }, {dark: true}),
            EditorView.updateListener.of((cm) => {
              if (cm.docChanged) {
                  let validationResult = validateJson(editor.state.doc.toString());
                  if (!validationResult.error) {
                    indent();
                  }
                  debouncedSetErrorState(validationResult);
                  const value = editor.state.doc.toString();
                  if (props.modelValue != value) {
                    context.emit('update:modelValue', value);
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
        if (props.modelValue !== editor.state.doc.toString()) {
          editor.dispatch({
            changes: {from: 0, to: editor.state.doc.length, insert: prettyPrint(props.modelValue)}
          })
        }
      }
      watchEffect(setEditorValue);
      editor.dispatch({effects: [StateEffect.appendConfig.of([underlineField, underlineTheme])]});
      setEditorValue();
    })

    function setErrorState(validationResult) {
      underlineChanged = true;
      if(validationResult.error){
        if (validationResult.from > 0 && validationResult.to > 0 && validationResult.to > validationResult.from)
          editor.dispatch({effects: [addUnderline.of({ from: validationResult.from, to: validationResult.to })]});
        syntaxError.value = validationResult.message;
      } else {
        syntaxError.value = null;
        const cursorPosition = editor.state.selection.main.head;
        editor.dispatch({ selection: {anchor: cursorPosition} })
      }
    }
    function indent() {
        const cursorPosition = editor.state.selection.main.head;
        if (cursorPosition === 0)
          return;
        const json = editor.state.doc.toString();
        if (validateJson(json).error) {
          return
        }
        const lastChar=json[cursorPosition-1]
        let cursorCodePosition = 0;
        for (let i = 0; i < cursorPosition; ++i) {
          if (' \t\n\r\v'.indexOf(json[i]) === -1) {
              cursorCodePosition += 1;
          }
        }
        if (' \t\n\r\v'.indexOf(lastChar) === -1) {
          const newValue = prettyPrint(json);
          if (json != newValue) {
            editor.dispatch({ changes: {from: 0, to: editor.state.doc.length, insert: newValue} })
            let newCursorPosition = 0;
            for (let i = 0; i < newValue.length; ++i) {
              newCursorPosition += 1;
              if (' \t\n\r\v'.indexOf(newValue[i]) === -1) {
                  cursorCodePosition -= 1;
                  if (cursorCodePosition === 0)
                    break
              }
            }
            underlineChanged = true;
            editor.dispatch({ selection: {anchor: newCursorPosition} })
          }
        }
    }
    watch(() => props.error, (newError) => {
      if (newError)
        showError.value = true;
    });
    function clearError() {
      syntaxError.value = null;
      showError.value = false;
    }
    return { syntaxError, clearError, showError }
  }
});
</script>

<style>
  .cm-editor {
    height: 100%;
  }
  .CodeMirror {
    height: 100%;
    background-color: #34495E;
    color: white;
    padding: 10px;
  }
  .CodeMirror-gutters {
    background-color: unset;
  }
  .cm-string {
    color: #42b983!important;
  }

  .alert-msg{
    margin-left: 20px;
    transition: all 0.5s;
    margin-right: unset;
  }
  .alert-msg.show{
    opacity: 1;
    visibility: visible;
  }
  .alert-msg.hide{
    opacity: 0;
    visibility: hidden;
  }
  @media (max-width: 992px) {
    .alert-msg {
      margin-left: unset;
      justify-content: center;
    }
  }
  @media (max-width: 576px) {
    .cm-line {
      tab-size: 2;
    }
  }
</style>