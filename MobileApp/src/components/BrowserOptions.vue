<template>
  <div class="container">
    <div class="row">
      <div class="col text-center">
        <span>Display:</span>
      </div>
    </div>
    <div class="row flex-colum justify-content-center">
      <div class="btn-group col-lg-5 col-md-5 col-12 p-0" role="group" aria-label="Basic example">
        <button type="button" class="btn btn-secondary toggle_left p-2" :class="selected === 0 ? 'active' : 'inactive'" @click="$emit('select', 0)">
          <span :class="`bi bi-${layoutModeIcon}`" aria-hidden="true"></span>
          <span class="ps-2">View</span>
        </button>
        <button type="button" class="btn btn-secondary toggle_right p-2" :class="selected === 1 ? 'active' : 'inactive'" @click="$emit('select', 1)">
          <span class="bi bi-code" aria-hidden="true"></span>
          <span class="ps-2">app.js</span>
        </button>
      </div>
    </div>
  </div>
</template>
<script>
import { defineComponent, computed } from 'vue';

export default defineComponent({
  name: 'GenerateOptions',
  props: {
    selected: Number,
    layoutMode: String,
  },
  emits: ['select'],
  setup(props) {

    const layoutModeIcon = computed(() =>{
      switch (props.layoutMode) {
        case 'table':
          return 'table';
        case 'card':
          return 'view-stacked';
        case 'wizard':
          return 'file-earmark-code';
      }
      return 'pencil';
    })

    return { layoutModeIcon }
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
.toggle_left {
  border-bottom-left-radius: 25px;
  border-top-left-radius: 25px;
  border: none;
  box-shadow: 0 .5rem 1rem rgba(0,0,0,.10)!important;
}
.toggle_right {
  border-bottom-right-radius: 25px;
  border-top-right-radius: 25px;
  border: none;
  box-shadow: 0 .5rem 1rem rgba(0,0,0,.10)!important;
}
</style>
