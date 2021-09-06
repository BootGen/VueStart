<template>
  <Editor />
</template>

<script>
import Editor from './views/Editor.vue'

export default {
  name: 'App',
  components: {
    Editor,
  }
}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2c3e50;
  height: 100%!important;
}
</style>
