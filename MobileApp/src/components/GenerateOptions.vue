<template>
  <div class="container">
    <div class="row flex-colum justify-content-center">
      <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="frontendMode === frontendModes.Vanilla ? 'active' : 'inactive'" @click="$emit('frontendChanged', frontendModes.Vanilla)">
        <img v-if="frontendMode === frontendModes.Vanilla" alt="css" src="../assets/css_white.webp" height="25">
        <img v-else alt="css" src="../assets/css_green.webp" height="25">
        <span class="ps-2">Vanilla</span>
      </div>
      <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="frontendMode === frontendModes.Bootstrap ? 'active' : 'inactive'" @click="$emit('frontendChanged', frontendModes.Bootstrap)">
        <img v-if="frontendMode === frontendModes.Bootstrap" alt="bootstrap" src="../assets/bootstrap_white.webp" height="25">
        <img v-else alt="bootstrap" src="../assets/bootstrap_green.webp" height="25">
        <span class="ps-2">Bootstrap</span>
      </div>
     <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="frontendMode === frontendModes.Tailwind ? 'active' : 'inactive'" @click="$emit('frontendChanged', frontendModes.Tailwind)">
        <img v-if="frontendMode === frontendModes.Tailwind" alt="tailwind" src="../assets/tailwind_white.webp" height="25">
        <img v-else alt="tailwind" src="../assets/tailwind_green.webp" height="25">
        <span class="ps-2">Tailwind</span>
      </div>
    </div>
    <div class="row flex-colum justify-content-center">
      <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="editable ? 'active' : 'inactive'" @click="$emit('editableChanged', true)">
        <span class="bi bi-pencil" aria-hidden="true"></span>
        <span class="ps-2">Editable</span>
      </div>
      <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="!editable ? 'active' : 'inactive'" @click="$emit('editableChanged', false)">
        <span class="bi bi-eye" aria-hidden="true"></span>
        <span class="ps-2">Read-only</span>
      </div>
    </div>
  </div>
</template>
<script>
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'GenerateOptions',
  props: {
    frontendMode: String,
    editable: Boolean
  },
  emits: ['frontendChanged', 'editableChanged'],
  setup() {
    const frontendModes = {
      Vanilla: 'vanilla',
      Bootstrap: 'bootstrap',
      Tailwind: 'tailwind'
    }

    return { frontendModes }
  },
})
</script>

<style scoped>
.fab-icon-holder {
  cursor: pointer;
  height: 40px;
  border-radius: 25px;
  background-color: transparent;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #42b983;
  padding: 1rem;
  box-shadow: 0 .5rem 1rem rgba(0,0,0,.10)!important;
  margin: 0.25rem;
}
.fab-icon-holder .bi{
  font-size: 1.2rem;
}
.active {
  background: #42b983;
  color: #ffffff;
}
.inactive {
  background: #ffffff;
  color: #42b983;
}
.fab-icon-holder.inactive:hover {
  background: #42b98320;
}
.fab-icon-holder.active:hover {
  background: #17a062;
}
</style>
