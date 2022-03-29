<template>
  <div class="container">
    <div class="row flex-colum justify-content-center">
      <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="frontendMode === frontendModes.Vanilla ? 'active' : 'inactive'" @click="$emit('frontendChanged', frontendModes.Vanilla)">
        <span class="bi bi-filetype-css" aria-hidden="true"></span>
        <span class="ps-2">Vanilla</span>
      </div>
      <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="frontendMode === frontendModes.Bootstrap ? 'active' : 'inactive'" @click="$emit('frontendChanged', frontendModes.Bootstrap)">
        <span class="bi bi-bootstrap" aria-hidden="true"></span>
        <span class="ps-2">Bootstrap</span>
      </div>
     <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="frontendMode === frontendModes.Tailwind ? 'active' : 'inactive'" @click="$emit('frontendChanged', frontendModes.Tailwind)">
        <span class="bi bi-wind" aria-hidden="true"></span>
        <span class="ps-2">Tailwind</span>
      </div>
    </div>
    <div class="row flex-colum justify-content-center">
      <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="editable ? 'active' : 'inactive'" @click="$emit('editableChanged', true)">
        <span class="bi bi-pencil" aria-hidden="true"></span>
        <span class="ps-2">Editable</span>
      </div>
      <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="!editable ? 'active' : 'inactive'" @click="$emit('editableChanged', false)">
        <span class="bi bi-shield" aria-hidden="true"></span>
        <span class="ps-2">Uneditable</span>
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
.fab-icon-holder:hover {
  background: #42b983;
  color: #ffffff;
}
.active {
  background: #42b983;
  color: #ffffff;
}
.inactive {
  background: #ffffff;
  color: #42b983;
}
</style>
