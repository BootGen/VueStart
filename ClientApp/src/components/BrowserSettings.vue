<template>
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="staticBackdropLabel">Settings</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close" @click="cancel"></button>
      </div>
      <div class="modal-body">
        <h5>Frontend mode</h5>
        <div class="row px-3">
          <div class="col-4 form-check">
            <input class="form-check-input" type="radio" name="vanilla" id="vanilla" :checked="newFrontend == 'vanilla'" @click="newFrontend = 'vanilla'">
            <label class="form-check-label" for="vanilla">
              Vanilla
            </label>
          </div>
          <div class="col-4 form-check">
            <input class="form-check-input" type="radio" name="bootstrap" id="bootstrap" :checked="newFrontend == 'bootstrap'" @click="newFrontend = 'bootstrap'">
            <label class="form-check-label" for="bootstrap">
              Bootstrap
            </label>
          </div>
          <div class="col-4 form-check">
            <input class="form-check-input" type="radio" name="tailwind" id="tailwind" :checked="newFrontend == 'tailwind'" @click="newFrontend = 'tailwind'">
            <label class="form-check-label" for="tailwind">
              Tailwind
            </label>
          </div>
        </div>
        <hr>
        <h5>Editable</h5>
        <div class="row px-3">
          <div class="col-4 form-check">
            <input class="form-check-input" type="radio" name="editable" id="editable" :checked="newEditable" @click="newEditable = true">
            <label class="form-check-label" for="editable">
              Editable
            </label>
          </div>
          <div class="col-4 form-check">
            <input class="form-check-input" type="radio" name="read-only" id="read-only" :checked="!newEditable" @click="newEditable = false">
            <label class="form-check-label" for="read-only">
              Read-only
            </label>
          </div>
        </div>
        <hr>
        <h5>Theme color</h5>
        <div class="d-flex align-center">
          <input type="color" class="form-control form-control-color" id="colorInput" v-model="newColor" title="Choose your color"  @click="triggerColorPicker">
          <p class="p-2">{{ newColor }}</p>
        </div>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal" @click="cancel">Cancel</button>
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
    function cancel() {
      newFrontend.value = props.frontendMode;
      newEditable.value = props.editable;
      newColor.value = props.color;
      context.emit('cancel');
    }
    return { newFrontend, newEditable, newColor, triggerColorPicker, cancel }
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
.form-check-input:checked {
  background-color: #42b983;
  border-color: #42b983;
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