<template>
  <div class="container">
    <div class="row">
      <div class="col text-center">
        <span>Templates:</span>
      </div>
    </div>
    <div class="row flex-colum justify-content-center">
      <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="layoutMode === layoutModes.Card ? 'active' : 'inactive'" @click="$emit('layoutChanged', layoutModes.Card)">
        <span class="bi bi-view-stacked" aria-hidden="true"></span>
        <span class="ps-2">Card</span>
      </div>
      <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="layoutMode === layoutModes.Wizard ? 'active' : 'inactive'" @click="$emit('layoutChanged', layoutModes.Wizard)">
        <span class="bi bi-input-cursor" aria-hidden="true"></span>
        <span class="ps-2">Wizard</span>
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
  name: 'GenerateOptions',
  props: {
    layoutMode: String
  },
  emits: ['layoutChanged',],
  setup() {
    const layoutModes = {
      Table: 'table',
      Card: 'card',
      Wizard: 'wizard'
    }

    return { layoutModes }
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
