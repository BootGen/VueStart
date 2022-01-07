<template>
  <div class="col-12">
    <div class="download-panel-container" :class="{ 'hide': !showDownloadPanel, 'show' : showDownloadPanel, }">
      <download-panel class="download-panel shadow" :class="{ 'hide': !showDownloadPanel, 'show' : showDownloadPanel, }" :show="showDownloadPanel" @close="showDownloadPanel = false" @download="download"></download-panel>
    </div>
    <div class="d-flex justify-content-center align-items-center jumbotron" :class="{ 'landing': !showContent, 'content' : showContent }">
      <img class="vuecoon img-fluid" alt="Vuecoon" :src="require(`./assets/vuecoon_${!inputError ? 'default' : 'error'}.webp`)" :class="{ 'landing': !showContent, 'content' : showContent, }">
      <div class="jumbo-text-full" :class="{ 'landing': !showContent, 'content' : showContent }">
        <div class="d-flex align-items-center justify-content-center">
          <img class="vue_logo" alt="vue" :src="require(`./assets/vue_logo.webp`)">
          <p class="title">ue Start!</p>
        </div>
        <div class="d-flex align-items-center jumbo-text" :class="{ 'landing': !showContent, 'content' : showContent }">
          <div class="d-flex flex-column align-items-center">
            <p class="lead text-justify">
              Welcome to <span class="fg-primary">VueStart</span>!<br />
              we are glad you finally found your place.<br />
              All you have to do is give us the structure described in json and you can already download the finished application in the form of your choice.<br />
              When you're ready, just click the <span class="fg-primary">Start</span> button.
            </p>
            <button class="btn fill-btn rounded-pill m-1 btn-lg" @click="showContent = !showContent">Start!</button>
          </div>
        </div>
        <div class="d-flex slogen-text" :class="{ 'landing': !showContent, 'content' : showContent }">
          <p class="text-center">
          Generate forms, data editors and viewers!
          </p>
        </div>
      </div>
      <div @click="openGithub()" class="d-flex flex-column align-items-center justify-content-center px-2 github" :class="{ 'landing': !showContent, 'content' : showContent }">
        <div class="d-flex align-items-center px-2">
          <span class="bi bi-github px-2 github-icon" aria-hidden="true"></span>
          <span class="bi bi-star-fill star-icon px-2" aria-hidden="true"></span>
        </div>
        <p class="small-text">Star this project on GitHub!</p>
      </div>
    </div>  

    <div class="codemirror custom-card" :class="{ 'landing': !showContent, 'content' : showContent, }">
      <code-mirror v-model:modelValue="json" v-model:error="inputError"></code-mirror>
    </div>
    <div class="browser-container" :class="{ 'landing': !showContent, 'content' : showContent, }">
      <div class="browser custom-card shadow">
        <browser-frame v-model="appUrl" :borderRadius="generateType == generateTypes.Editor">
          <div class="d-flex w-100 h-auto">
            <tab :title="generateTypes.Editor" icon="pencil" :showVr="generateType != generateTypes.Editor && generateType != generateTypes.View" @select="changeGeneratedMode(generateTypes.Editor)" :class="{ 'inactive': generateType != generateTypes.Editor, 'active' : generateType == generateTypes.Editor, 'border-bottom-right' : generateType == generateTypes.View }"></tab>
            <tab :title="generateTypes.View" icon="eye" :showVr="generateType != generateTypes.View && generateType != generateTypes.Form" @select="changeGeneratedMode(generateTypes.View)" :class="{ 'inactive': generateType != generateTypes.View, 'active' : generateType == generateTypes.View, 'border-bottom-right' : generateType == generateTypes.Form, 'border-bottom-left' : generateType == generateTypes.Editor }"></tab>
            <tab :title="generateTypes.Form" icon="file-earmark-code" :showVr="generateType != generateTypes.Form" @select="changeGeneratedMode(generateTypes.Form)" :class="{ 'inactive': generateType != generateTypes.Form, 'active' : generateType == generateTypes.Form, 'border-bottom-left' : generateType == generateTypes.View }"></tab>
            <button type="button" class="btn-site inactive" :class="{ 'border-bottom-left' : generateType == generateTypes.Form }"><span class="bi bi-plus" aria-hidden="true"></span></button>
          </div>
        </browser-frame>
      </div>
      <div class="d-flex browser-buttons">
        <div class="fab-container">
          <div class="fab fab-icon-holder">
            <span class="bi bi-pencil" aria-hidden="true" v-if="generateType == generateTypes.Editor"></span>
            <span class="bi bi-eye" aria-hidden="true" v-if="generateType == generateTypes.View"></span>
            <span class="bi bi-file-earmark-code" aria-hidden="true" v-if="generateType == generateTypes.Form"></span>
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
            <span class="bi bi-view-stacked" aria-hidden="true" v-if="layoutMode == layoutModes.Card"></span>
            <span class="bi bi-text-indent-left" aria-hidden="true" v-if="layoutMode == layoutModes.Accordion"></span>
            <span class="bi bi-table" aria-hidden="true" v-if="layoutMode == layoutModes.Table"></span>
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
    <div class="col-12 d-flex align-items-center footer" :class="{ 'landing': !showContent, 'content' : showContent, }">
      <p>Powered by <a href="https://bootgen.com" target="_blank">BootGen</a> | Created by <a href="https://codesharp.hu" target="_blank">Code Sharp Kft.</a></p>
    </div>
    <tip :tipMsg="tipMsg" v-if="tipMsg != '' && showContent"></tip>
  </div>
</template>

<script>
import axios from 'axios';
import { defineComponent, ref, watchEffect } from 'vue';
import { getSchema } from './utils/Schema';
import { debounce } from './utils/Helper';
import CodeMirror from './components/CodeMirror.vue';
import BrowserFrame from './components/BrowserFrame.vue'
import DownloadPanel from './components/DownloadPanel.vue'
import Tab from './components/Tab.vue'
import Tip from './components/Tip.vue';

export default defineComponent({
  name: 'LandingPage',
  components: { CodeMirror, BrowserFrame, DownloadPanel, Tab, Tip },
  setup() {
    const json = ref('');
    const jsonSchema = ref('');
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
    const showDownloadPanel = ref(false);
    const inputError = ref(null);
    const tipMsg = ref('');
    
    if(localStorage.getItem('showTips') != 'false') {
      localStorage.setItem('showTips', true);
    }
    if (!localStorage.getItem('firstUse') && localStorage.getItem('showTips') == 'true') {
      tipMsg.value = 'Try to edit the JSON data on the left side, and see the changes in the application on the right side';
    }

    function saveToLocalStorage(newValue) {
      try {
        let obj = JSON.parse(newValue);
        let minimized = JSON.stringify(obj);
        let oldValue = localStorage.getItem('json');
        if (minimized != oldValue) {
            localStorage.setItem('json', minimized);
            if(showContent.value && localStorage.getItem('showTips') == 'true') {
              let debouncedTip = debounce(setTip, 1000);
              let largeDebouncedTip = debounce(setTip, 8000);
              if(!localStorage.getItem('regeneratedTip')) {
                localStorage.setItem('firstUse', true)
                debouncedTip('If you make structural changes to the JSON data, the application is automatically regenerated.');
              }
              if(!localStorage.getItem('buttonsTip')) {
                largeDebouncedTip('Try out multiple application types and layouts with the buttons in the bottom right corner');
                localStorage.setItem('regeneratedTip', true);
                localStorage.setItem('buttonsTip', true);
              }
            }
        }
        document.getElementById('download-btn').classList.add('pulse-download-btn');
        setTimeout(function(){ 
          document.getElementById('download-btn').classList.remove('pulse-download-btn');
        }, 2000);
      } catch (e) {
        console.log(e)
      }
    }
    async function getProjectContentFromServer(name) {
      const data = (await axios.get(`/${name}.json`, {responseType: 'text', ...config})).data;
      if (typeof data === 'string')
        return data;
      return JSON.stringify(data);
    }
    function setTip(msg) {
      tipMsg.value = msg;
    }
    getProjectContentFromServer('example_input').then( (content) => {
      json.value = content;
      jsonSchema.value = ref(getSchema(JSON.parse(json.value)));
      let debouncedGenerate = debounce(generate, 1000);
      watchEffect(() => {
        try {
          const newSchema = getSchema(JSON.parse(json.value));
          if(JSON.stringify(newSchema) != JSON.stringify(jsonSchema.value)) {
            jsonSchema.value = newSchema;
            debouncedGenerate();
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
    window.onload = function () {
        window.history.pushState(null, "", window.location.href);
        window.onpopstate = function() {
          if (showContent.value) {
            showContent.value = false;
            window.history.pushState(null, "", window.location.href);
          } else {
            history.back();
          }
        };
    }
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
    const showContent = ref(false);
    const appUrl = ref("");

    async function generate() {
      if(typeof JSON.parse(json.value) == 'object') {
        try {
          const resp = await axios.post(`api/generate/${generateType.value}/${layoutMode.value}`, JSON.parse(json.value), config);
          saveToLocalStorage(json.value);
          appUrl.value = `api/files/${resp.data.id}/index.html`;
          inputError.value = null;
        } catch (e) {
          inputError.value = e.response.data.error;
        }
      } else {
        inputError.value = 'The root element must be an object!';
      }
    }
    function changeGeneratedMode(type) {
      generateType.value = type
      generate()
    }
    function changeLayoutMode(type) {
      layoutMode.value = type
      generate()
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
    function openGithub (){
      window.open("https://github.com/BootGen/VueStart");
    }

    return { showContent, json, appUrl, download, generateType, generateTypes, changeGeneratedMode, layoutMode, layoutModes, changeLayoutMode, showDownloadPanel, inputError, openGithub, tipMsg }
  }
});

</script>

<style>
body {
  height: 100%;
  overflow: hidden;
}
.fg-primary {
  color: #42b983;
}
.text-justify{
  text-align: justify;
}
.download-panel{
  transition: all 1s ease-in-out;
  width: max-content;
  margin: 1rem auto;
}
.download-panel.show{
  opacity: 1;
}
.download-panel.hide{
  opacity: 0;
  visibility: hidden;
  margin-top: 100%;
}
.download-panel-container {
  transition: all 1s ease-in-out;
  position: fixed;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.1);
  z-index: 999;
  display: flex;
  justify-content: center;
  align-items: center;
}
.download-panel-container.show{
  opacity: 1;
}
.download-panel-container.hide{
  opacity: 0;
  visibility: hidden;
}

.dot {
  margin: 4px;
  height: 12px;
  width: 12px;
  background-color: #bbb;
  border-radius: 50%;
  display: inline-block;
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

  .fill-btn {
    color: #ffffff;
    background-color: #42b983;
  }
  .fill-btn:hover {
    color: #ffffff;
    border-color: #17a062;
    background-color: #17a062;
  }
  .outline-btn {
    color: #42b983;
    background-color: transparent;
    border: solid 1px #42b983;
    padding: 0.25rem 1rem;
  }
  .outline-btn:hover {
    color: #42b983;
    background-color: rgba(200, 200, 200, 0.3);
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
  .vuecoon {
    transition: all 1s ease-in-out;
  }
  .vuecoon.landing {
    max-width: 300px;
    margin: 1%;
  }
  .vuecoon.content {
    max-width: min(15vh, 25vw);
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
    max-width: 50%;
    justify-content: center;
    display: flex;
    flex-direction: column;
    align-items: center;
  }
  .jumbo-text-full.landing{
    width: 50%;
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
  .small-text {
    font-size: 0.8rem;
  }
  .github {
    transition: all 1s ease-in-out;
    overflow: hidden;
    cursor: pointer;
  }
  .github.content{
    opacity: 1;
    height: 100%;
    width: auto;
    visibility: visible;
  }
  .github.landing{
    opacity: 0;
    height: 0;
    width: 0;
    visibility: hidden;
  }
  .github-icon {
    font-size: min(5vh, 5vw);
  }
  .star-icon {
    color: rgb(222, 169, 64);
    font-size: min(4vh, 4vw);
  }
  .title {
    margin-left: -3px;
    font-size: 1.9rem;
    margin-top: 0;
    margin-bottom: .5rem;
    font-weight: 500;
    line-height: 1.2;
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
  .footer{
    position: absolute;
    transition: all 1s ease-in-out;
    overflow: hidden;
    bottom: 0;
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
  .footer.content{
    opacity: 1;
    height: 2.5rem;
    transition-delay: 500ms;
    visibility: visible;
  }
  .footer.landing{
    opacity: 0;
    height: 0vh;
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
    .footer.content{
      transition-delay: 700ms;
    }

    .footer{
      font-size: 0.8rem;
      bottom: unset;
    }
    .footer.content{
      height: 2rem;
      top: calc(170vh + 1.5rem);
      padding-top: 5px;
    }
  }
  @media (max-width: 768px) {
    body {
      height: unset;
      overflow: unset;
    }

    .jumbo-text.landing{
      height: 29rem;
    }
    .github .small-text {
      display: none!important;
    }
  }
  @media (max-width: 576px) {
    body {
      height: unset;
      overflow: unset;
    }
    .jumbotron.content {
      height: min(15vh, 25vw);
    }
    .jumbo-text.landing{
      height: 26rem;
    }
    .jumbo-text-full.landing{
      width: 100vw;
      text-align: center;
    }
    .jumbo-text-full.content{
      margin: unset;
      max-width: 100%;
      text-align: center;
    }
    .vuecoon.landing {
      max-width: 35vw;
    }
    .text-justify{
      font-size: 1rem;
    }
    .jumbo-text-full.content {
      max-width: 50%;
    }
    .github-icon {
        font-size: min(10vh, 10vw);
    }
    .vue_logo {
      width: 2.5rem;
      height: 2.5rem;
    }
    .title {
      font-size: 1.5rem;
      margin-right: min(2.5vh, 7.5vw);
    }
    .star-icon {
      display: none!important;
    }
    .github > .small-text {
      display: none!important;
    }
    .slogen-text {
      display: none!important;
    }
    .browser-container.content{
      top: calc(min(15vh, 25vw) + 76vh - 1.5vh);
    }
    .footer.content{
      top: calc(170vh + 1.5rem);
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

  #app {
    font-family: Avenir, Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    color: #333;
    height: 100%!important;
  }

  .custom-card {
    border-radius: 5px;
  }
</style>
