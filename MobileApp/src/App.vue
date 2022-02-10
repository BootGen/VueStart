<template>
  <div class="col-12 d-flex flex-column align-items-center p-2">
    <tip :modified="modified" :generated="generated" :typeChanged="typeChanged" :downloaded="downloaded" @success="setSuccessVuecoon"></tip>
    <div class="download-panel-container" :class="{ 'hide': !showDownloadPanel, 'show' : showDownloadPanel, }">
      <download-panel class="download-panel shadow" :class="{ 'hide': !showDownloadPanel, 'show' : showDownloadPanel, }" :show="showDownloadPanel" @close="showDownloadPanel = false" @download="download"></download-panel>
    </div>
    <landing :vuecoonState="vuecoonState"></landing>
    <options :generateType="generateType" :layoutMode="layoutMode" @layoutChanged="changeLayoutMode" @typeChanged="chengeGenType"></options>
    <editor :config="config" :generateType="generateType" :layoutMode="layoutMode" @download="onDownloadClicked" @modified="modified = true" @generated="generated = true" @typeChanged="typeChanged = true" @hasError="hasError" @setVuecoon="setVuecoon"></editor>
    <supporters class="mt-4"></supporters>
    <div class="col-12 d-flex align-items-center footer">
      <p>Powered by <a href="https://bootgen.com" target="_blank">BootGen</a> | Created by <a href="https://codesharp.hu" target="_blank">Code Sharp</a></p>
    </div>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
import DownloadPanel from './components/DownloadPanel.vue';
import Landing from './components/Landing.vue';
import Editor from './components/Editor.vue';
import Supporters from './components/Supporters.vue';
import Tip from './components/Tip.vue';
import axios from "axios";
import { debounce } from "@/utils/Helper";
import Options from './components/Options.vue';

export default defineComponent({
  name: 'LandingPage',
  components: { DownloadPanel, Landing, Editor, Tip, Supporters, Options, Options },
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
    const generateType = ref('editor');
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

    let debounceResetVuecoon = debounce(resetVuecoon, 2000);
    function setSuccessVuecoon(){
      vuecoonState.value = vuecoonStates.Success;
      debounceResetVuecoon();
    }
    function resetVuecoon (){
      vuecoonState.value = vuecoonStates.Default;
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

    function setVuecoon(state) {
      vuecoonState.value = state;
    }
    function changeLayoutMode(mode) {
      layoutMode.value = mode;
    }
    function chengeGenType(type) {
      generateType.value = type;
    }

    return { showDownloadPanel, vuecoonState,
      config, download, onDownloadClicked, modified, generated, typeChanged,
      downloaded, hasError, setSuccessVuecoon, setVuecoon,
      changeLayoutMode, chengeGenType, generateType, layoutMode }
  }
});

</script>

<style>
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
