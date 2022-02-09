<template>
    <div class="codemirror custom-card shadow">
      <code-mirror v-model="json" :error="inputError" :isFixable="isFixable" @fixData="fixData" @hasSyntaxError="$emit('hasError', $event)"></code-mirror>
    </div>
    <div class="browser-container mt-4">
      <div class="browser custom-card shadow">
        <browser-frame v-model="appUrl"></browser-frame>
      </div>
      <div class="d-flex browser-buttons">
        <div class="fab-container mx-2">
          <div class="fab fab-icon-holder">
            <span class="bi bi-pencil" aria-hidden="true" v-if="generateType === generateTypes.Editor"></span>
            <span class="bi bi-eye" aria-hidden="true" v-if="generateType === generateTypes.View"></span>
            <span class="bi bi-file-earmark-code" aria-hidden="true" v-if="generateType === generateTypes.Form"></span>
            <span class="ps-2">Mode</span>
          </div>
          <ul class="fab-options">
            <li>
              <div class="fab-icon-holder" @click="changeGeneratedMode(generateTypes.Editor)">
                <span class="bi bi-pencil" aria-hidden="true"></span>
                <span class="ps-2">Editor</span>
              </div>
            </li>
            <li>
              <div class="fab-icon-holder" @click="changeGeneratedMode(generateTypes.View)">
                <span class="bi bi-eye" aria-hidden="true"></span>
                <span class="ps-2">View</span>
              </div>
            </li>
            <li>
              <div class="fab-icon-holder" @click="changeGeneratedMode(generateTypes.Form)">
                <span class="bi bi-file-earmark-code" aria-hidden="true"></span>
                <span class="ps-2">Form</span>
              </div>
            </li>
          </ul>
        </div>
        <div class="fab-container">
          <div class="fab fab-icon-holder">
            <span class="bi bi-view-stacked" aria-hidden="true" v-if="layoutMode === layoutModes.Card"></span>
            <span class="bi bi-text-indent-left" aria-hidden="true" v-if="layoutMode === layoutModes.Accordion"></span>
            <span class="bi bi-table" aria-hidden="true" v-if="layoutMode === layoutModes.Table"></span>
            <span class="ps-2">Layout</span>
          </div>
          <ul class="fab-options">
            <li>
              <div class="fab-icon-holder" @click="changeLayoutMode(layoutModes.Card)">
                <span class="bi bi-view-stacked" aria-hidden="true"></span>
                <span class="ps-2">Card</span>
              </div>
            </li>
            <li>
              <div class="fab-icon-holder" @click="changeLayoutMode(layoutModes.Accordion)">
                <span class="bi bi-text-indent-left" aria-hidden="true"></span>
                <span class="ps-2">Accordion</span>
              </div>
            </li>
            <li>
              <div class="fab-icon-holder" @click="changeLayoutMode(layoutModes.Table)">
                <span class="bi bi-table" aria-hidden="true"></span>
                <span class="ps-2">Table</span>
              </div>
            </li>
          </ul>
        </div>
        <div id="color-picker-btn" class="fab fab-icon-holder mx-2" @click="triggerColorPicker">
          <input type="color" class="form-control form-control-color position-absolute" id="colorInput" v-model="selectedColor" title="Choose your color">
          <span class="bi bi-palette" aria-hidden="true"></span>
        </div>
        <div id="download-btn" class="fab fab-icon-holder pulse-download-btn" @click="onDownloadClicked">
          <span class="bi bi-download" aria-hidden="true"></span>
        </div>
      </div>
    </div>
</template>
<script>
import { defineComponent, ref, watchEffect } from 'vue';
import CodeMirror from './CodeMirror.vue';
import BrowserFrame from './BrowserFrame.vue'
import Tab from "@/components/Tab";
import axios from "axios";
import { getSchema } from "@/utils/Schema";
import { debounce } from "@/utils/Helper";
import { validateJson } from '@/utils/Validate';

export default defineComponent({
  components: { CodeMirror, BrowserFrame, Tab },
  props: {
    config: Object
  },
  emits: ['download', 'modified', 'generated', 'typeChanged', 'hasError', 'setVuecoon'],
  setup(props, context) {
    const inputError = ref(null);
    const isFixable = ref(false);
    const json = ref('');
    const jsonSchema = ref(getSchema({}));
    const appUrl = ref("");
    const generateTypes = {
      View: 'view',
      Form: 'form',
      Editor: 'editor',
    }
    const generateType = ref(generateTypes.Editor);
    const layoutModes = {
      Card: 'card',
      Accordion: 'accordion',
      Table: 'table'
    }
    const layoutMode = ref(layoutModes.Card);
    const tempColor = ref('42b983');
    const selectedColor = ref('#42b983');

    window.addEventListener('storage', () => {
      json.value = localStorage.getItem('json');
    });
    async function fixData() {
      const fixedJson = await axios.post('api/generate/fix', JSON.parse(json.value));
      json.value = JSON.stringify(fixedJson.data);
      await generate(json.value);
    }
    function changeGeneratedMode(type) {
      generateType.value = type;
      context.emit('typeChanged');
      generate(json.value)
    }
    function changeLayoutMode(type) {
      layoutMode.value = type;
      context.emit('typeChanged');
      generate(json.value)
    }
    async function generate(data) {
      try {
        const resp = await axios.post(`api/generate/${generateType.value}/${layoutMode.value}/${tempColor.value}`, JSON.parse(data), props.config);
        saveToLocalStorage(data);
        appUrl.value = `api/files/${resp.data.id}/index.html`;
        inputError.value = null;
        isFixable.value = false;
        context.emit('hasError', false);
      } catch (e) {
        const response = e.response;
        if (response) {
          isFixable.value = !!response.data.fixable;
          inputError.value = response.data.error;
        }
        context.emit('hasError', true);
      }
    }

    function saveToLocalStorage(newValue) {
      let obj = JSON.parse(newValue);
      let minimized = JSON.stringify(obj);
      let oldValue = localStorage.getItem('json');
      if (minimized !== oldValue) {
        localStorage.setItem('json', minimized);
        if (oldValue)
          context.emit('modified');
      }
      const downloadButton = document.getElementById('download-btn');
      downloadButton.classList.add('pulse-download-btn');
      setTimeout(function(){
        downloadButton.classList.remove('pulse-download-btn');
      }, 2000);
    }
    async function getProjectContentFromServer(name) {
      const data = (await axios.get(`/${name}.json`, {responseType: 'text', ...props.config})).data;
      if (typeof data === 'string')
        return data;
      return JSON.stringify(data);
    }

    localStorage.removeItem('json');
    getProjectContentFromServer('example_input').then( (content) => {
      json.value = content;
      jsonSchema.value = getSchema(JSON.parse(json.value));
      generate(json.value);
      function generateAndEmit(data) {
        generate(data);
        context.emit('generated');
      }
      let debouncedGenerate = debounce(generateAndEmit, 1000);
      watchEffect(() => {
        if('#' + tempColor.value !== selectedColor.value){
          tempColor.value = selectedColor.value.slice(1, 7);
          document.getElementById('color-picker-btn').style.backgroundColor = selectedColor.value;
          setTextColor();
          debouncedGenerate(json.value);
        }
        try {
          if (validateJson(json.value).error)
            return;
          const newSchema = getSchema(JSON.parse(json.value));
          if(JSON.stringify(newSchema) !== JSON.stringify(jsonSchema.value)) {
            jsonSchema.value = newSchema;
            debouncedGenerate(json.value);
          } else {
            saveToLocalStorage(json.value);
            inputError.value = null;
          }
        } catch (e) {
          if (e.schemaError) {
            inputError.value = e.schemaError;
          } else {
            console.log(e)
          }
        }
      })
    });
    function onDownloadClicked() {
      context.emit('download', `api/download/${generateType.value}/${layoutMode.value}/${tempColor.value}`, `${generateType.value}.zip`)
    }
    function triggerColorPicker() {
      document.getElementById("colorInput").click();
    }

    function setTextColor() {
      let r = parseInt(tempColor.value.substr(0, 2), 16),
          g = parseInt(tempColor.value.substr(2, 2), 16),
          b = parseInt(tempColor.value.substr(4, 2), 16);
      let brightness = Math.sqrt(
        r * r * .241 + 
        g * g * .691 + 
        b * b * .068
      );
      if (brightness > 170) {
        document.getElementById('color-picker-btn').style.color = '#2c3e50';
      } else {
        document.getElementById('color-picker-btn').style.color = '#ffffff';
      }
    }
    function refresh() {
      generate(json.value);
      context.emit('setVuecoon', 'default');
    }
    function pageRefresh() {
      let debouncedRefresh = debounce(refresh, 1000);
      context.emit('setVuecoon', 'loading');
      debouncedRefresh();
    }

    return { json, inputError, appUrl, generateType, generateTypes, layoutMode, layoutModes, selectedColor,
      changeGeneratedMode, changeLayoutMode, fixData, isFixable, onDownloadClicked, triggerColorPicker, pageRefresh }
  },
})
</script>

<style>
.codemirror{
  width: 100%;
}
.browser-container{
  width: 100%;
}
.fab-container {
  z-index: 999;
  cursor: pointer;
}
.fab-icon-holder {
  height: 50px;
  border-radius: 25px;
  background-color: #42b983;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #ffffff;
  padding: 1rem;
  box-shadow: 0 .5rem 1rem rgba(0,0,0,.10)!important;
}
.fab-icon-holder .bi{
  font-size: 1.5rem;
}
.fab-icon-holder:hover {
  background: #17a062;
}
.fab {
  height: 50px;
  background: #42b983;
}
.fab-options {
  list-style-type: none;
  margin: 0;
  position: absolute;
  bottom: 55px;
  padding: 0;
  opacity: 0;
  transition: all 0.3s ease;
  transform: scale(0);
  transform-origin: 85% bottom;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}
.fab:hover+.fab-options,
.fab-options:hover {
  opacity: 1;
  transform: scale(1);
}
.fab-options li {
  display: flex;
  justify-content: flex-end;
  padding: 5px;
}

.pulse-download-btn {
  z-index: 9;
  box-shadow: 0 0 0 0 rgba(66, 185, 131, 0.7);
  -webkit-animation: pulse 1s infinite cubic-bezier(0.66, 0, 0, 1);
  -moz-animation: pulse 1s infinite cubic-bezier(0.66, 0, 0, 1);
  -ms-animation: pulse 1s infinite cubic-bezier(0.66, 0, 0, 1);
  animation: pulse 1s infinite cubic-bezier(0.66, 0, 0, 1);
}
input#colorInput {
  width: 100px;
  opacity: 0;
  padding-top: 25px;
}
.shadow {
  box-shadow: 0 .5rem 1rem rgba(0,0,0,.10)!important;
}
.footer p {
  margin: auto;
}
a {
  color: #42b983;
}
a:hover {
  color: #17a062;
}
.pulse-download-btn:hover {
  -webkit-animation: none;
  -moz-animation: none;
  -ms-animation: none;
  animation: none;
}

@-webkit-keyframes pulse {
  to {
    box-shadow: 0 0 0 15px rgba(66, 185, 131, 0);
  }
}
@-moz-keyframes pulse {
  to {
    box-shadow: 0 0 0 15px rgba(66, 185, 131, 0);
  }
}
@-ms-keyframes pulse {
  to {
    box-shadow: 0 0 0 15px rgba(66, 185, 131, 0);
  }
}
@keyframes pulse {
  to {
    box-shadow: 0 0 0 15px rgba(66, 185, 131, 0);
  }
}

.custom-card {
  border-radius: 5px;
}
</style>
