<template>
  <modal-panel v-model="showSettingsPanel">
    <generate-settings v-model="generateSettings" @cancel="showSettingsPanel = false" @save="saveSettings"></generate-settings>
  </modal-panel>
  <modal-panel v-model="showWarningPanel">
    <div class="alert alert-warning show px-3 py-3 my-0" role="alert">
      <button type="button" class="btn-close" style="float: right" @click="showWarningPanel=false"></button>
      <h3>Warnings</h3>
      <ul>
        <li v-for="(warning, idx) in warnings" :key="idx">{{warning}}</li>
      </ul>
    </div>
  </modal-panel>
  <div class="codemirror custom-card" :class="page">
    <code-mirror v-model="json" @setSyntaxError="setSyntaxError" @resetSyntaxError="resetSyntaxError" class="codemirror-content" :class="{'h90': alert.shown, 'h100': !alert.shown}"></code-mirror>
    <div class="d-flex codemirror-buttons" :class="[page, {'isalert': alert.shown}]">
      <div class="fab-container mx-1">
        <div class="fab fab-icon-holder">
          <span class="bi bi-lightbulb" aria-hidden="true"></span>
          <span class="ps-2">JSON Samples</span>
        </div>
        <ul class="fab-options">
          <li>
            <div class="fab-icon-holder" @click="loadTasksExample">
              <span class="bi bi-card-checklist" aria-hidden="true"></span>
              <span class="ps-2">Tasks</span>
            </div>
          </li>
          <li>
            <div class="fab-icon-holder" @click="loadOrdersExample">
              <span class="bi bi-cart-check" aria-hidden="true"></span>
              <span class="ps-2">Orders</span>
            </div>
          </li>
        </ul>
      </div>
    </div>
    <div class="my-1 col-12 alert alert-dismissible fade" :class="[alert.class, (alert.shown ? 'show' : '')]" role="alert" v-if="alert.shown">
      <div class="text-center">
        {{ alert.message }}
        <a :href="alert.action.href" :target="alert.action.target" class="alert-link" @click="alert.action.callback" v-if="alert.action.active">{{ alert.action.message }}</a>
      </div>
      <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close" @click="alert.shown=false"></button>
    </div>
  </div>
  <div class="browser-container" :class="page">
      <div class="browser custom-card shadow">
        <browser-frame v-model="browserData" :config="config" :undoable="undoStackIdx > 0" :redoable="undoStackIdx < undoStack.length-1"  @refresh="pageRefresh" @undo="undo" @redo="redo" :borderRadius="selectedTab === 0">
          <div class="d-flex w-100 h-auto">
            <tab :title="generateSettings.frontend" :img="selectedTab === 0 ? generateSettings.frontend + '_green' : generateSettings.frontend + '_white'" :class="{'active': selectedTab === 0, 'inactive': selectedTab !== 0, 'border-bottom-right': selectedTab === 1}" @select="selectedTab = 0"></tab>
            <tab title="index.html" icon="code" :class="{'active': selectedTab === 1, 'inactive': selectedTab !== 1, 'border-bottom-left': selectedTab === 0, 'border-bottom-right': selectedTab === 2}" @select="selectedTab = 1"></tab>
            <tab title="app.js" icon="code" :class="{'active': selectedTab === 2, 'inactive': selectedTab !== 2, 'border-bottom-left': selectedTab === 1}" @select="selectedTab = 2"></tab>
            <tab class="inactive" :class="{'border-bottom-left': selectedTab === 2}"></tab>
          </div>
        </browser-frame>
      </div>
      <div class="d-flex browser-buttons" :class="page">
        <div id="settings-btn" class="fab fab-icon-holder mx-1" @click="onSettingsClicked">
          <span class="bi bi-gear" aria-hidden="true"></span>
            <span class="ps-2">Settings</span>
        </div>
        <div id="share-btn" class="fab fab-icon-holder mx-1" :class="{'disable-share': shareLinkOnClipboard}" @click="share">
          <span class="bi bi-share" aria-hidden="true"></span>
          <span class="ps-2">Share</span>
          <span class="copied">Copied!</span>
        </div>
        <div id="download-btn" class="fab fab-icon-holder pulse-download-btn mx-1" @click="onDownloadClicked">
          <span class="bi bi-download" aria-hidden="true"></span>
            <span class="ps-2">Download Code</span>
        </div>
      </div>
    </div>
</template>
<script>
import {defineComponent, reactive, ref, watch} from 'vue';
import CodeMirror from './CodeMirror.vue';
import BrowserFrame from './BrowserFrame.vue'
import GenerateSettings from './GenerateSettings.vue'
import Tab from "@/components/Tab";
import axios from "axios";
import {getSchema} from "@/utils/Schema";
import {debounce} from "@/utils/Helper";
import {validateJson} from '@/utils/Validate';
import Tip from '@/utils/Tip'
import ModalPanel from "@/components/ModalPanel";

export default defineComponent({
  components: { CodeMirror, BrowserFrame, Tab, ModalPanel, GenerateSettings },
  props: {
    page: String,
    config: Object,
    loadedData: Object
  },
  emits: ['download', 'generationFailed', 'generationSuccess', 'setVuecoon', 'success'],
  setup(props, context) {
    const showSettingsPanel = ref(false);
    const generateSettings = ref({
      frontend: 'vanilla',
      isReadonly: false,
      color: '42b983',
      classSettings: []
    });
    const inputError = ref(null);
    const json = ref('');
    let sharedJson = '';
    let sharedLink = '';
    const jsonSchema = ref(getSchema({}));
    const selectedTab = ref(0);
    const browserData = ref({ page_url: '', source_url: '' });
    const showWarningPanel = ref(false);
    const warnings = ref([]);
    const undoStack = ref([]);
    const undoStackIdx = ref(-1);
    let generatedId = '';
    let isGenerating = false;
    const shareLinkOnClipboard = ref(false);
    let noAction = {
      active: false,
      href: 'javascript:void(0)',
      target: '_self',
      message: '',
      callback() {}
    };
    const noAlert = {
      shown: false,
      message: '',
      class: 'alert-primary',
      action: noAction
    }
    const alert = ref(noAlert);
    const tip = new Tip();

    const keys = ['idtoken', 'tipIdx'];
    for (let [key, value] of Object.entries(localStorage)) {
      if(!keys.includes(key)) {
        localStorage.removeItem(key);
      }
    }

    function seturl() {
      switch (selectedTab.value) {
        case 0:
          browserData.value = {
            page_url: `api/files/${generatedId}/index.html`
          };
          break;
        case 1:
          browserData.value = {
            source_url: `api/files/${generatedId}/index.html?display=true`
          };
          break;
        case 2:
          browserData.value = {
            source_url: `api/files/${generatedId}/app.js?display=true`
          };
          break;
      }
    }
    watch(selectedTab, seturl);

    window.addEventListener('storage', () => {
      const item = localStorage.getItem(generatedId);
      if (item)
        json.value = item;
    });
    async function fixData() {
      alert.value = noAlert;
      const fixedJson = await axios.post('api/generate/fix', JSON.parse(json.value));
      json.value = JSON.stringify(fixedJson.data);
      await generate(json.value);
    }

    function showAlert(msg, kind) {
      alert.value = {
        shown: true,
        message: msg,
        class: kind,
        action: noAction
      }
    }

    function showGitHubCTA() {
      alert.value = {
        shown: true,
        message: 'If you like this project, please give us a star on',
        class: 'alert-success',
        action: {
          active: true,
          href: 'https://github.com/BootGen/VueStart',
          target: '_blank',
          message: 'GitHub!',
          callback: () => {
            alert.value = noAlert
          }
        }
      }
    }

    function showTip() {
      const msg = tip.getTip();
      if (msg) {
        alert.value = {
          shown: true,
          message: msg,
          class: 'alert-primary',
          action: {
            active: true,
            href: 'javascript:void(0)',
            target: '_self',
            message: 'Hide tips.',
            callback() {
              tip.hideTips();
              setTimeout(showGitHubCTA, 500)
            }
          }
        }
        return true;
      }
      return false;
    }

    async function generate(data) {
      try {
        tryGenerate(data);
      } catch (e) {
        handleGenerationError(e.response);
      }
    }

    async function tryGenerate(data) {
      let resp = await axios.post(`api/generate`, { settings: generateSettings.value, data: JSON.parse(data) }, props.config);
      generateSettings.value.classSettings = resp.data.settings;
      localStorage.removeItem(generatedId);
      generatedId = resp.data.id;
      saveToLocalStorage(data);
      seturl();
      jsonSchema.value = getSchema(JSON.parse(data));
      inputError.value = null;
      if (resp.data.warnings && resp.data.warnings.length > 0) {
        setWarnings(resp.data.warnings);
      } else {
        resetWarnings();
      }
      context.emit('generationSuccess');
    }

    function handleGenerationError(response) {
      if (response) {
        alert.value = {
          shown: true,
          class: 'alert-danger',
          message: response.data.error,
          action: {
            target: '_self',
            active: !!response.data.fixable,
            href: 'javascript:void(0)',
            message: 'Fix it!',
            callback: fixData
          }
        }
        inputError.value = response.data.error;
      } else {
        console.error(e);
      }
      context.emit('generationFailed');
    }

    function setWarnings(warnings) {
      warnings.value = warnings;
      alert.value = {
        shown: true,
        class: 'alert-warning',
        message: 'Generation succeeded with ',
        action: {
          target: '_self',
          active: true,
          href: 'javascript:void(0)',
          message: 'warnings.',
          callback() {
            showWarningPanel.value = true;
          }
        }
      }
    }

    function resetWarnings() {
      warnings.value = [];
      if (!showTip()) {
        alert.value = noAlert;
      }
    }

    function saveToLocalStorage(newValue) {
      let obj = JSON.parse(newValue);
      let minimized = JSON.stringify(obj);
      let oldValue = localStorage.getItem(generatedId);
      if (minimized !== oldValue) {
        localStorage.setItem(generatedId, minimized);
        if (oldValue) {
          if (tip.modified())
            context.emit('success');
          showTip();
        }
      }
      if(minimized !== undoStack.value[undoStackIdx.value]) {
        undoStackIdx.value++;
        undoStack.value.splice(undoStackIdx.value);
        undoStack.value.push(minimized)
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

    async function loadTasksExample() {
      context.emit('setVuecoon', 'loading');
      json.value = await getProjectContentFromServer('tasks_example_input');
      await generate(json.value);
    }
    async function loadOrdersExample() {
      context.emit('setVuecoon', 'loading');
      json.value = await getProjectContentFromServer('orders_example_input');
      await generate(json.value);
    }
    getProjectContentFromServer('orders_example_input').then( (content) => {
      json.value = content;
      jsonSchema.value = getSchema(JSON.parse(json.value));
      generate(json.value);
      watch(json, () => {
        try {
          trySaveJson();
        } catch (e) {
          handleJsonSaveError(e);
        }
      })
    });

    function trySaveJson() {
      let debouncedGenerate = debounce(generateAndEmit, 1000);
      if (validateJson(json.value).error)
        return;
      if(JSON.stringify(getSchema(JSON.parse(json.value))) !== JSON.stringify(jsonSchema.value)) {
        isGenerating = true;
        debouncedGenerate(json.value);
      } else if(!isGenerating && generatedId !== '') {
        saveToLocalStorage(json.value);
        inputError.value = null;
      }
    }

    function generateAndEmit(data) {
      generate(data);
      if (tip.generated())
        context.emit('success');
      showTip();
      isGenerating = false
    }

    function handleJsonSaveError(e) {
      if (e.schemaError) {
        inputError.value = e.schemaError;
      } else {
        console.log(e)
      }
    }

    function onDownloadClicked() {
      if (tip.downloaded()) {
        context.emit('success');
        showGitHubCTA();
      }
      context.emit('download', `${generateSettings.value.frontend}.zip`, generateSettings.value, json.value);
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
    function setSyntaxError (message) {
      showAlert(message, 'alert-danger');
      context.emit('generationFailed');
    }
    function resetSyntaxError() {
      context.emit('generationSuccess');
      if (!showTip()) {
        alert.value = noAlert;
      }
    }
    function undo() {
      if(undoStackIdx.value > 0) {
        undoStackIdx.value--;
        json.value = undoStack.value[undoStackIdx.value];
      }
    }
    function redo() {
      if(undoStackIdx.value < undoStack.value.length-1) {
        undoStackIdx.value++;
        json.value = undoStack.value[undoStackIdx.value];
      }
    }
    async function share() {
      let request = {
        settings: generateSettings.value,
        data: JSON.parse(json.value)
      }
      let shareableJson = JSON.stringify(json.value);
      if(shareableJson !== sharedJson) {
        sharedLink = await axios.post('api/share', request);
        sharedJson = shareableJson;
      }
      navigator.clipboard.writeText(window.location.origin + '/' + sharedLink.data.hash);
      shareLinkOnClipboard.value = true;
      setTimeout(()=> {
        shareLinkOnClipboard.value = false;
      }, 800);
    }

    watch(() => [props.loadedData], () => { 
      if(window.location.pathname !== '/' && window.location.pathname !== '/supporters' && window.location.pathname !== '/editor') {
        loadSharedLink();
      }
    });
    async function loadSharedLink(){
      if(props.loadedData) {
        generateSettings.value = {...props.loadedData.settings}
        json.value = JSON.stringify(props.loadedData.data);
      }
    }
    function onSettingsClicked() {
      showSettingsPanel.value = true;
    }

    function saveSettings() {
      showSettingsPanel.value = false;
      generate(json.value);
    }

    return { json, inputError,
      onDownloadClicked, pageRefresh,
      selectedTab, browserData, loadTasksExample, loadOrdersExample, setSyntaxError, resetSyntaxError,
      alert, showWarningPanel, warnings, undo, redo, undoStackIdx, undoStack, share, shareLinkOnClipboard,
      showSettingsPanel, onSettingsClicked, generateSettings, saveSettings }
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
.copied {
  transition: all 1s ease-in-out;
  transition-delay: 10ms;
  position: absolute;
  opacity: 0;
  color: #42b983;
  bottom: 1rem;
  display: flex;
  justify-content: center;
}
.disable-share .copied {
  opacity: 1;
  bottom: 4rem;
}
.fab-icon-holder .bi {
  font-size: 1.5rem;
}
.fab-icon-holder img {
  width: 1.5rem;
  height: 1.5rem;
}
.fab-icon-holder:hover {
  background: #17a062;
}

.fab {
  height: 50px;
  background: #42b983;
  cursor: pointer;
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
  transition: all 1s ease-in-out;
  transition-delay: 150ms;
}
.browser-buttons.landing {
  opacity: 0;
}
.browser-buttons.content {
  opacity: 1;
}
.codemirror-buttons {
  position: absolute;
  bottom: -1rem;
  left: 2rem;
  font-size: 1rem!important;
  z-index: 99;
  transition: all 1s ease-in-out;
  transition-delay: 150ms;
}
.codemirror-buttons.landing {
  opacity: 0;
}
.codemirror-buttons.content {
  opacity: 1;
}
.codemirror-buttons.content.isalert {
  bottom: -2rem!important;
}
.codemirror-content {
  transition: all 1s ease-in-out;
  transition-delay: 150ms;
}
.codemirror-content.h90 {
  height: calc(100% - 4rem);
}
.codemirror-content.h100 {
  height: 100%;
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
  width: 48.5%;
  margin: 1%;
  transition: all 1s ease-in-out;
  transition-delay: 150ms;
}
.codemirror.content{
  opacity: 1;
  height: 80vh;
  top: 12vh;
  visibility: visible;
}
.codemirror.landing, .codemirror.supporters, .codemirror.notfound {
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
.browser.landing, .browser.supporters, .browser.notfound {
  height: 0vh;
  top: 98vh;
  visibility: hidden;
}
.shadow {
  box-shadow: 0 .5rem 1rem rgba(0,0,0,.10)!important;
}
.browser-container{
  position: absolute;
  top: 12vh;
  width: 48.5%;
  margin: 1% 1% 1% 50.5%;
  transition: all 1s ease-in-out;
  vertical-align: bottom;
  background-color: transparent;
  box-shadow: 0rem -1.5rem 2rem rgb(0 0 0 / 10%);
}
.browser-container.content{
  opacity: 1;
  height: 80vh;
  transition-delay: 300ms;
  visibility: visible;
}
.browser-container.supporters, .browser-container.notfound {
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
    transition-delay: 600ms;
  }
  
  .browser-container.landing {
    top: 32vh;
  }

  .browser-container.landing > .browser {
    height: 64vh;
  }

  .browser-container.content{
    top: 95vh;
  }
  .codemirror-buttons {
    left: 3rem;
    bottom: 6vh!important;
  }
  .codemirror-buttons.content.isalert {
    bottom: 8vh!important;
  }
}
@media (max-width: 768px) {
  body {
    height: unset;
    overflow: unset;
  }
  .codemirror-buttons.content.isalert {
    bottom: 11vh!important;
  }
}
@media (max-width: 576px) {
  body {
    height: unset;
    overflow: unset;
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
  .browser-container.content {
    top: 92vh;
  }
  .codemirror-buttons {
    right: 3rem;
    bottom: 12vh!important;
  }
  .codemirror-buttons.content.isalert {
    bottom: 16vh!important;
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
