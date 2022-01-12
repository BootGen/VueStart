<template>
  <div class="col-12">
    <tip class="tip-msg" :tipMsg="tipMsg" :class="{ 'show': showContent && tipMsg, 'hide' : !showContent || !tipMsg }"></tip>
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
    <editor :showContent="showContent"></editor>
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

export default defineComponent({
  name: 'LandingPage',
  components: { DownloadPanel, Editor, Tip },
  setup() {
    const showDownloadPanel = ref(false);
    const tipMsg = ref(null);
    const vuecoonStates = {
      Default: 'default',
      Error: 'error',
      Success: 'success'
    };
    const vuecoonState = ref(vuecoonStates.Default);
    
    /*watchEffect(() => {
      if(inputError.value) {
        vuecoonState.value = vuecoonStates.Error;
      } else {
        vuecoonState.value = vuecoonStates.Default;
      }
    });*/
    if (localStorage.getItem('firstUse') === 'false' && localStorage.getItem('regeneratedTip') === 'true' && localStorage.getItem('buttonsTip') !== 'true') {
      setTip('Try out multiple application types and layouts with the buttons in the bottom right corner');
    } else if (localStorage.getItem('firstUse') === 'false' && localStorage.getItem('regeneratedTip') !== 'true') {
      setTip('If you make structural changes to the JSON data, the application is automatically regenerated.');
    } else if (localStorage.getItem('firstUse') !== 'false') {
      setTip('Try to edit the JSON data on the left side, and see the changes in the application on the right side');
    }
    /*function setVuecoon (state, time){
      vuecoonState.value = state;
      let debounceResetVuecoon = debounce(resetVuecoon, time*2);
      debounceResetVuecoon();
    }
    function resetVuecoon (){
      vuecoonState.value = vuecoonStates.Default;
    }*/
    function setTip(msg) {
      tipMsg.value = msg;
    }
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

    return { showContent, showDownloadPanel, openGithub, tipMsg, changeView, vuecoonState }
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
  .tip-msg{
    bottom: 0;
    justify-content: center!important;
    position: fixed;
    z-index: 999;
    transition: all 1s ease-in-out;
  }
  .tip-msg.show{
    opacity: 1;
    visibility: visible;
  }
  .tip-msg.hide{
    opacity: 0;
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
