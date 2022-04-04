<template>
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
    <code-mirror v-model="json"  @hasSyntaxError="syntaxError" :class="{'h-90': alert.shown, 'h-100': !alert.shown}"></code-mirror>
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
        <browser-frame v-model="browserData" :config="config"  @refresh="pageRefresh" :borderRadius="selectedTab === 0">
          <div class="d-flex w-100 h-auto">
            <tab :title="frontendMode" :icon="frontendModeIcon" :class="{'active': selectedTab === 0, 'inactive': selectedTab !== 0, 'border-bottom-right': selectedTab === 1}" @select="selectedTab = 0"></tab>
            <tab title="index.html" icon="code" :class="{'active': selectedTab === 1, 'inactive': selectedTab !== 1, 'border-bottom-left': selectedTab === 0, 'border-bottom-right': selectedTab === 2}" @select="selectedTab = 1"></tab>
            <tab title="app.js" icon="code" :class="{'active': selectedTab === 2, 'inactive': selectedTab !== 2, 'border-bottom-left': selectedTab === 1}" @select="selectedTab = 2"></tab>
            <tab class="inactive" :class="{'border-bottom-left': selectedTab === 2}"></tab>
          </div>
        </browser-frame>
      </div>
      <div class="d-flex browser-buttons" :class="page">
        <div class="fab-container">
          <div class="fab fab-icon-holder">
            <span class="bi bi-lightbulb" aria-hidden="true"></span>
            <span class="ps-2">Example</span>
          </div>
          <ul class="fab-options">
            <li>
              <div class="fab-icon-holder" @click="loadTasksExample">
                <span class="bi bi-card-checklist" aria-hidden="true"></span>
                <span class="ps-2">Tasks</span>
              </div>
            </li>
            <li>
              <div class="fab-icon-holder" @click="loadBookingExample">
                <span class="bi bi-book-half" aria-hidden="true"></span>
                <span class="ps-2">Booking</span>
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
        <div class="fab-container mx-2">
          <div class="fab fab-icon-holder">
            <span class="bi bi-filetype-css" aria-hidden="true" v-if="frontendMode === frontendModes.Vanilla"></span>
            <span class="bi bi-bootstrap" aria-hidden="true" v-if="frontendMode === frontendModes.Bootstrap"></span>
            <span class="bi bi-wind" aria-hidden="true" v-if="frontendMode === frontendModes.Tailwind"></span>
            <span class="ps-2">Frontend</span>
          </div>
          <ul class="fab-options">
            <li>
              <div class="fab-icon-holder" @click="changeFrontendMode(frontendModes.Vanilla)">
                <span class="bi bi-filetype-css" aria-hidden="true"></span>
                <span class="ps-2">Vanilla</span>
              </div>
            </li>
            <li>
              <div class="fab-icon-holder" @click="changeFrontendMode(frontendModes.Bootstrap)">
                <span class="bi bi-bootstrap" aria-hidden="true"></span>
                <span class="ps-2">Bootstrap</span>
              </div>
            </li>
            <li>
              <div class="fab-icon-holder" @click="changeFrontendMode(frontendModes.Tailwind)">
                <span class="bi bi-wind" aria-hidden="true"></span>
                <span class="ps-2">Tailwind</span>
              </div>
            </li>
          </ul>
        </div>
        <div class="fab-container mx-2">
          <div class="fab fab-icon-holder">
            <span class="bi bi-pencil" aria-hidden="true" v-if="editable"></span>
            <span class="bi bi-eye" aria-hidden="true" v-else></span>
          </div>
          <ul class="fab-options">
            <li>
              <div class="fab-icon-holder" @click="editableChanged(true)">
                <span class="bi bi-pencil" aria-hidden="true"></span>
                <span class="ps-2">Editable</span>
              </div>
            </li>
            <li>
              <div class="fab-icon-holder" @click="editableChanged(false)">
                <span class="bi bi-eye" aria-hidden="true"></span>
                <span class="ps-2">Read-only</span>
              </div>
            </li>
          </ul>
        </div>
        <div id="color-picker-btn" class="fab fab-icon-holder" @click="triggerColorPicker">
          <input type="color" class="form-control form-control-color position-absolute" id="colorInput" v-model="selectedColor" title="Choose your color">
          <span class="bi bi-palette" aria-hidden="true"></span>
        </div>
        <div id="download-btn" class="fab fab-icon-holder pulse-download-btn mx-2" @click="onDownloadClicked">
          <span class="bi bi-download" aria-hidden="true"></span>
        </div>
      </div>
    </div>
</template>
<script>
import {computed, defineComponent, ref, watch} from 'vue';
import CodeMirror from './CodeMirror.vue';
import BrowserFrame from './BrowserFrame.vue'
import Tab from "@/components/Tab";
import axios from "axios";
import {getSchema} from "@/utils/Schema";
import {debounce} from "@/utils/Helper";
import {validateJson} from '@/utils/Validate';
import Tip from '@/utils/Tip'
import ModalPanel from "@/components/ModalPanel";

export default defineComponent({
  components: { CodeMirror, BrowserFrame, Tab, ModalPanel },
  props: {
    page: String,
    config: Object
  },
  emits: ['download', 'hasError', 'setVuecoon', 'success'],
  setup(props, context) {
    const inputError = ref(null);
    const json = ref('');
    const jsonSchema = ref(getSchema({}));
    const selectedTab = ref(0);
    const editable = ref(true);
    const browserData = ref({ page_url: '', source_url: '' });
    const generatedId = ref('');
    const syntaxErr = ref(false);
    const showWarningPanel = ref(false);
    const warnings = ref([]);
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
    const tip = new Tip()
    function seturl() {
      switch (selectedTab.value) {
        case 0:
          browserData.value = {
            page_url: `api/files/${generatedId.value}/index.html`
          };
          break;
        case 1:
          browserData.value = {
            source_url: `api/files/${generatedId.value}/index.html?display=true`
          };
          break;
        case 2:
          browserData.value = {
            source_url: `api/files/${generatedId.value}/app.js?display=true`
          };
          break;
      }
    }
    watch(selectedTab, seturl);
    const frontendModeIcon = computed(() =>{
      switch (frontendMode.value) {
        case frontendModes.Tailwind:
          return 'wind';
        case frontendModes.Vanilla:
          return 'filetype-css';
        case frontendModes.Bootstrap:
          return 'bootstrap';
      }
      return 'filetype-css';
    })
    const frontendModes = {
      Vanilla: 'vanilla',
      Bootstrap: 'bootstrap',
      Tailwind: 'tailwind'
    }
    const frontendMode = ref(frontendModes.Vanilla);
    const tempColor = ref('42b983');
    const selectedColor = ref('#42b983');

    window.addEventListener('storage', () => {
      json.value = localStorage.getItem('json');
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

    function changeFrontendMode(type) {
      frontendMode.value = type;
      if (tip.typeChanged())
        context.emit('success')
      showTip()
      generate(json.value)
    }
    async function generate(data) {
      try {
        const resp = ref(null);
        if(editable.value) {
          resp.value = await axios.post(`api/generate/${frontendMode.value}/table-editable/${tempColor.value}`, JSON.parse(data), props.config);
        } else {
          resp.value = await axios.post(`api/generate/${frontendMode.value}/table/${tempColor.value}`, JSON.parse(data), props.config);
        }
        saveToLocalStorage(data);
        generatedId.value = resp.value.data.id;
        seturl();
        inputError.value = null;
        if (resp.value.data.warnings && resp.value.data.warnings.length > 0) {
          warnings.value = resp.value.data.warnings;
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
        } else {
          warnings.value = [];
          if (!showTip()) {
            alert.value = noAlert;
          }
        }
        context.emit('hasError', false);
      } catch (e) {
        const response = e.response;
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
        context.emit('hasError', true);
      }
    }

    function saveToLocalStorage(newValue) {
      let obj = JSON.parse(newValue);
      let minimized = JSON.stringify(obj);
      let oldValue = localStorage.getItem('json');
      if (minimized !== oldValue) {
        localStorage.setItem('json', minimized);
        if (oldValue) {
          if (tip.modified())
            context.emit('success');
          showTip();
        }
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
    async function loadTasksExample() {
      context.emit('setVuecoon', 'loading');
      json.value = await getProjectContentFromServer('tasks_example_input');
      changeFrontendMode(frontendModes.Vanilla);
    }
    async function loadOrdersExample() {
      context.emit('setVuecoon', 'loading');
      json.value = await getProjectContentFromServer('orders_example_input');
      changeFrontendMode(frontendModes.Tailwind);
    }
    async function loadBookingExample() {
      context.emit('setVuecoon', 'loading');
      json.value = await getProjectContentFromServer('booking_example_input');
      changeFrontendMode(frontendModes.Bootstrap);
    }
    getProjectContentFromServer('orders_example_input').then( (content) => {
      json.value = content;
      jsonSchema.value = getSchema(JSON.parse(json.value));
      generate(json.value);
      function generateAndEmit(data) {
        generate(data);
        if (tip.generated())
          context.emit('success');
        showTip();
      }
      let debouncedGenerate = debounce(generateAndEmit, 1000);
      watch([tempColor, selectedColor, syntaxErr],() => {
        if ('#' + tempColor.value !== selectedColor.value) {
            document.getElementById('color-picker-btn').style.backgroundColor = selectedColor.value;
          if (!syntaxErr.value) {
            document.getElementById('color-picker-btn').style.backgroundColor = selectedColor.value;
            tempColor.value = selectedColor.value.slice(1, 7);
            setTextColor();
            debouncedGenerate(json.value);
          }
        }
      });
      watch(json, () => {
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
      if (tip.downloaded()) {
        context.emit('success');
        showGitHubCTA();
      }
      if(editable.value) {
        context.emit('download', `api/download/${frontendMode.value}/table-editable/${tempColor.value}`, `${frontendMode.value}.zip`);
      } else {
        context.emit('download', `api/download/${frontendMode.value}/table/${tempColor.value}`, `${frontendMode.value}.zip`);
      }
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
    function syntaxError (hasError, message) {
      syntaxErr.value = hasError;
      if (hasError) {
        showAlert(message, 'alert-danger');
      } else {
        if (!showTip()) {
          alert.value = noAlert;
        }
      }
      context.emit('hasError', hasError);
    }
    function editableChanged(b) {
      editable.value = b;
      generate(json.value);
    }

    return { json, inputError, frontendMode, frontendModes, selectedColor,
      changeFrontendMode, onDownloadClicked, triggerColorPicker, pageRefresh,
      selectedTab, frontendModeIcon, browserData, loadTasksExample, loadOrdersExample, loadBookingExample, syntaxError,
      alert, showWarningPanel, warnings, editable, editableChanged }
  },
})
</script>

<style>
body {
  height: 100%;
  overflow: hidden;
}
.h-90 {
  height: calc(100% - 4rem);
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
  transition: all 1s ease-in-out;
  transition-delay: 150ms;
}
.browser-buttons.landing {
  /*bottom: -20vh;*/
  opacity: 0;
}
.browser-buttons.content {
  /*bottom: -1rem;*/
  opacity: 1;
}

#download-btn {
  cursor: pointer;
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
  width: 45%;
  margin: 1%;
  transition: all 1s ease-in-out;
  transition-delay: 150ms;
  overflow: hidden;
}
.codemirror.content{
  opacity: 1;
  height: 80vh;
  top: 12vh;
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
  top: 12vh;
  width: 52%;
  margin: 1% 1% 1% 47%;
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
.browser-container.supporters{
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

  /*.browser-container.content{
    top: 95vh;
  }*/

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
