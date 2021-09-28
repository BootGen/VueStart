<template>
  <div class="h-100">
    <div class="d-flex justify-content-center align-items-center jumbotron" :class="{ 'landing': !showNav, 'content' : showNav }">
      <div>
        <h1>Start Vue!</h1>
        <div class="d-flex align-items-center jumbo-text" :class="{ 'landing': !showNav, 'content' : showNav }">
          <div>
            <p class="lead text-justify">
              Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.
            </p>
            <button class="btn btn-outline-primary rounded-pill m-1" @click="showNav = !showNav">Go!</button>
          </div>
        </div>
        <div class="slogen-text" :class="{ 'landing': !showNav, 'content' : showNav }">
          <p class="lead text-justify">
            Lorem Ipsum is simply dummy text of the printing and typesetting industry.
          </p>
        </div>
      </div>
      <vueuen :type="$store.state.vueuenType"></vueuen>
    </div>
    <div class="d-flex menu justify-content-between" :class="{ 'landing': !showNav, 'content' : showNav, }">
      <span></span>
      <span></span>
      <button type="button" class="btn btn-outline-primary rounded-pill m-1 btn-sm" @click="download"><span class="bi bi-download" aria-hidden="true"></span></button>
    </div>
    <div class="position-absolute end-50 codemirror custom-card" :class="{ 'landing': !showNav, 'content' : showNav, }">
      <code-mirror v-model="json"></code-mirror>
    </div>
    <div class="position-absolute start-50 browser custom-card" :class="{ 'landing': !showNav, 'content' : showNav, }">
      <browser-frame v-model="appUrl"></browser-frame>
    </div>
    <div class="d-flex position-absolute footer" :class="{ 'landing': !showNav, 'content' : showNav, }">
      <p>Powered by <a href="https://bootgen.com" target="_blank">BootGen</a> | Created by <a href="https://codesharp.hu" target="_blank">Code Sharp Kft.</a></p>
    </div>
  </div>
</template>

<script>
import axios from 'axios';
import { defineComponent, ref, watchEffect } from 'vue';
import { getSchema } from '../utils/Schema';
import { debounce } from '../utils/Helper';
import Vueuen from '../components/Vueuen.vue';
import CodeMirror from '../components/CodeMirror.vue';
import BrowserFrame from '../components/BrowserFrame.vue'

export default defineComponent({
  name: 'LandingPage',
  components: { Vueuen, CodeMirror, BrowserFrame },
  setup() {
    const json = ref("");
      const jsonSchema = ref('');

    function saveToLocalStorage(newValue) {
      try {
        let obj = JSON.parse(newValue);
        let minimized = JSON.stringify(obj);
        let oldValue = localStorage.getItem('json');
        if (minimized != oldValue) {
            localStorage.setItem('json', minimized);
        }
      } catch (e) {
        console.log(e)
      }
    }
    async function getProjectContentFromServer(name) {
      const data = (await axios.get(`/${name}.json`, {responseType: 'text'})).data;
      if (typeof data === 'string')
        return data;
      return JSON.stringify(data);
    }
    getProjectContentFromServer('example_input').then( (content) => {
      json.value = content;
      jsonSchema.value = ref(getSchema(JSON.parse(json.value)));
      let debouncedGenerate = debounce(generate, 1000);
      watchEffect(() => {
        const newSchema = getSchema(JSON.parse(json.value));
        if(JSON.stringify(newSchema) != JSON.stringify(jsonSchema.value)) {
          jsonSchema.value = newSchema
          debouncedGenerate()
        } else {
          saveToLocalStorage(json.value);
        }
      })
    })
    window.addEventListener('storage', () => {
      json.value = localStorage.getItem('json').toString();
    });
    const showNav = ref(false);
    const appUrl = ref("http://localhost:8080/sites/default/");
    async function generate() {
      const resp = await axios.post('http://localhost:8080/generate/editor', JSON.parse(json.value))
      saveToLocalStorage(json.value);
      appUrl.value = `http://localhost:8080/files/${resp.data.id}/index.html`;
    }

    async function download() {
      const response = await axios.post('http://localhost:8080/generate/editor/download', JSON.parse(json.value), {responseType: 'blob'});
      const fileURL = window.URL.createObjectURL(new Blob([response.data]));
      const fileLink = document.createElement('a');
      fileLink.href = fileURL;
      fileLink.target = '_blank';
      fileLink.setAttribute('download', 'editor.zip');
      document.body.appendChild(fileLink);
      fileLink.click();
    }

    return { showNav, json, appUrl, download}
  }
});

</script>

<style scoped>
  .btn {
    font-size: 1.5em;
  }
  .jumbotron {
    transition: all 1s ease-in-out;
    padding: 5vmin;
  }
  .jumbotron.landing {
    height: calc( 100vh - 2.5rem );
    background-color: white;
    transition-delay: 300ms;
    border-bottom-left-radius: 0;
  }
  .jumbotron.content {
    height: 15vh;
    background-color: #f1f1f1;
    border-bottom-left-radius: 20px;
  }
  .jumbo-text{
    transition: all 1s ease-in-out;
    overflow: hidden;
  }
  .jumbo-text.content{
    opacity: 0;
    height: 0rem;
    width: 80vw;
  }
  .jumbo-text.landing{
    opacity: 1;
    height: 18rem;
    width: 60vw;
  }
  .slogen-text{
    transition: all 1s ease-in-out;
    overflow: hidden;
  }
  .slogen-text.content{
    opacity: 1;
    height: 100%;
    visibility: visible;
  }
  .slogen-text.landing{
    opacity: 0;
    height: 0;
    visibility: hidden;
  }
  .menu{
    transition: all 1s ease-in-out;
    transition-delay: 150ms;
    margin: 1%;
    overflow: hidden;
  }
  .menu.content{
    opacity: 1;
    height: 4vh;
    visibility: visible;
  }
  .menu.landing{
    opacity: 0;
    height: 0vh;
    visibility: hidden;
  }
  .codemirror{
    width: 47%;
    margin: 1%;
    transition: all 1s ease-in-out;
    transition-delay: 150ms;
    overflow: hidden;
  }
  .codemirror.content{
    opacity: 1;
    height: calc( 75vh - 1rem );
    top: 20vh;
    visibility: visible;
  }
  .codemirror.landing{
    opacity: 0;
    height: 0vh;
    top: 90vh;
    visibility: hidden;
  }
  .browser{
    width: 47%;
    margin: 1%;
    transition: all 1s ease-in-out;
    overflow: hidden;
    vertical-align: bottom;
  }
  .browser.content{
    opacity: 1;
    height: calc( 75vh - 1rem );
    transition-delay: 300ms;
    top: 20vh;
    visibility: visible;
  }
  .browser.landing{
    opacity: 0;
    height: 0vh;
    top: 90vh;
    visibility: hidden;
  }
  .footer{
    right: 0;
    margin: 1%;
    transition: all 1s ease-in-out;
    overflow: hidden;
    vertical-align: bottom;
  }
  .footer.content{
    opacity: 1;
    height: 2.5rem;
    transition-delay: 500ms;
    top: calc( 100vh - 4rem );
    visibility: visible;
  }
  .footer.landing{
    opacity: 0;
    height: 0vh;
    top: 90vh;
    visibility: hidden;
  }
</style>
