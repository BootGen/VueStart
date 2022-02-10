<template>
    <div class="codemirror custom-card" :class="page">
      <code-mirror v-model="json" :error="inputError" :isFixable="isFixable" @fixData="fixData" @hasSyntaxError="$emit('hasError', $event)"></code-mirror>
    </div>
    <div class="browser-container" :class="page">
      <div class="browser custom-card shadow">
        <browser-frame v-model="browserData" :config="config"  @refresh="pageRefresh" :borderRadius="selectedTab === 0">
          <div class="d-flex w-100 h-auto">
            <tab :title="generateType" :icon="generatedTypeIcon" :class="{'active': selectedTab === 0, 'inactive': selectedTab !== 0, 'border-bottom-right': selectedTab === 1}" @select="selectedTab = 0"></tab>
            <tab title="index.html" icon="code" :class="{'active': selectedTab === 1, 'inactive': selectedTab !== 1, 'border-bottom-left': selectedTab === 0, 'border-bottom-right': selectedTab === 2}" @select="selectedTab = 1"></tab>
            <tab title="app.js" icon="code" :class="{'active': selectedTab === 2, 'inactive': selectedTab !== 2, 'border-bottom-left': selectedTab === 1}" @select="selectedTab = 2"></tab>
            <tab class="inactive" :class="{'border-bottom-left': selectedTab === 2}"></tab>
          </div>
        </browser-frame>
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
import {computed, defineComponent, ref, watch, watchEffect} from 'vue';
import CodeMirror from './CodeMirror.vue';
import BrowserFrame from './BrowserFrame.vue'
import Tab from "@/components/Tab";
import axios from "axios";
import {getSchema} from "@/utils/Schema";
import {debounce} from "@/utils/Helper";
import { validateJson } from '@/utils/Validate';

export default defineComponent({
  components: { CodeMirror, BrowserFrame, Tab },
  props: {
    page: String,
    config: Object
  },
  emits: ['download', 'modified', 'generated', 'typeChanged', 'hasError', 'setVuecoon'],
  setup(props, context) {
    const inputError = ref(null);
    const isFixable = ref(false);
    const json = ref('');
    const jsonSchema = ref(getSchema({}));
    const selectedTab = ref(0);
    const browserData = ref({ page_url: "" });
    const generatedId = ref('');
    const generateTypes = {
      View: 'view',
      Form: 'form',
      Editor: 'editor',
    }
    watch(selectedTab,function () {
      switch (selectedTab.value) {
        case 0:
          browserData.value = {
            page_url: `api/files/${generatedId.value}/index.html`
          };
          break;
        case 1:
          browserData.value = {
            source_url: `api/files/${generatedId.value}/index.html`
          };
          break;
        case 2:
          browserData.value = {
            source_url: `api/files/${generatedId.value}/app.js`
          };
          break;
      }
    })
    const generatedTypeIcon = computed(() =>{
      switch (generateType.value) {
        case generateTypes.Editor:
          return 'pencil';
        case generateTypes.View:
          return 'eye';
        case generateTypes.Form:
          return 'file-earmark-code';
      }
      return 'pencil';
    })
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
        console.log(`api/generate/${generateType.value}/${layoutMode.value}/${tempColor.value}`);
        const resp = await axios.post(`api/generate/${generateType.value}/${layoutMode.value}/${tempColor.value}`, JSON.parse(data), props.config);
        saveToLocalStorage(data);
        generatedId.value = resp.data.id;
        if (selectedTab.value === 0) {
          browserData.value = {
            page_url: `api/files/${resp.data.id}/index.html`
          };
        } else {
          browserData.value = {
            source_url: `api/files/${resp.data.id}/index.html`
          };
        }
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
      watch([tempColor, selectedColor],() => {
        if ('#' + tempColor.value !== selectedColor.value) {
          tempColor.value = selectedColor.value.slice(1, 7);
          document.getElementById('color-picker-btn').style.backgroundColor = selectedColor.value;
          setTextColor();
          debouncedGenerate(json.value);
        }
      });
      watch(json, () => {
        console.log(json.value);
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

    return { json, inputError, generateType, generateTypes, layoutMode, layoutModes, selectedColor,
      changeGeneratedMode, changeLayoutMode, fixData, isFixable, onDownloadClicked, triggerColorPicker, pageRefresh,
      selectedTab, generatedTypeIcon, browserData}
  },
})
</script>

<style>
body {
  height: 100%;
  overflow: hidden;
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

.browser-buttons {
  position: absolute;
  bottom: -1rem;
  right: 2rem;
  font-size: 1rem!important;
  z-index: 99;
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

.codemirror{
  position: absolute;
  width: 47%;
  margin: 1%;
  transition: all 1s ease-in-out;
  transition-delay: 150ms;
  overflow: hidden;
}
.codemirror.content{
  opacity: 1;
  height: 76vh;
  top: 14vh;
  visibility: visible;
}
.codemirror.landing, .codemirror.supporters{
  opacity: 0;
  height: 0vh;
  top: 98vh;
  visibility: hidden;
}
.browser{
  z-index: 9;
  border-radius: 5px;
  background-color: #42b983;
  transition: all 1s ease-in-out;
  overflow: hidden;
  vertical-align: bottom;
  width: 100%;
  height: 80vh;
}
.browser.landing, .browser.supporters{
  height: 0vh;
  top: 98vh;
  visibility: hidden;
}
.shadow {
  box-shadow: 0 .5rem 1rem rgba(0,0,0,.10)!important;
}
.browser-container{
  position: absolute;
  width: 54%;
  margin: 1%;
  margin-left: 45%;
  transition: all 1s ease-in-out;
  vertical-align: bottom;
  background-color: transparent;
  box-shadow: 0rem -1.5rem 2rem rgb(0 0 0 / 10%);
}
.browser-container.content{
  opacity: 1;
  height: 80vh;
  transition-delay: 300ms;
  top: 12vh;
  visibility: visible;
}
.browser-container.landing, .browser-container.supporters{
  opacity: 0;
  height: 0vh;
  top: 98vh;
  visibility: hidden;
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

@media (max-width: 1200px) {
}
@media (max-width: 992px) {
  body {
    height: unset;
    overflow: unset;
  }

  .codemirror{
    position: unset;
    width: 92%;
    margin-left: 4%;
    margin-right: 4%;
  }

  .browser-container{
    width: 98%;
    margin: 1%;
  }

  .browser-container.content{
    top: calc(15vh + 76vh - 1vh);
    transition-delay: 600ms;
  }

}
@media (max-width: 768px) {
  body {
    height: unset;
    overflow: unset;
  }

}
@media (max-width: 576px) {
  body {
    height: unset;
    overflow: unset;
  }

  .browser-container.content{
    top: calc(min(15vh, 25vw) + 76vh - 1.5vh);
  }

  .fab-icon-holder .bi {
    font-size: 1rem;
  }
  .fab-icon-holder {
    height: 40px;
  }
  .fab-options {
    bottom: 40px;
  }
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
