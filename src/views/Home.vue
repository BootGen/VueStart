<template>
  <div class="container-fluid h-100"> 
    <div class="row d-flex flex-wrap h-100">
      <div class="col-6 h-100">
        <code-mirror></code-mirror>
      </div>
      <div class="col-6 h-100">
        <browser-frame></browser-frame>
      </div>
    </div>
  </div>
</template>

<script>
import BrowserFrame from '../components/BrowserFrame.vue'
import CodeMirror from '../components/CodeMirror.vue'

export default {
  components: { BrowserFrame, CodeMirror },
  name: 'Home',
}
</script>
