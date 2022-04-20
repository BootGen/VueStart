<template>
  <div class="col-12">
    <modal-panel v-model="showDownloadPanel">
      <download-panel @close="showDownloadPanel = false" @download="download"></download-panel>
    </modal-panel>
    <div class="d-flex justify-content-center align-items-center jumbotron" :class="page">
      <div class="vuecoon" :class="page">
        <img v-if="page === 'supporters'" @click="showEditor()" class="img-fluid clickable" alt="Vuecoon" :src="require(`./assets/vuecoon_${vuecoonState}.webp`)">
        <img v-else class="img-fluid" alt="Vuecoon" :src="require(`./assets/vuecoon_${vuecoonState}.webp`)">
      </div>
      <div class="d-flex align-items-center jumbo-text" :class="page">
        <div class="d-flex flex-column align-items-center">
          <p class="lead text-justify m-0">
            <img src="./assets/vue_logo.webp"> <img src="./assets/bootstrap.webp"> <img src="./assets/tailwind.webp"> <img src="./assets/css.webp">
          </p>
          <p class="lead text-justify m-0">
            Zero dependency data table solution for Vue.js. Generate pure Vue.js data tables. Input some JSON data, choose from Bootstrap, Tailwind CSS or Vanilla CSS, download the code and use it in any project.
          </p>
          <button class="btn fill-btn rounded-pill m-1 btn-lg" @click="showEditor">Start!</button>
        </div>
      </div>
    </div>
    <p class="title">Vue Start!</p>
    <div class="github" :class="page">
      <div class="d-flex clickable flex-column align-items-center" @click="openGithub()">
        <div class="d-flex align-items-center px-2">
          <span class="bi bi-github px-2 github-icon" aria-hidden="true"></span>
          <span class="bi bi-star-fill star-icon px-2" aria-hidden="true"></span>
        </div>
        <p class="small-text">Star this project on GitHub!</p>
      </div>
    </div>
    <transition name="fade">
      <supporters v-if="page === 'supporters'"></supporters>
    </transition>
    <editor :config="config" :page="page" @download="onDownloadClicked" @hasError="hasError" @setVuecoon="setVuecoon"  @success="setSuccessVuecoon"></editor>
    <div class="col-12 d-flex align-items-center footer" :class="page">
      <p><a href="javascript:void(0)" @click="showSupporters">Supporters</a> | Powered by <a href="https://bootgen.com" target="_blank">BootGen</a> | Created by <a href="https://codesharp.hu" target="_blank">Code Sharp</a></p>
    </div>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
import DownloadPanel from './components/DownloadPanel.vue'
import Editor from './components/Editor.vue'
import Supporters from './components/Supporters.vue'
import axios from "axios";
import {debounce} from "@/utils/Helper";
import ModalPanel from "@/components/ModalPanel"

export default defineComponent({
  name: 'LandingPage',
  components: {ModalPanel, DownloadPanel, Editor, Supporters },
  setup() {
    const showDownloadPanel = ref(false);
    const downloaded = ref(false);
    const vuecoonStates = {
      Default: 'default',
      Error: 'error',
      Loading: 'loading',
      Success: 'success'
    };
    const vuecoonState = ref(vuecoonStates.Default);
    let generatedId = '';

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
    function hasError(value) {
      if(value) {
        vuecoonState.value = vuecoonStates.Error;
      } else {
        if (vuecoonState.value !== vuecoonStates.Success)
          vuecoonState.value = vuecoonStates.Default;
      }
    }

    let debounceResetVuecoon = debounce(resetVuecoon, 2000);
    function setSuccessVuecoon(){
      vuecoonState.value = vuecoonStates.Success;
      debounceResetVuecoon();
    }
    function resetVuecoon (){
      vuecoonState.value = vuecoonStates.Default;
    }

    function setShowContentForUrl(){
      if (window.location.pathname === '/supporters')
        page.value = 'supporters';
      else
        page.value = window.location.pathname === '/editor' ? 'content' : 'landing';
    }
    window.addEventListener('popstate', setShowContentForUrl);
    window.addEventListener('load', setShowContentForUrl);
    const page = ref('landing');
    function openGithub (){
      window.open("https://github.com/BootGen/VueStart");
    }
    function showSupporters (){
      window.scrollTo(0, 0);
      page.value = 'supporters';
      history.pushState({}, '', 'supporters');
    }
    function showEditor(){
      window.scrollTo(0, 0);
      page.value = 'content';
      history.pushState({}, '', 'editor');
    }

    async function download() {
      const response = await axios.post(downloadUrl, JSON.parse(localStorage.getItem(generatedId)), {responseType: 'blob', ...config});
      const fileURL = window.URL.createObjectURL(new Blob([response.data]));
      const fileLink = document.createElement('a');
      fileLink.href = fileURL;
      fileLink.target = '_blank';
      fileLink.setAttribute('download', downloadedFileName);
      document.body.appendChild(fileLink);
      fileLink.click();
      showDownloadPanel.value = false;
    }

    function onDownloadClicked(url, fileName, id) {
      downloadUrl = url;
      downloadedFileName = fileName;
      showDownloadPanel.value = true;
      downloaded.value = true;
      generatedId = id;
    }

    function setVuecoon(state) {
      vuecoonState.value = state;
    }

    return { page, showDownloadPanel, openGithub, showEditor, vuecoonState,
      config, download, onDownloadClicked,
      hasError, setSuccessVuecoon, setVuecoon, showSupporters }
  }
});

</script>

<style>
  body {
    height: 99%;
    overflow: hidden;
  }
  p {
    margin: 0;
  }
  .text-justify{
    text-align: justify;
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
    display: flex;
    align-items: center;
  }
  .vuecoon.landing {
    max-width: 250px;
  }
  .vuecoon.content, .vuecoon.supporters {
    max-width: min(12vh, 22vw);
  }
  .jumbotron {
    transition: all 1s ease-in-out;
    transition-delay: 300ms;
    margin-left: 1%;
    margin-right: 1%;
    flex-direction: column;
    width: 48.5%;
  }
  .jumbotron.landing {
    height: 100vh;
  }
  .jumbotron.content, .jumbotron.supporters {
    height: 15vh;
    margin-right: 10%;
  }
  .jumbo-text {
    transition: all 1s ease-in-out;
    overflow: hidden;
    margin-right: 2%;
    margin-left: 1%;
  }
  .jumbo-text.content, .jumbo-text.supporters{
    opacity: 0;
    height: 0rem;
    margin-top: 0;
  }
  .jumbo-text.landing{
    opacity: 1;
    height: 20rem;
    margin: 3rem;
  }
  .jumbo-text img {
    height: 5rem;
    width: 5rem;
    margin: 0.5rem;
  }
  .small-text {
    font-size: 0.8rem;
  }
  .clickable {
    cursor: pointer;
  }
  .github {
    transition: all 1s ease-in-out;
    transition-delay: 300ms;
    position: absolute;
    width: 48.5%;
    top: 0;
    display: flex;
    align-items: center;
    height: 12vh;
    justify-content: center;
    right: 1%;
  }
  .github-icon {
    font-size: min(5vh, 5vw);
  }
  .star-icon {
    color: rgb(222, 169, 64);
    font-size: min(4vh, 4vw);
  }
  @import url('https://fonts.googleapis.com/css2?family=Caveat+Brush&display=swap');
  .title {
    font-family: 'Caveat Brush', cursive;
    font-size: 3rem;
    position: absolute;
    top: 0;
    display: flex;
    justify-content: center;
    align-items: center;
    width: 100%;
    height: 12vh;
    z-index: -1;
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
  .footer.content, .footer.supporters{
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

  @media (max-width: 992px) {
    body {
      height: unset;
      overflow: unset;
    }
    .github {
      height: min(15vh, 25vw);
    }
    .github.landing {
      right: 0;
      margin-top: 0;
      justify-content: flex-end;
    }
    .github.content, .github.supporters {
      justify-content: flex-end;
    }
    .vuecoon.landing {
      max-width: 200px;
      margin: 1%;
    }
    .jumbotron {
      flex-direction: row;
    }
    .jumbotron.landing {
      height: 30vh;
      width: 99%;
      margin-top: 2vh;
    }
    .jumbotron.content, .jumbotron.supporters {
      height: min(15vh, 25vw);
    }
    .footer.content, .footer.supporters{
      transition-delay: 700ms;
    }
    .footer{
      font-size: 0.8rem;
      bottom: unset;
    }
    .footer.content, .footer.supporters{
      height: 2rem;
      top: calc(170vh + 4rem);
      padding-top: 5px;
    }
    .github .small-text {
      display: none!important;
    }
  }
  @media (max-width: 768px) {
    body {
      height: unset;
      overflow: unset;
    }
    .text-justify{
      font-size: 1rem;
    }
    .github-icon {
      font-size: min(7vh, 7vw);
    }
  }
  @media (max-width: 576px) {
    body {
      height: unset;
      overflow: unset;
    }
    .jumbo-text.landing{
      height: 12rem;
    }
    .vuecoon.landing {
      max-width: 35vw;
    }
    .title {
      font-size: 2.2rem;
    }
    .github > .small-text {
      display: none!important;
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

.fade-enter-active, .fade-leave-active {
  transition: opacity .5s;
}
.fade-enter, .fade-leave-to {
  opacity: 0;
}
</style>
