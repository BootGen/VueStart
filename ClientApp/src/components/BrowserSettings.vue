<template>
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="staticBackdropLabel">Settings</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close" @click="cancel"></button>
      </div>
      <div class="modal-body">
        <h5>Frontend mode</h5>
        <select class="form-control" aria-label="Select frontend" v-model="newFrontend">
          <option value="vanilla">
                <span class="bi bi-pencil" aria-hidden="true"></span>Vanilla</option>
          <option value="bootstrap">Bootstrap</option>
          <option value="tailwind">Tailwind</option>
        </select>
        <hr>
        <h5>Editable</h5>
        <select class="form-select" aria-label="Editable" v-model="newEditable">
          <option :value="true">Editable</option>
          <option :value="false">Read-only</option>
        </select>
        <hr>
        <h5>Theme color</h5>
        <div class="d-flex align-center">
          <input type="color" class="form-control form-control-color" id="colorInput" v-model="newColor" title="Choose your color"  @click="triggerColorPicker">
          <p class="p-2">{{ newColor }}</p>
        </div>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal" @click="$emit('cancel')">Cancel</button>
        <button type="button" class="btn btn-primary" @click="$emit('save', newFrontend, newEditable, newColor)">Save</button>
      </div>
    </div>
  </div>
</template>

<script>
import { defineComponent, ref, watch } from 'vue';

export default defineComponent({
  name: 'BrowserSettings',
  emits: ['cancel', 'save'],
  props: {
    frontendMode: String,
    editable: Boolean,
    color: String
  },
  setup(props, context) {
    const newFrontend = ref(props.frontendMode);
    const newEditable = ref(props.editable);
    const newColor = ref(props.color);

    watch(() => [props.frontendMode, props.editable, props.color], () => {
      newFrontend.value = props.frontendMode;
      newEditable.value = props.editable;
      newColor.value = props.color;
    });

    function triggerColorPicker() {
      document.getElementById("colorInput").click();
    }
    return { newFrontend, newEditable, newColor, triggerColorPicker }
  }
});
</script>

<style scoped>
.btn-primary {
  background-color: #42b983;
  border-color: #42b983;
}
.btn-primary:hover {
  background-color: #17a062;
  border-color: #17a062;
}
.btn-secondary {
  background-color: transparent;
  color: black;
}
.btn-secondary:hover {
  background-color: #42b98310;
}
.modal-dialog {
  width: 500px;
}
@media (max-width: 576px) {
  .modal-dialog {
    width: auto;
  }
}
</style>