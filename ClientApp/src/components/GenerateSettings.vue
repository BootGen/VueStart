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
            <input class="form-check-input" type="radio" name="vanilla" id="vanilla" :checked="editedSettings.frontend === 'vanilla'" @click="editedSettings.frontend = 'vanilla'">
            <label class="form-check-label" for="vanilla">
              Vanilla
            </label>
          </div>
          <div class="col-4 form-check">
            <input class="form-check-input" type="radio" name="bootstrap" id="bootstrap" :checked="editedSettings.frontend === 'bootstrap'" @click="editedSettings.frontend = 'bootstrap'">
            <label class="form-check-label" for="bootstrap">
              Bootstrap
            </label>
          </div>
          <div class="col-4 form-check">
            <input class="form-check-input" type="radio" name="tailwind" id="tailwind" :checked="editedSettings.frontend === 'tailwind'" @click="editedSettings.frontend = 'tailwind'">
            <label class="form-check-label" for="tailwind">
              Tailwind
            </label>
          </div>
        </div>
        <hr>
        <h5>Editable</h5>
        <div class="row px-3">
          <div class="col-4 form-check">
            <input class="form-check-input" type="radio" name="editable" id="editable" :checked="!editedSettings.isReadonly" @click="editedSettings.isReadonly = false">
            <label class="form-check-label" for="editable">
              Editable
            </label>
          </div>
          <div class="col-4 form-check">
            <input class="form-check-input" type="radio" name="read-only" id="read-only" :checked="editedSettings.isReadonly" @click="editedSettings.isReadonly = true">
            <label class="form-check-label" for="read-only">
              Read-only
            </label>
          </div>
        </div>
        <hr>
        <h5>Theme color</h5>
        <div class="d-flex align-center">
          <input type="color" class="form-control form-control-color" id="colorInput" v-model="editedSettings.color" title="Choose your color"  @click="triggerColorPicker">
          <p class="p-2">{{ editedSettings.color }}</p>
        </div>
        <hr>
        <h5>Table settings</h5>
        <div class="row px-3">
          <div class="col-4 form-check" v-for="myClass in editedSettings.classSettings" :key="myClass.name">
            <input class="form-check-input" type="radio" :name="myClass.name" :id="myClass.name" :checked="myClass.name === selectedClass.name" @click="selectClass(myClass)">
            <label class="form-check-label" :for="myClass.name">
              {{ myClass.name }}
            </label>
          </div>
        </div>
        <table class="table text-start mt-2 mb-4" v-if="selectedClass">
          <thead>
            <tr class="table-light">
              <th scope="col"></th>
              <th scope="col" v-for="myClass in selectedClass.propertySettings" :key="myClass.name">{{ myClass.name }}</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <th scope="row">Visible Name</th>
              <td class="align-middle" v-for="myClass in selectedClass.propertySettings" :key="myClass.name">
                <div class="col-12 d-flex justify-content-start">
                  <input type="text" class="form-control" placeholder="Visible Name" v-model="myClass.visibleName">
                </div>
              </td>
            </tr>
            <tr>
              <th scope="row">Is Read Only</th>
              <td class="align-middle" v-for="myClass in selectedClass.propertySettings" :key="myClass.name">
                <div class="col-12 d-flex justify-content-start">
                  <div class="form-check">
                    <input class="form-check-input" type="checkbox" v-model="myClass.isReadOnly" >
                  </div>
                </div>
              </td>
            </tr>
            <tr>
              <th scope="row">Is Hidden</th>
              <td class="align-middle" v-for="myClass in selectedClass.propertySettings" :key="myClass.name">
                <div class="col-12 d-flex justify-content-start">
                  <div class="form-check">
                    <input class="form-check-input" type="checkbox" v-model="myClass.isHidden">
                  </div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal" @click="cancel">Cancel</button>
        <button type="button" class="btn btn-primary" @click="save">Save</button>
      </div>
    </div>
  </div>
</template>

<script>
import { defineComponent, ref, watch } from 'vue';

export default defineComponent({
  name: 'BrowserSettings',
  emits: ['close', 'update:modelValue'],
  props: {
    modelValue: Object
  },
  setup(props, context) {
    const editedSettings = ref({  });
    const selectedClass = ref(props.modelValue.classSettings[0]);

    function resetSettings() {
      editedSettings.value = { ...props.modelValue };
      editedSettings.value.color = '#' + editedSettings.value.color;
      selectedClass.value = editedSettings.value.classSettings[0];
    }
    resetSettings();

    watch(props, () => {
      resetSettings();
    });
    function selectClass(newClass) {
      selectedClass.value = newClass;
    }
    function triggerColorPicker() {
      document.getElementById("colorInput").click();
    }
    function cancel() {
      editedSettings.value = { ...props.modelValue };
      context.emit('close');
    }
    function save() {

      context.emit('update:modelValue', { ...editedSettings.value, color: editedSettings.value.color.substr(1) });
      context.emit('close');
    }
    return { editedSettings, selectedClass, triggerColorPicker, selectClass, save, cancel }
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
  width: 80vw;
}
@media (min-width: 576px) {
  .modal-dialog {
    max-width: unset;
  }
}
</style>