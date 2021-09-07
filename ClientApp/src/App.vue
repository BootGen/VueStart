<template>
  <router-view v-slot="{ Component }">
  <transition name="slide">
    <component :is="Component" />
  </transition>
</router-view>
</template>

<script>

export default {
  name: 'App',
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

.wrapper {
  width: 100%;
  min-height: 100vh;
}
.slide-enter-active,
.slide-leave-active {
  transition: all 0.2s ease-out;
}


.slide-enter-to {
  position: absolute;
  right: 0;
}


.slide-enter-from {
  position: absolute;
  right: -100%;
}


.slide-leave-to {
  position: absolute;
  left: -100%;
}


.slide-leave-from {
  position: absolute;
  left: 0;
}
</style>
