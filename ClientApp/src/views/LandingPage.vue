<template>
  <div class="col-12">
    <div class="d-flex justify-content-center align-items-center jumbotron" :class="{ 'landing': !showNav, 'content' : showNav }">
      <img class="vuecoon img-fluid" alt="Vuecoon" :src="require(`../assets/vuecoon_${$store.state.vuecoonType}.webp`)" :class="{ 'landing': !showNav, 'content' : showNav, }">
      <div class="jumbo-text-full" :class="{ 'landing': !showNav, 'content' : showNav }">
        <div class="d-flex align-items-center justify-content-center ">
          <img class="vue_logo" alt="vue" :src="require(`../assets/vue_logo.webp`)">
          <h1 class="title">ue Start!</h1>
        </div>
        <div class="d-flex align-items-center jumbo-text" :class="{ 'landing': !showNav, 'content' : showNav }">
          <div class="d-flex flex-column align-items-center">
            <p class="lead">
              Lorem Ipsum is simply dummy text of the printing and typesetting industry. Deploy test. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.
            </p>
            <button class="btn fill-btn rounded-pill m-1 btn-lg" @click="showNav = !showNav">Start!</button>
          </div>
        </div>
        <div class="slogen-text" :class="{ 'landing': !showNav, 'content' : showNav }">
          <p class="lead">
            Lorem Ipsum is simply dummy text of the printing and typesetting industry.
          </p>
        </div>
      </div>
    </div>

    <div class="codemirror custom-card" :class="{ 'landing': !showNav, 'content' : showNav, }">
      <code-mirror v-model="json"></code-mirror>
    </div>
    <div class="browser-container" :class="{ 'landing': !showNav, 'content' : showNav, }">
      <div class="browser custom-card shadow">
        <browser-frame v-model="appUrl" :borderRadius="generateType == generateTypes.Editor">
          <div class="d-flex w-100 h-auto">
            <tab title="Editor" icon="pencil" :showVr="generateType != generateTypes.Editor && generateType != generateTypes.View" @select="changeGeneratedMode(generateTypes.Editor)" :class="{ 'inactive': generateType != generateTypes.Editor, 'active' : generateType == generateTypes.Editor, 'border-bottom-right' : generateType == generateTypes.View }"></tab>
            <tab title="View" icon="eye" :showVr="generateType != generateTypes.View && generateType != generateTypes.Form" @select="changeGeneratedMode(generateTypes.View)" :class="{ 'inactive': generateType != generateTypes.View, 'active' : generateType == generateTypes.View, 'border-bottom-right' : generateType == generateTypes.Form, 'border-bottom-left' : generateType == generateTypes.Editor }"></tab>
            <tab title="Form" icon="file-earmark-code" :showVr="generateType != generateTypes.Form" @select="changeGeneratedMode(generateTypes.Form)" :class="{ 'inactive': generateType != generateTypes.Form, 'active' : generateType == generateTypes.Form, 'border-bottom-left' : generateType == generateTypes.View }"></tab>
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
          </div>
          <ul class="fab-options">
            <li>
              <div class="fab-icon-holder" @click="changeGeneratedMode(generateTypes.Editor)">
                <span class="bi bi-pencil" aria-hidden="true"></span>
              </div>
            </li>
            <li>
              <div class="fab-icon-holder" @click="changeGeneratedMode(generateTypes.View)">
                <span class="bi bi-eye" aria-hidden="true"></span>
              </div>
            </li>
            <li>
              <div class="fab-icon-holder" @click="changeGeneratedMode(generateTypes.Form)">
                <span class="bi bi-file-earmark-code" aria-hidden="true"></span>
              </div>
            </li>
          </ul>
        </div>
        <div class="fab-container mx-2">
          <div class="fab fab-icon-holder">
            <span class="bi bi-view-stacked" aria-hidden="true" v-if="layoutMode == layoutModes.Card"></span>
            <span class="bi bi-text-indent-left" aria-hidden="true" v-if="layoutMode == layoutModes.Accordion"></span>
            <span class="bi bi-table" aria-hidden="true" v-if="layoutMode == layoutModes.Table"></span>
          </div>
          <ul class="fab-options">
            <li>
              <div class="fab-icon-holder" @click="changeLayoutMode(layoutModes.Card)">
                <span class="bi bi-view-stacked" aria-hidden="true"></span>
              </div>
            </li>
            <li>
              <div class="fab-icon-holder" @click="changeLayoutMode(layoutModes.Accordion)">
                <span class="bi bi-text-indent-left" aria-hidden="true"></span>
              </div>
            </li>
            <li>
              <div class="fab-icon-holder" @click="changeLayoutMode(layoutModes.Table)">
                <span class="bi bi-table" aria-hidden="true"></span>
              </div>
            </li>
          </ul>
        </div>
        <div id="download-btn" class="fab fab-icon-holder pulse-download-btn" @click="download">
          <span class="bi bi-download" aria-hidden="true"></span>
        </div>
      </div>
    </div>
    <div class="col-12 d-flex align-items-center footer" :class="{ 'landing': !showNav, 'content' : showNav, }">
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
    
    window.onload = function () {
        window.history.pushState(null, "", window.location.href);
        window.onpopstate = function() {
          if (showNav.value) {
            showNav.value = false;
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
    const showNav = ref(false);
    const appUrl = ref("");
    async function generate() {
      const resp = await axios.post(`api/generate/${generateType.value}/${layoutMode.value}`, JSON.parse(json.value), config);
      saveToLocalStorage(json.value);
      appUrl.value = `api/files/${resp.data.id}/index.html`;
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

    return { showNav, json, appUrl, download, generateType, generateTypes, changeGeneratedMode, layoutMode, layoutModes, changeLayoutMode }
  }
});

</script>

<style>
  .fab-container {
    bottom: 50px;
    right: 50px;
    z-index: 999;
    cursor: pointer;
  }

  .fab-icon-holder {
    width: 50px;
    height: 50px;
    border-radius: 100%;
    background: #42b983;
  }

  .fab-icon-holder:hover {
    background: #17a062;
  }

  .fab-icon-holder span {
    display: flex;
    align-items: center;
    justify-content: center;
    height: 100%;
    font-size: 25px;
    color: #ffffff;
  }

  .fab {
    width: 60px;
    height: 60px;
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
    bottom: 0.5rem;
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
    max-width: 15vh;
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
    max-width: 40%;
    justify-content: center;
    display: flex;
    flex-direction: column;
    align-items: center;
    margin-right: 15vh;
  }
  .jumbo-text-full.landing{
    width: 40%;
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
    height: 82vh;
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
    overflow: hidden;
    vertical-align: bottom;
    background-color: transparent;
    box-shadow: 0rem -1.5rem 2rem rgb(0 0 0 / 10%);
  }
  .browser-container.content{
    opacity: 1;
    height: calc(82vh + 2rem);
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
    padding-top: 1rem;
    transition: all 1s ease-in-out;
    overflow: hidden;
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
    top: calc(15vh + 80vh);
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
    .jumbo-text.landing{
      height: 21rem;
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
      top: calc(15vh + 78vh - 1vh + 82vh + 1.5rem);
      padding-top: 5px;
    }
  }
  @media (max-width: 768px) {

    .jumbo-text.landing{
      height: 29rem;
    }
  }
  @media (max-width: 576px) {
    .jumbotron.content {
      height: 20vh;
    }
    .vue_logo.landing{
      top: 80%;
    }
    .vue_logo.content{
      top: 19%;
      width: 90px;
      left: calc( 50% - 45px );
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
