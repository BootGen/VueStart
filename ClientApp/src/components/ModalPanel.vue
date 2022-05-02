<template>
  <div class="modal-panel-container" :class="{ 'hide': !modelValue, 'show' : modelValue }">
    <div class="modal-panel-bg" @click="$emit('update:modelValue', false)"></div>
    <div class="modal-panel" :class="{ 'hide': !modelValue, 'show' : modelValue }">
      <slot></slot>
    </div>
  </div>
</template>

<script>
export default {
  name: "ModalPanel",
  props: {
    modelValue: Boolean
  },
  emits: ['update:modelValue']
}
</script>

<style scoped>
.modal-panel{
  transition: all 0.5s ease-in-out;
  width: max-content;
  margin: 1rem auto;
  z-index: 9999;
  position: relative;
}
.modal-panel.show{
  opacity: 1;
}
.modal-panel.hide{
  opacity: 0;
  visibility: hidden;
  margin-top: 100%;
}
.modal-panel-container {
  transition: all 0.5s ease-in-out;
  position: fixed;
  width: 100vw;
  height: 100vh;
  top: 0;
  background-color: rgba(0, 0, 0, 0.1);
  z-index: 999;
  display: flex;
  justify-content: center;
  align-items: center;
}
.modal-panel-container.show{
  opacity: 1;
}
.modal-panel-container.hide{
  opacity: 0;
  visibility: hidden;
}
.modal-panel-bg {
  position: fixed;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.1);
}
</style>