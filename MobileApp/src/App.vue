<template>
  <div class="container-fluid m-0">
    <landing :vuecoonState="vuecoonState"></landing>
    <generate-options class="mt-3" :layoutMode="layoutMode" @layoutChanged="changeLayoutMode"></generate-options>
    <editor :config="config" :layoutMode="layoutMode" @hasError="hasError" @setVuecoon="setVuecoon"></editor>
    <supporters class="mt-5"></supporters>
    <div class="row">
      <div class="col-12 d-flex align-items-center footer mt-3">
        <p>Powered by <a href="https://bootgen.com" target="_blank">BootGen</a> | Created by <a href="https://codesharp.hu" target="_blank">Code Sharp</a></p>
      </div>
    </div>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
import Landing from './components/Landing.vue';
import Editor from './components/Editor.vue';
import Supporters from './components/Supporters.vue';
import axios from "axios";
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
    const layoutMode = ref('card');

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
    function changeLayoutMode(mode) {
      layoutMode.value = mode;
    }

    return { vuecoonState, config,
      hasError, setVuecoon,
      changeLayoutMode, layoutMode }
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
</style>
