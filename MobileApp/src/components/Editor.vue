<template>
  <settings :generateSettings="generateSettings" @save="saveSettings"></settings>
  <div class="custom-card shadow mt-4">
    <code-mirror v-model="json" :error="inputError" :isFixable="isFixable" @fixData="fixData" @hasSyntaxError="$emit('hasError', $event)"></code-mirror>
  </div>
  <div class="container mt-4">
    <div class="row flex-colum justify-content-center">
      <div id="generate-btn" class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" @click="generate(json)">
        <span class="bi bi-arrow-right" aria-hidden="true"></span>
        <span class="ps-2">Generate</span>
      </div>
    </div>
  </div>
  <browser-options :selected="selectedTab" :frontendMode="generateSettings.frontend" @select="selectTab"></browser-options>
  <div class="browser custom-card shadow mt-2">
    <browser-frame v-model="browserData"></browser-frame>
  </div>
</template>
<script>
import { defineComponent, ref, watch } from 'vue';
import CodeMirror from './CodeMirror.vue';
import BrowserFrame from './BrowserFrame.vue';
import BrowserOptions from './BrowserOptions.vue';
import Settings from './Settings.vue';
import axios from "axios";

export default defineComponent({
  components: { CodeMirror, BrowserFrame, BrowserOptions, Settings },
  props: {
    config: Object,
    loadedData: Object
  },
  emits: ['hasError', 'setVuecoon'],
  setup(props, context) {
    const inputError = ref(null);
    const isFixable = ref(false);
    const json = ref('');
    const selectedTab = ref(0);
    const browserData = ref({ page_url: '', source_url: '' });
    let generatedId = '';
    const generateSettings = ref({
      frontend: 'vanilla',
      isReadonly: true,
      color: '42b983',
      classSettings: []
    });
    
    watch(() => [props.loadedData], () => {
      if(window.location.pathname !== '/') {
        loadSharedLink();
      }
    });

    async function loadSharedLink(){
      if(props.loadedData) {
        json.value = JSON.stringify(props.loadedData.json);
        generateSettings.value.frontend = props.loadedData.frontendType;
        generateSettings.value.isReadonly = props.loadedData.editable;
        generateSettings.value.color = props.loadedData.color;
        generate(json.value);
      }
    }

    const keys = ['idtoken', 'tipIdx'];
    for (let [key, value] of Object.entries(localStorage)) {
      if(!keys.includes(key)) {
        localStorage.removeItem(key);
      }
    }

    window.addEventListener('storage', () => {
      json.value = localStorage.getItem(generatedId);
    });
    async function fixData() {
      const fixedJson = await axios.post('api/generate/fix', JSON.parse(json.value));
      json.value = JSON.stringify(fixedJson.data);
      await generate(json.value);
    }
    async function generate(data) {
      try {
        const request = {
          settings: generateSettings.value,
          data: JSON.parse(data)
        }
        let resp = await axios.post('api/generate', request, props.config);
        localStorage.removeItem(generatedId);
        generatedId = resp.data.id;
        saveToLocalStorage(data);
        if (selectedTab.value === 0) {
          browserData.value = {
            page_url: `api/files/${resp.data.id}/index.html`
          };
        } else {
          browserData.value = {
            source_url: `api/files/${resp.data.id}/app.js?display=true`
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
        } else {
          console.error(e);
        }
        context.emit('hasError', true);
      }
    }

    function saveToLocalStorage(newValue) {
      let obj = JSON.parse(newValue);
      let minimized = JSON.stringify(obj);
      let oldValue = localStorage.getItem(generatedId);
      if (minimized !== oldValue) {
        localStorage.setItem(generatedId, minimized);
      }
    }
    async function getProjectContentFromServer(name) {
      const data = (await axios.get(`/${name}.json`, {responseType: 'text', ...props.config})).data;
      if (typeof data === 'string')
        return data;
      return JSON.stringify(data);
    }

    getProjectContentFromServer('example_input').then( (content) => {
      json.value = content;
      generate(json.value);
    });

    function selectTab(mode) {
      selectedTab.value = mode;
      switch (selectedTab.value) {
        case 0:
          browserData.value = {
            page_url: `api/files/${generatedId}/index.html`
          };
          break;
        case 1:
          browserData.value = {
            source_url: `api/files/${generatedId}/app.js?display=true`
          };
          break;
      }
    }

    function saveSettings(settings) {
      if(generateSettings.value !== settings){
        generateSettings.value = settings;
        generate(json.value);
      }
    }

    return { json, inputError, generateSettings,
      fixData, isFixable, generate, selectedTab, selectTab, browserData,
      saveSettings }
  },
})
</script>

<style>
.custom-card {
  border-radius: 5px;
}
.fab-icon-holder {
  cursor: pointer;
  height: 40px;
  border-radius: 25px;
  background-color: #42b983;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #ffffff;
  padding: 1rem;
  box-shadow: 0 .5rem 1rem rgba(0,0,0,.10)!important;
  margin: 0.25rem;
}
.fab-icon-holder .bi{
  font-size: 1.2rem;
}
.fab-icon-holder:hover {
  background: #17a062;
}
.shadow {
  box-shadow: 0 .5rem 1rem rgba(0,0,0,.10)!important;
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
</style>
