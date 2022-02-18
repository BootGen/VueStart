<template>
  <div class="container">
    <div class="row">
      <div class="col text-center">
        <span>Display:</span>
      </div>
    </div>
    <div class="row flex-colum justify-content-center">
      <div v-if="layoutMode === 'editor'" class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="selected === 0 ? 'active' : 'inactive'" @click="$emit('select', 0)">
        <span class="bi bi-pencil" aria-hidden="true"></span>
        <span class="ps-2">Editor</span>
      </div>
      <div v-else-if="layoutMode === 'card'" class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="selected === 0 ? 'active' : 'inactive'" @click="$emit('select', 0)">
        <span class="bi bi-eye" aria-hidden="true"></span>
        <span class="ps-2">View</span>
      </div>
      <div v-else-if="layoutMode === 'wizard'" class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="selected === 0 ? 'active' : 'inactive'" @click="$emit('select', 0)">
        <span class="bi bi-file-earmark-code" aria-hidden="true"></span>
        <span class="ps-2">Form</span>
      </div>
      <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="selected === 1 ? 'active' : 'inactive'" @click="$emit('select', 1)">
        <span class="bi bi-code" aria-hidden="true"></span>
        <span class="ps-2">index.html</span>
      </div>
      <div class="fab-icon-holder col-lg-3 col-md-3 col-sm-12" :class="selected === 2 ? 'active' : 'inactive'" @click="$emit('select', 2)">
        <span class="bi bi-code" aria-hidden="true"></span>
        <span class="ps-2">app.js</span>
      </div>
    </div>
  </div>
</template>
<script>
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'GenerateOptions',
  props: {
    selected: Number,
    layoutMode: String,
  },
  emits: ['select'],
  setup() {
    return { }
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
