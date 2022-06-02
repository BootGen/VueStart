<template>
  <div class="browser custom-card shadow">
    <div class="col-12 browser-frame">
      <div class="d-flex browser-nav py-1">
        <button type="button" class="btn-site inactive w-auto" :class="{ 'border-bottom-right' : borderRadius }">
          <div class="d-flex align-items-center">
            <span class="dot" style="background:#ED594A;"></span>
            <span class="dot" style="background:#FDD800;"></span>
            <span class="dot" style="background:#5AFF5A;"></span>
          </div>
        </button>
        <slot></slot>
      </div>
      <div class="d-flex justify-content-around align-items-center menu">
        <div class="d-flex">
          <span class="bi bi-arrow-left icon" :class="{'clickable': undoable}" aria-hidden="true" @click="$emit('undo')"></span>
          <span class="bi bi-arrow-right icon" :class="{'clickable': redoable}" aria-hidden="true" @click="$emit('redo')"></span>
          <span class="bi bi-arrow-clockwise icon clickable" aria-hidden="true" @click="$emit('refresh')"></span>
        </div>
        <div class="middle mx-2">
          <input type="text" :value="displayUrl" disabled>
        </div>
        <span class="bi bi-three-dots-vertical icon" aria-hidden="true"></span>
      </div>
      <div class="col-12 content bg-white">
        <iframe id="frameA" class="h-95 w-100" :class="{hidden: !frameA || !modelValue.page_url}" :src="urlA" title="CodeSharp"></iframe>
        <iframe id="frameB" class="h-95 w-100" :class="{hidden: frameA || !modelValue.page_url}" :src="urlB" title="CodeSharp"></iframe>
        <div class="col-12 h-100" id="js_source" :class="{hidden: !js || !modelValue.source_url}"></div>
        <div class="col-12 h-100" id="html_source" :class="{hidden: js || !modelValue.source_url}"></div>
      </div>
    </div>
  </div>
</template>

<script>
import {defineComponent, ref, onMounted, watchEffect} from 'vue';
import axios from "axios";
import {EditorView} from "@codemirror/view";
import {basicSetup, EditorState} from "@codemirror/basic-setup";
import {javascript} from "@codemirror/lang-javascript";
import {html} from "@codemirror/lang-html";

export default defineComponent({
  name: 'BrowserFrame',
  props: {
    modelValue: Object,
    config: Object,
    borderRadius: Boolean,
    undoable: Boolean,
    redoable: Boolean
  },
  emits: ['refresh', 'undo', 'redo'],
  setup(props) {
    const js = ref(true);
    const frameA = ref(true);
    const urlA = ref(props.modelValue.page_url);
    const urlB = ref('');
    const displayUrl = ref('');
    onMounted(function() {
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
            }
            document.getElementById('frameB').onload = onLoad;
          } else {
            if (urlB.value === props.modelValue.page_url)
              return;
            urlA.value = props.modelValue.page_url
            const onLoad = function () {
              document.getElementById('frameA').onload = undefined;
              frameA.value = true;
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
            html(),
            EditorView.editable.of(false)
          ]
        }),
        parent: document.getElementById('html_source')
      });
      const jsEditor = new EditorView({
        state: EditorState.create({
          extensions: [
            basicSetup,
            javascript(),
            EditorView.editable.of(false)
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
.h-95 {
  height: 95%;
}
.browser-nav {
  position: relative;
  margin-bottom: 5px;
  z-index: 99;
}
.browser-frame{
  height: calc( 100% - 3rem);
}
#js_source, #html_source {
  background-color: #fff;
}
.menu {    
  background-color: #fff;
  position: absolute;
  width: 100%;
  top: 20px;
  z-index: 9;
  padding: 5px;
  padding-top: 25px;
  border-bottom: solid 1px #eee;
}
.icon {
  color: #42b983;
  width: 30px;
  height: 30px;
  align-items: center;
  justify-content: center;
  display: flex;
}
.clickable{
  cursor: pointer;
}
.clickable:hover {
  background-color: #eeeeee;
  border-radius: 100%;
}
.middle {
  width: 100%;
}

/* Three dots */
.dot {
  margin: 4px;
  height: 12px;
  width: 12px;
  background-color: #bbb;
  border-radius: 50%;
  display: inline-block;
}

/* Style the input field */
input[type=text] {
  width: 100%;
  border-radius: 15px;
  border: none;
  background-color: #eee;
  margin-top: -8px;
  height: 25px;
  color: #42b983;
  padding: 5px;
  padding-left: 15px;
}

/* Three bars (hamburger menu) */
.bar {
  width: 17px;
  height: 3px;
  background-color: #aaa;
  margin: 3px 0;
  display: block;
}

/* Page content */
.content {
  border-radius: 5px;
  height: calc( 100% - 1rem);
  margin-top: 30px;
}

.hidden {
  display: none;
}
@media (max-width: 576px) {
  .dot {
    margin: 3px;
    height: 10px;
    width: 10px;
  }
}
</style>
