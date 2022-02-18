<template>
  <div class="container-fluid m-0">
    <download-panel :class="{ 'hide': !showDownloadPanel, 'show' : showDownloadPanel, }" :show="showDownloadPanel" @close="showDownloadPanel = false" @download="download"></download-panel>
    <landing :vuecoonState="vuecoonState"></landing>
    <generate-options class="mt-5" :layoutMode="layoutMode" @layoutChanged="changeLayoutMode"></generate-options>
    <editor :config="config" :layoutMode="layoutMode" @download="onDownloadClicked" @hasError="hasError" @setVuecoon="setVuecoon"></editor>
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
import DownloadPanel from './components/DownloadPanel.vue';
import Landing from './components/Landing.vue';
import Editor from './components/Editor.vue';
import Supporters from './components/Supporters.vue';
import axios from "axios";
import GenerateOptions from './components/GenerateOptions.vue';

export default defineComponent({
  name: 'LandingPage',
  components: { DownloadPanel, Landing, Editor, Supporters, GenerateOptions },
  setup() {
    const showDownloadPanel = ref(false);
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

    let downloadUrl = "";
    let downloadedFileName = "";
    function hasError(value) {
      if(value) {
        vuecoonState.value = vuecoonStates.Error;
      } else {
        if (vuecoonState.value === vuecoonStates.Error)
          vuecoonState.value = vuecoonStates.Default;
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
    }

    function setVuecoon(state) {
      vuecoonState.value = state;
    }
    function changeLayoutMode(mode) {
      layoutMode.value = mode;
    }

    return { showDownloadPanel, vuecoonState,
      config, download, onDownloadClicked,
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
  .download-panel{
    transition: all 1s ease-in-out;
  }
  .download-panel.show{
    opacity: 1;
  }
  .download-panel.hide{
    opacity: 0;
    visibility: hidden;
    margin-top: 100%;
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
