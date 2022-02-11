<template>
  <div class="col-lg-8 col-md-12 d-flex flex-column justify-content-center align-items-center browser-buttons">
    <span>Generate type:</span>
    <div class="row w-100 justify-content-center">
      <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="generateType === generateTypes.Editor ? 'active' : 'inactive'" @click="$emit('typeChanged', generateTypes.Editor)">
        <span class="bi bi-pencil" aria-hidden="true"></span>
        <span class="ps-2">Editor</span>
      </div>
      <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="generateType === generateTypes.View ? 'active' : 'inactive'" @click="$emit('typeChanged', generateTypes.View)">
        <span class="bi bi-eye" aria-hidden="true"></span>
        <span class="ps-2">View</span>
      </div>
      <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="generateType === generateTypes.Form ? 'active' : 'inactive'" @click="$emit('typeChanged', generateTypes.Form)">
        <span class="bi bi-file-earmark-code" aria-hidden="true"></span>
        <span class="ps-2">Form</span>
      </div>
    </div>
    <span class="mt-4">Layout mode:</span>
    <div class="row w-100 justify-content-center">
      <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="layoutMode === layoutModes.Card ? 'active' : 'inactive'" @click="$emit('layoutChanged', layoutModes.Card)">
        <span class="bi bi-view-stacked" aria-hidden="true"></span>
        <span class="ps-2">Card</span>
      </div>
      <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="layoutMode === layoutModes.Accordion ? 'active' : 'inactive'" @click="$emit('layoutChanged', layoutModes.Accordion)">
        <span class="bi bi-text-indent-left" aria-hidden="true"></span>
        <span class="ps-2">Accordion</span>
      </div>
      <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="layoutMode === layoutModes.Table ? 'active' : 'inactive'" @click="$emit('layoutChanged', layoutModes.Table)">
        <span class="bi bi-table" aria-hidden="true"></span>
        <span class="ps-2">Table</span>
      </div>
    </div>
  </div>
</template>
<script>
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'Options',
  props: {
    generateType: String,
    layoutMode: String
  },
  emits: ['layoutChanged', 'typeChanged'],
  setup() {
    const generateTypes = {
      View: 'view',
      Form: 'form',
      Editor: 'editor',
    }
    const layoutModes = {
      Card: 'card',
      Accordion: 'accordion',
      Table: 'table'
    }

    return { generateTypes, layoutModes }
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
