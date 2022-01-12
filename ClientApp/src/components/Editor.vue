<template>
    <div class="codemirror custom-card" :class="{ 'landing': !showContent, 'content' : showContent, }">
      <code-mirror v-model="json" :error="inputError" :isFixable="isFixable" @fixData="fixData"></code-mirror>
    </div>
    <div class="browser-container" :class="{ 'landing': !showContent, 'content' : showContent, }">
      <div class="browser custom-card shadow">
        <browser-frame v-model="appUrl" :borderRadius="generateType === generateTypes.Editor">
          <div class="d-flex w-100 h-auto">
            <tab :title="generateTypes.Editor" icon="pencil" :showVr="generateType !== generateTypes.Editor && generateType !== generateTypes.View" @select="changeGeneratedMode(generateTypes.Editor)" :class="{ 'inactive': generateType !== generateTypes.Editor, 'active' : generateType === generateTypes.Editor, 'border-bottom-right' : generateType === generateTypes.View }"></tab>
            <tab :title="generateTypes.View" icon="eye" :showVr="generateType !== generateTypes.View && generateType !== generateTypes.Form" @select="changeGeneratedMode(generateTypes.View)" :class="{ 'inactive': generateType !== generateTypes.View, 'active' : generateType === generateTypes.View, 'border-bottom-right' : generateType === generateTypes.Form, 'border-bottom-left' : generateType === generateTypes.Editor }"></tab>
            <tab :title="generateTypes.Form" icon="file-earmark-code" :showVr="generateType !== generateTypes.Form" @select="changeGeneratedMode(generateTypes.Form)" :class="{ 'inactive': generateType !== generateTypes.Form, 'active' : generateType === generateTypes.Form, 'border-bottom-left' : generateType === generateTypes.View }"></tab>
            <button type="button" class="btn-site inactive" :class="{ 'border-bottom-left' : generateType === generateTypes.Form }"><span class="bi bi-plus" aria-hidden="true"></span></button>
          </div>
        </browser-frame>
      </div>
      <div class="d-flex browser-buttons">
        <div class="fab-container">
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
        <div class="fab-container mx-2">
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
        <div id="download-btn" class="fab fab-icon-holder pulse-download-btn" @click="showDownloadPanel = true">
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
import {getSchema} from "@/utils/Schema";
import {debounce} from "@/utils/Helper";

export default defineComponent({
  components: { CodeMirror, BrowserFrame, Tab },
  props: {
    showContent: Boolean
  },
  setup() {
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
    window.addEventListener('storage', () => {
      json.value = localStorage.getItem('json').toString();
    });
    let idtoken = localStorage.getItem('idtoken');
    if (!idtoken) {
      idtoken = ''
      while (idtoken.length < 16)
        idtoken += Math.random().toString(36).substring(2);
      idtoken = idtoken.substring(0, 16);
      localStorage.setItem('idtoken', idtoken)
    }

    let config = {
      headers: {
        'idtoken': idtoken,
        'citation': document.referrer
      }
    }
    async function fixData() {
      const fixedJson = await axios.post('api/generate/fix', JSON.parse(json.value));
      json.value = JSON.stringify(fixedJson.data);
      await generate(json.value);
    }
    function changeGeneratedMode(type) {
      generateType.value = type
      generate(json.value)
    }
    function changeLayoutMode(type) {
      layoutMode.value = type
      generate(json.value)
    }
    async function download() {
      const response = await axios.post(`api/download/${generateType.value}/${layoutMode.value}`, JSON.parse(json.value), {responseType: 'blob', ...config});
      const fileURL = window.URL.createObjectURL(new Blob([response.data]));
      const fileLink = document.createElement('a');
      fileLink.href = fileURL;
      fileLink.target = '_blank';
      fileLink.setAttribute('download', `${generateType.value}.zip`);
      document.body.appendChild(fileLink);
      fileLink.click();
    }

    async function generate(data) {
      try {
        const resp = await axios.post(`api/generate/${generateType.value}/${layoutMode.value}`, JSON.parse(data), config);
        saveToLocalStorage(data);
        appUrl.value = `api/files/${resp.data.id}/index.html`;
        inputError.value = null;
        isFixable.value = false;
      } catch (e) {
        const response = e.response;
        if (response) {
          isFixable.value = !!response.data.fixable;
          inputError.value = response.data.error;
        }
      }
    }

    function saveToLocalStorage(newValue) {
      let obj = JSON.parse(newValue);
      let minimized = JSON.stringify(obj);
      let oldValue = localStorage.getItem('json');
      if (minimized !== oldValue) {
        localStorage.setItem('json', minimized);
      }
      const downloadButton = document.getElementById('download-btn');
      downloadButton.classList.add('pulse-download-btn');
      setTimeout(function(){
        downloadButton.classList.remove('pulse-download-btn');
      }, 2000);
    }
    async function getProjectContentFromServer(name) {
      const data = (await axios.get(`/${name}.json`, {responseType: 'text', ...config})).data;
      if (typeof data === 'string')
        return data;
      return JSON.stringify(data);
    }
    getProjectContentFromServer('example_input').then( (content) => {
      json.value = content;
      jsonSchema.value = getSchema(JSON.parse(json.value));
      generate(json.value);
      let debouncedGenerate = debounce(generate, 1000);
      watchEffect(() => {
        try {
          const newSchema = getSchema(JSON.parse(json.value));
          if(JSON.stringify(newSchema) !== JSON.stringify(jsonSchema.value)) {
            jsonSchema.value = newSchema;
            debouncedGenerate(json.value);
          } else {
            saveToLocalStorage(json.value);
          }
        } catch {
          const nop = () => {};
          nop()
        }
      })
    })
    return { json, inputError, appUrl, generateType, generateTypes, layoutMode, layoutModes,
      changeGeneratedMode, changeLayoutMode, download, fixData, isFixable }
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
  bottom: 70px;
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
.codemirror.landing{
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
.browser.landing{
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
.browser-container.landing{
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
    bottom: 50px;
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
