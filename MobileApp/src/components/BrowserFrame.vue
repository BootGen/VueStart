<template>
  <iframe id="frameA" class="w-100" :class="{hidden: !frameA || !modelValue.page_url}" :src="urlA" title="CodeSharp"></iframe>
  <iframe id="frameB" class="w-100" :class="{hidden: frameA || !modelValue.page_url}" :src="urlB" title="CodeSharp"></iframe>
  <div class="col-12 h-100" id="js_source" :class="{hidden: !js || !modelValue.source_url}"></div>
  <div class="col-12 h-100" id="html_source" :class="{hidden: js || !modelValue.source_url}"></div>
</template>

<script>
import { defineComponent, watchEffect, ref, onMounted } from 'vue';
import axios from "axios";
import {EditorView} from "@codemirror/view";
import {basicSetup, EditorState} from "@codemirror/basic-setup";
import {javascript} from "@codemirror/lang-javascript";
import {html} from "@codemirror/lang-html";

export default defineComponent({
  name: 'BrowserFrame',
  props: {
    modelValue: Object,
  },
  setup(props) {
    const js = ref(true);
    const frameA = ref(true);
    const urlA = ref(props.modelValue.page_url);
    const urlB = ref('');
    const displayUrl = ref('');
    onMounted(function() {
      let iframeA = document.querySelector('#frameA');
      let iframeB = document.querySelector('#frameB');

      watchEffect(function () {
        if (props.modelValue.page_url) {
          displayUrl.value = `https://vuestart.com/${props.modelValue.page_url}`
          if (frameA.value) {
            if (urlA.value === props.modelValue.page_url)
              return;
            urlB.value = props.modelValue.page_url
            const onLoad = function () {
              document.getElementById('frameB').onload = undefined;
              frameA.value = false;
              setTimeout(()=> {
              iframeB.style.height = iframeB.contentDocument.body.scrollHeight + 40 + 'px';
              }, 1000);
            }
            document.getElementById('frameB').onload = onLoad;
          } else {
            if (urlB.value === props.modelValue.page_url)
              return;
            urlA.value = props.modelValue.page_url
            const onLoad = function () {
              document.getElementById('frameA').onload = undefined;
              frameA.value = true;
              setTimeout(()=> {
              iframeA.style.height = iframeA.contentDocument.body.scrollHeight + 40 + 'px';
              }, 1000);
            }
            document.getElementById('frameA').onload = onLoad;
          }
        }
        if (props.modelValue.source_url) {
          displayUrl.value = `view-source:https://vuestart.com/${props.modelValue.source_url.split('?')[0]}`
          axios.get(props.modelValue.source_url, {responseType: 'text', ...props.config}).then(resp => {
            if (displayUrl.value.endsWith('js')) {
              js.value = true;
              jsEditor.dispatch({
                changes: {from: 0, to: jsEditor.state.doc.length, insert: resp.data}
              })
            } else {
              js.value = false;
              htmlEditor.dispatch({
                changes: {from: 0, to: htmlEditor.state.doc.length, insert: resp.data}
              })
            }
          });
        }
      })
      const htmlEditor = new EditorView({
        state: EditorState.create({
          extensions: [
            basicSetup,
            html()
          ]
        }),
        parent: document.getElementById('html_source')
      });
      const jsEditor = new EditorView({
        state: EditorState.create({
          extensions: [
            basicSetup,
            javascript()
          ]
        }),
        parent: document.getElementById('js_source')
      });
    });
    return {frameA, urlA, urlB, displayUrl, js}
  }
});
</script>

<style scoped>
.hidden {
  display: none;
}
</style>
