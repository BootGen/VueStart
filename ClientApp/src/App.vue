<template>
  <div class="col-12">
    <tip :modified="modified" :generated="generated" :typeChanged="typeChanged" :downloaded="downloaded" v-if="showContent" ></tip>
    <div class="download-panel-container" :class="{ 'hide': !showDownloadPanel, 'show' : showDownloadPanel, }">
      <download-panel class="download-panel shadow" :class="{ 'hide': !showDownloadPanel, 'show' : showDownloadPanel, }" :show="showDownloadPanel" @close="showDownloadPanel = false" @download="download"></download-panel>
    </div>
    <div class="d-flex justify-content-center align-items-center jumbotron" :class="{ 'landing': !showContent, 'content' : showContent }">
      <img class="vuecoon img-fluid" alt="Vuecoon" :src="require(`./assets/vuecoon_${vuecoonState}.webp`)" :class="{ 'landing': !showContent, 'content' : showContent, }">
      <div class="jumbo-text-full" :class="{ 'landing': !showContent, 'content' : showContent }">
        <div class="d-flex align-items-center justify-content-center">
          <img class="vue_logo" alt="vue" :src="require(`./assets/vue_logo.webp`)">
          <p class="title">ue Start!</p>
        </div>
        <div class="d-flex align-items-center jumbo-text" :class="{ 'landing': !showContent, 'content' : showContent }">
          <div class="d-flex flex-column align-items-center">
            <p class="lead text-justify">
              Speed up frontend development, with this open source Vue.js component generator.
              Generate forms, tables and data editors for any JSON data.
            </p>
            <button class="btn fill-btn rounded-pill m-1 btn-lg" @click="changeView">Start!</button>
          </div>
        </div>
        <div class="d-flex slogen-text" :class="{ 'landing': !showContent, 'content' : showContent }">
          <p class="text-center">
          Generate forms, tables and data editors!
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
    <editor :config="config" :showContent="showContent" @download="onDownloadClicked" @modified="modified = true" @generated="generated = true" @typeChanged="typeChanged = true" ></editor>
    <div class="col-12 d-flex align-items-center footer" :class="{ 'landing': !showContent, 'content' : showContent, }">
      <p>Powered by <a href="https://bootgen.com" target="_blank">BootGen</a> | Created by <a href="https://codesharp.hu" target="_blank">Code Sharp Kft.</a></p>
    </div>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
//import { debounce } from './utils/Helper';
import DownloadPanel from './components/DownloadPanel.vue'
import Editor from './components/Editor.vue'
import Tip from './components/Tip.vue';
import axios from "axios";

export default defineComponent({
  name: 'LandingPage',
  components: { DownloadPanel, Editor, Tip },
  setup() {
    const showDownloadPanel = ref(false);
    const modified = ref(false);
    const generated = ref(false);
    const typeChanged = ref(false);
    const downloaded = ref(false);
    const vuecoonStates = {
      Default: 'default',
      Error: 'error',
      Success: 'success'
    };
    const vuecoonState = ref(vuecoonStates.Default);

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

    let downloadUrl = "";
    let downloadedFileName = "";

    /*watchEffect(() => {
      if(inputError.value) {
        vuecoonState.value = vuecoonStates.Error;
      } else {
        vuecoonState.value = vuecoonStates.Default;
      }
    });*/

    /*function setVuecoon (state, time){
      vuecoonState.value = state;
      let debounceResetVuecoon = debounce(resetVuecoon, time*2);
      debounceResetVuecoon();
    }
    function resetVuecoon (){
      vuecoonState.value = vuecoonStates.Default;
    }*/

    /*function setNextTip(msg, time){
      setVuecoon(vuecoonStates.Success, time);
      setTip(null);
      if(msg) {
        let debouncedTip = debounce(setTip, time);
        debouncedTip(msg);
      }
    }*/

    function setShowContentForUrl(){
      showContent.value = window.location.pathname === '/editor';
    }
    window.addEventListener('popstate', setShowContentForUrl);
    window.addEventListener('load', setShowContentForUrl);
    const showContent = ref(false);
    function openGithub (){
      window.open("https://github.com/BootGen/VueStart");
    }
    function changeView(){
      showContent.value = !showContent.value;
      if(showContent.value) {
        history.pushState({}, '', 'editor');
      } else {
        history.back();
      }
    }

    async function download() {
      const response = await axios.post(downloadUrl, JSON.parse(localStorage.getItem('json')), {responseType: 'blob', ...config});
      const fileURL = window.URL.createObjectURL(new Blob([response.data]));
      const fileLink = document.createElement('a');
      fileLink.href = fileURL;
      fileLink.target = '_blank';
      fileLink.setAttribute('download', downloadedFileName);
      document.body.appendChild(fileLink);
      fileLink.click();
      showDownloadPanel.value = false;
    }

    function onDownloadClicked(url, fileName) {
      downloadUrl = url;
      downloadedFileName = fileName;
      showDownloadPanel.value = true;
      downloaded.value = true;
    }

    return { showContent, showDownloadPanel, openGithub, changeView, vuecoonState,
      config, download, onDownloadClicked, modified, generated, typeChanged, downloaded }
  }
});

</script>

<style>
body {
  height: 100%;
  overflow: hidden;
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

.shadow {
    box-shadow: 0 .5rem 1rem rgba(0,0,0,.10)!important;
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

    .footer.content{
      top: calc(170vh + 1.5rem);
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
