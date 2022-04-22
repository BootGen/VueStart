<template>
  <div class="container-fluid m-0">
    <landing :vuecoonState="vuecoonState"></landing>
    <generate-options class="mt-3" :frontendMode="frontendMode" :editable="editable" @frontendChanged="changeFrontendMode" @editableChanged="chengeEditable"></generate-options>
    <editor :config="config" :frontendMode="frontendMode" :editable="editable" @hasError="hasError" @setVuecoon="setVuecoon"></editor>
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
      <div class="col-12 d-flex align-items-center footer mt-3">
        <p><a href="javascript:void(0)" @click="showSupporters">Supporters</a> | Powered by <a href="https://bootgen.com" target="_blank">BootGen</a> | Created by <a href="https://codesharp.hu" target="_blank">Code Sharp</a></p>
      </div>
    </div>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
import Landing from './components/Landing.vue';
import Editor from './components/Editor.vue';
import Supporters from './components/Supporters.vue';
import GenerateOptions from './components/GenerateOptions.vue';

export default defineComponent({
  name: 'LandingPage',
  components: { Landing, Editor, Supporters, GenerateOptions },
  setup() {
    const vuecoonStates = {
      Default: 'default',
      Error: 'error',
      Success: 'success'
    };
    const vuecoonState = ref(vuecoonStates.Default);
    const frontendMode = ref('vanilla');
    const editable = ref(false);
    const isShowSupporters = ref(false);

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
    function setVuecoon(state) {
      vuecoonState.value = state;
    }
    function changeFrontendMode(mode) {
      frontendMode.value = mode;
    }
    function chengeEditable(b) {
      editable.value = b;
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

    return { vuecoonState, config,
      hasError, setVuecoon, openGithub,
      changeFrontendMode, frontendMode, editable, chengeEditable, showSupporters, isShowSupporters }
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
</style>
