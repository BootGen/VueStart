<template>
  <not-found :class="{'notfound': isNotFound, 'found': !isNotFound}" @showEditor="showEditor"></not-found>
  <div class="container-fluid" :class="{'notfound': isNotFound, 'found': !isNotFound}">
    <landing :vuecoonState="vuecoonState"></landing>
    <editor :config="config" :loadedData="loadedData" @hasError="hasError"></editor>
    <div class="d-flex flex-column align-items-center mt-5">
      <div @click="openGithub()" class="d-flex flex-column align-items-center justify-content-center px-2 github">
        <div class="d-flex align-items-center px-2">
          <span class="bi bi-github px-2 github-icon" aria-hidden="true"></span>
          <span class="bi bi-star-fill star-icon px-2" aria-hidden="true"></span>
        </div>
        <p class="small-text">Star this project on GitHub!</p>
      </div>
      <div class="d-flex flex-column align-items-center">
        <p>Staring our project on  GitHub may seem like a small thing, but it really keeps us motivated. We thank you for your support!</p>
      </div>
    </div>
    <supporters :show="isShowSupporters"></supporters>
    <div class="row">
      <footer class="col-12 d-flex align-items-center footer mt-3">
        <p><a href="javascript:void(0)" @click="showSupporters" aria-label="Supporters">Supporters</a> | Powered by <a href="https://bootgen.com" target="_blank" aria-label="BootGen">BootGen</a> | Created by <a href="https://codesharp.hu" target="_blank" aria-label="Code Sharp">Code Sharp</a> | Send <a href="https://github.com/BootGen/VueStart/discussions/55" target="_blank" aria-label="Feedback">Feedback!</a></p>
      </footer>
    </div>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
import Landing from './components/Landing.vue';
import Editor from './components/Editor.vue';
import Supporters from './components/Supporters.vue';
import NotFound from './components/NotFound.vue';
import axios from "axios";

export default defineComponent({
  name: 'LandingPage',
  components: { Landing, Editor, Supporters, NotFound },
  setup() {
    const vuecoonStates = {
      Default: 'default',
      Error: 'error',
      Success: 'success'
    };
    const vuecoonState = ref(vuecoonStates.Default);
    const isShowSupporters = ref(false);
    const isNotFound = ref(false);
    const loadedData = ref({});

    async function setShowContentForUrl() {   
      let path = window.location.pathname;
      if(path === '/') {
        isNotFound.value = false;   
      } else {
        try {
          let resp = await axios.get(`api/share${path}`);
          if (resp.status == 200) {
            isNotFound.value = false;
            loadedData.value = resp.data.generateRequest;
          }
        } catch (e) {
          isNotFound.value = true;
          loadedData.value = {};
        }
      }
    }
    window.addEventListener('popstate', setShowContentForUrl);
    window.addEventListener('load', setShowContentForUrl);
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

    function hasError(value) {
      if(value) {
        vuecoonState.value = vuecoonStates.Error;
      } else {
        if (vuecoonState.value === vuecoonStates.Error)
          vuecoonState.value = vuecoonStates.Default;
      }
    }

    function showSupporters (){
      isShowSupporters.value = !isShowSupporters.value;
      let pageHeight = document.body.scrollHeight;
      setTimeout(() => {
        window.scrollTo({
          top: pageHeight,
          left: 100,
          behavior: 'smooth'
        })
      }, 10);
    }
    function openGithub (){
      window.open("https://github.com/BootGen/VueStart");
    }
    function showEditor(){
      history.pushState({}, '', '/');
      isNotFound.value = false;
    }

    return { vuecoonState, config,
      hasError, openGithub,
      showSupporters, isShowSupporters, isNotFound, showEditor, loadedData }
  }
});

</script>

<style>
  .container-fluid{
    width: auto!important;
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
  .small-text {
    font-size: 0.8rem;
  }
  .shadow {
    box-shadow: 0 .5rem 1rem rgba(0,0,0,.10)!important;
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
  .github-icon {
    font-size: min(9vw, 5rem);
  }
  .star-icon {
    color: rgb(222, 169, 64);
    font-size: min(6vw, 3.5rem);
  }
  .notfound-page {
    transition: all 1s ease-in-out;
    transition-delay: 300ms;
    position: absolute;
    width: 100%!important;
    height: 100%!important;
    z-index: 999;
  }
  .notfound-page.found {
    top: -100%;
  }
  .notfound-page.notfound {
    top: 0;
    opacity: 1!important;
  }
  .container-fluid {
    transition: all 1s ease-in-out;
    transition-delay: 300ms;
  }
  .container-fluid.found {
    opacity: 1;
    height: 100%;
    overflow: visible;
  }
  .container-fluid.notfound {
    opacity: 0;
    height: 0;
    overflow: hidden;
  }
</style>
