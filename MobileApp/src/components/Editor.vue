<template>
    <div class="custom-card shadow mt-4">
      <code-mirror v-model="json" :error="inputError" :isFixable="isFixable" @fixData="fixData" @hasSyntaxError="$emit('hasError', $event)"></code-mirror>
    </div>
    <div class="container mt-4">
      <div class="row flex-colum justify-content-center">
        <div id="color-picker-btn" class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" @click="triggerColorPicker">
          <input type="color" class="form-control form-control-color position-absolute" id="colorInput" v-model="selectedColor" title="Choose your color">
          <span class="bi bi-palette" aria-hidden="true"></span>
          <span class="ps-2">Color</span>
        </div>
        <div id="generate-btn" class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" @click="generate(json)">
          <span class="bi bi-arrow-right" aria-hidden="true"></span>
          <span class="ps-2">Generate</span>
        </div>
      </div>
    </div>
    <browser-options :selected="selectedTab" :frontendMode="frontendMode" @select="selectTab"></browser-options>
    <div class="browser custom-card shadow mt-2">
      <browser-frame v-model="browserData"></browser-frame>
    </div>
</template>
<script>
import { defineComponent, ref, watchEffect } from 'vue';
import CodeMirror from './CodeMirror.vue';
import BrowserFrame from './BrowserFrame.vue';
import BrowserOptions from './BrowserOptions.vue';
import axios from "axios";
import { getSchema } from "@/utils/Schema";

export default defineComponent({
  components: { CodeMirror, BrowserFrame, BrowserOptions },
  props: {
    config: Object,
    frontendMode: String,
    editable: Boolean
  },
  emits: ['hasError', 'setVuecoon'],
  setup(props, context) {
    const inputError = ref(null);
    const isFixable = ref(false);
    const json = ref('');
    const jsonSchema = ref(getSchema({}));
    const tempColor = ref('42b983');
    const selectedColor = ref('#42b983');
    const frontendMode = ref(props.frontendMode);
    const selectedTab = ref(0);
    const browserData = ref({ page_url: '', source_url: '' });
    const generatedId = ref('');

    const views = {
      View: 'view',
      App: 'app.js',
      Index: 'index.html'
    }
    const selectedView = ref(views.View);
    
    window.addEventListener('storage', () => {
      json.value = localStorage.getItem('json');
    });
    async function fixData() {
      const fixedJson = await axios.post('api/generate/fix', JSON.parse(json.value));
      json.value = JSON.stringify(fixedJson.data);
      await generate(json.value);
    }
    async function generate(data) {
      try {
        const resp = ref(null);
        if(props.editable) {
          resp.value = await axios.post(`api/generate/${frontendMode.value}/table-editable/${tempColor.value}`, JSON.parse(data), props.config);
        } else {
          await axios.post(`api/generate/${frontendMode.value}/table/${tempColor.value}`, JSON.parse(data), props.config);
        }
        saveToLocalStorage(data);
        generatedId.value = resp.value.data.id;
        if (selectedTab.value === 0) {
          browserData.value = {
            page_url: `api/files/${resp.value.data.id}/index.html`
          };
        } else {
          browserData.value = {
            source_url: `api/files/${resp.value.data.id}/app.js?display=true`
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
      let oldValue = localStorage.getItem('json');
      if (minimized !== oldValue) {
        localStorage.setItem('json', minimized);
      }
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
      watchEffect(() => {
        if(props.frontendMode !== frontendMode.value) {
          frontendMode.value = props.frontendMode;
          generate(json.value);
        }
        if('#' + tempColor.value !== selectedColor.value) {
          tempColor.value = selectedColor.value.slice(1, 7);
          document.getElementById('color-picker-btn').style.backgroundColor = selectedColor.value;
          setTextColor();
        }
      })
    });
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

    function selectTab(mode) {
      selectedTab.value = mode;
      switch (selectedTab.value) {
        case 0:
          browserData.value = {
            page_url: `api/files/${generatedId.value}/index.html`
          };
          break;
        case 1:
          browserData.value = {
            source_url: `api/files/${generatedId.value}/app.js?display=true`
          };
          break;
      }
    }

    return { json, inputError, selectedColor,
      fixData, isFixable, triggerColorPicker, generate, views, selectedView, selectedTab, selectTab, browserData }
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
input#colorInput {
  width: 100px;
  opacity: 0;
  padding-top: 25px;
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
