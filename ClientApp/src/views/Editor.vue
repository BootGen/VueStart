<template>
  <div class="container-fluid h-100"> 
    <div class="row d-flex flex-wrap h-100">
      <div class="col-6 h-100">
        <code-mirror></code-mirror>
      </div>
      <div class="col-6 h-100">
      <browser-frame></browser-frame>
      </div>
      <!-- type can be: default, loading, error -->
      <vueuen class="vueuen" type="default"></vueuen>
    </div>
  </div>
</template>

<script>
import BrowserFrame from '../components/BrowserFrame.vue'
import CodeMirror from '../components/CodeMirror.vue'
import Vueuen from '../components/Vueuen.vue'

export default {
  components: { BrowserFrame, CodeMirror, Vueuen },
  name: 'Editor',
}
</script>


<style>
  .vueuen img{
    position: absolute;
    bottom: 0;
    right: 0;
  }
</style>
