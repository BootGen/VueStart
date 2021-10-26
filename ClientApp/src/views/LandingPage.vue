<template>
  <div class="h-100">
    <div class="d-flex justify-content-center align-items-center jumbotron" :class="{ 'landing': !showNav, 'content' : showNav }">
      <img class="vuecoon img-fluid" alt="Vuecoon" :src="require(`../assets/vuecoon_${$store.state.vuecoonType}.webp`)" :class="{ 'landing': !showNav, 'content' : showNav, }">
      <div class="jumbo-text-full" :class="{ 'landing': !showNav, 'content' : showNav }">
        <div class="d-flex align-items-center">
          <img class="vue_logo" alt="vue" :src="require(`../assets/vue_logo.webp`)">
          <h1 class="title">ue Start!</h1>
        </div>
        <div class="d-flex align-items-center jumbo-text" :class="{ 'landing': !showNav, 'content' : showNav }">
          <div>
            <p class="lead text-justify">
              Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.
            </p>
            <button class="btn fill-btn rounded-pill m-1 btn-lg" @click="showNav = !showNav">Start!</button>
          </div>
        </div>
        <div class="slogen-text" :class="{ 'landing': !showNav, 'content' : showNav }">
          <p class="lead text-justify">
            Lorem Ipsum is simply dummy text of the printing and typesetting industry.
          </p>
        </div>
      </div>
    </div>

    <div class="codemirror custom-card shadow-lg" :class="{ 'landing': !showNav, 'content' : showNav, }">
      <code-mirror v-model="json"></code-mirror>
    </div>
    <div class="browser custom-card shadow-lg" :class="{ 'landing': !showNav, 'content' : showNav, }">
      <browser-frame v-model="appUrl" :borderRadius="generateType == generateTypes.Editor">
        <div class="d-flex w-100 justify-content-between h-auto">
          <tab title="Editor" icon="pencil" :showVr="generateType != generateTypes.Editor && generateType != generateTypes.View" @select="changeGeneratedMode(generateTypes.Editor)" :class="{ 'inactive': generateType != generateTypes.Editor, 'active' : generateType == generateTypes.Editor, 'border-bottom-right' : generateType == generateTypes.View }"></tab>
          <tab title="View" icon="eye" :showVr="generateType != generateTypes.View && generateType != generateTypes.Form" @select="changeGeneratedMode(generateTypes.View)" :class="{ 'inactive': generateType != generateTypes.View, 'active' : generateType == generateTypes.View, 'border-bottom-right' : generateType == generateTypes.Form, 'border-bottom-left' : generateType == generateTypes.Editor }"></tab>
          <tab title="Form" icon="file-earmark-code" :showVr="generateType != generateTypes.Form" @select="changeGeneratedMode(generateTypes.Form)" :class="{ 'inactive': generateType != generateTypes.Form, 'active' : generateType == generateTypes.Form, 'border-bottom-left' : generateType == generateTypes.View }"></tab>
          <button type="button" class="btn-site inactive plus" :class="{ 'border-bottom-left' : generateType == generateTypes.Form }"><span class="bi bi-plus" aria-hidden="true"></span></button>
        </div>
      </browser-frame>
      <button type="button" id="download-btn" class="btn fill-btn rounded-pill btn-lg" @click="download"><span>Download Application </span><span class="bi bi-download" aria-hidden="true"></span></button>
    </div>
    <div class="d-flex footer" :class="{ 'landing': !showNav, 'content' : showNav, }">
      <p>Powered by <a href="https://bootgen.com" target="_blank">BootGen</a> | Created by <a href="https://codesharp.hu" target="_blank">Code Sharp Kft.</a></p>
    </div>
  </div>
</template>

<script>
import axios from 'axios';
import { defineComponent, ref, watchEffect } from 'vue';
import { getSchema } from '../utils/Schema';
import { debounce } from '../utils/Helper';
import CodeMirror from '../components/CodeMirror.vue';
import BrowserFrame from '../components/BrowserFrame.vue'
import Tab from '../components/Tab.vue'

export default defineComponent({
  name: 'LandingPage',
  components: { CodeMirror, BrowserFrame, Tab },
  setup() {
    const json = ref("");
    const jsonSchema = ref('');
    const generateTypes = {
      View: "view",
      Form: "form",
      Editor: "editor",
    }
    const generateType = ref(generateTypes.Editor);

    function saveToLocalStorage(newValue) {
      try {
        let obj = JSON.parse(newValue);
        let minimized = JSON.stringify(obj);
        let oldValue = localStorage.getItem('json');
        if (minimized != oldValue) {
            localStorage.setItem('json', minimized);
        }
        document.getElementById('download-btn').classList.add('pulse-download-btn');
        setTimeout(function(){ 
          document.getElementById('download-btn').classList.remove('pulse-download-btn');
        }, 3000);
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
        try {
          const newSchema = getSchema(JSON.parse(json.value));
          if(JSON.stringify(newSchema) != JSON.stringify(jsonSchema.value)) {
            jsonSchema.value = newSchema
            debouncedGenerate()
          } else {
            saveToLocalStorage(json.value);
          }
        } catch {
          const nop = () => {};
          nop()
        }
      })
    })
    window.addEventListener('storage', () => {
      json.value = localStorage.getItem('json').toString();
    });
    const showNav = ref(false);
    const appUrl = ref("http://localhost:8080/sites/default/");
    async function generate() {
      const resp = await axios.post(`http://localhost:8080/generate/${generateType.value}`, JSON.parse(json.value));
      saveToLocalStorage(json.value);
      appUrl.value = `http://localhost:8080/files/${resp.data.id}/index.html`;
    }
    function changeGeneratedMode(type) {
      generateType.value = type
      generate()
    }
    async function download() {
      const response = await axios.post(`http://localhost:8080/generate/${generateType.value}/download`, JSON.parse(json.value), {responseType: 'blob'});
      const fileURL = window.URL.createObjectURL(new Blob([response.data]));
      const fileLink = document.createElement('a');
      fileLink.href = fileURL;
      fileLink.target = '_blank';
      fileLink.setAttribute('download', `${generateType.value}.zip`);
      document.body.appendChild(fileLink);
      fileLink.click();
    }

    return { showNav, json, appUrl, download, generateType, generateTypes, changeGeneratedMode}
  }
});

</script>

<style>
  .fill-btn {
    color: #ffffff;
    background-color: #42b983;
  }
  .fill-btn:hover {
    color: #ffffff;
    border-color: #17a062;
    background-color: #17a062;
  }
  #download-btn {
    position: absolute;
    bottom: 2rem;
    right: 2rem;
  }

  .pulse-download-btn {
    z-index: 9;
    box-shadow: 0 0 0 0 rgba(66, 185, 131, 0.7);
    -webkit-animation: pulse 1s infinite cubic-bezier(0.66, 0, 0, 1);
    -moz-animation: pulse 1s infinite cubic-bezier(0.66, 0, 0, 1);
    -ms-animation: pulse 1s infinite cubic-bezier(0.66, 0, 0, 1);
    animation: pulse 1s infinite cubic-bezier(0.66, 0, 0, 1);
  }
  .vuecoon {
    transition: all 1s ease-in-out;
  }
  .vuecoon.landing {
    max-width: 300px;
    margin: 1%;
  }
  .vuecoon.content {
    max-width: 170px;
  }
  .jumbotron {
    transition: all 1s ease-in-out;
    margin-left: 1%;
    margin-right: 1%;
  }
  .jumbotron.landing {
    height: calc( 100vh - 2.5rem );
    transition-delay: 300ms;
    border-bottom-left-radius: 0;
  }
  .jumbotron.content {
    height: 15vh;
  }
  .jumbo-text{
    transition: all 1s ease-in-out;
    overflow: hidden;
  }
  .jumbo-text.content{
    opacity: 0;
    height: 0rem;
  }
  .jumbo-text.landing{
    opacity: 1;
    height: 19rem;
  }
  .jumbo-text-full{
    transition: all 1s ease-in-out;
    overflow: hidden;
  }
  .jumbo-text-full.content{
    width: 80vw;
    justify-content: center;
    display: flex;
    flex-direction: column;
    align-items: center;
    margin-right: 170px;
  }
  .jumbo-text-full.content p{
    text-align: center;
  }
  .jumbo-text-full.landing{
    width: 35vw;
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
  .vue_logo {
    width: 3rem;
    height: 3rem;
  }
  .title {
    margin-left: -5px;
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
    height: 78vh;
    top: 15vh;
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
    position: absolute;
    width: 54%;
    margin: 1%;
    margin-left: 45%;
    transition: all 1s ease-in-out;
    overflow: hidden;
    vertical-align: bottom;
  }
  .browser.content{
    opacity: 1;
    height: 82vh;
    transition-delay: 300ms;
    top: 13vh;
    visibility: visible;
  }
  .browser.landing{
    opacity: 0;
    height: 0vh;
    top: 98vh;
    visibility: hidden;
  }
  .footer{
    position: absolute;
    right: 0;
    margin: 1%;
    transition: all 1s ease-in-out;
    overflow: hidden;
    vertical-align: bottom;
  }
  .footer p {
    margin: auto;
  }
  .footer a {
    color: #42b983;
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
    top: 98vh;
    visibility: hidden;
  }
  .pulse-download-btn:hover {
    -webkit-animation: none;
    -moz-animation: none;
    -ms-animation: none;
    animation: none;
  }

  @media (max-width: 1200px) {
    .jumbo-text.landing{
      height: 23rem;
    }
  }
  @media (max-width: 992px) {
    .vuecoon.landing {
      max-width: 250px;
    }
    .jumbo-text.landing{
      height: 21rem;
    }
    
    .jumbo-text-full.landing{
      width: 50vw;
    }

    .codemirror{
      position: unset;
      width: 92%;
      margin-left: 4%;
      margin-right: 4%;
    }

    .browser{
      width: 98%;
      margin: 1%;
    }

    .browser.content{
      top: calc(15vh + 78vh - 1vh);
      transition-delay: 600ms;
    }
    .footer.content{
      transition-delay: 700ms;
    }

    .footer{
      font-size: 0.8rem;
    }
    .footer.content{
      height: 2rem;
      top: calc(15vh + 78vh - 1vh + 82vh);
      padding-top: 5px;
    }
  }
  @media (max-width: 768px) {

    .jumbo-text.landing{
      height: 29rem;
    }
  }
  @media (max-width: 576px) {
    .vue_logo.landing{
      top: 80%;
    }
    .vue_logo.content{
      top: 19%;
      width: 90px;
      left: calc( 50% - 45px );
    }
    .jumbotron.content {
      height: 26vh;
    }
    .jumbo-text.landing{
      height: 26rem;
    }
    .jumbo-text-full.landing{
      width: 100vw;
    }
    .jumbo-text-full.content{
      margin: unset;
    }
    .vuecoon {
      display: none;
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
</style>
