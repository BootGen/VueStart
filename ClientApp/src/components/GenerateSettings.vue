<template>
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <h3 class="modal-title" id="staticBackdropLabel">Settings</h3>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close" @click="cancel"></button>
      </div>
      <div class="modal-body">
        <h5>Frontend mode</h5>
        <div class="row px-3">
          <div class="col-4 form-check cursor-pointer" @click="editedSettings.frontend = 'vanilla'">
            <img v-if="editedSettings.frontend == 'vanilla'" alt="vanilla" src="../assets/css.webp" height="50">
            <img v-else alt="vanilla" src="../assets/css_disabled.webp" height="50">
            <span class="ms-2">Vanilla</span>
          </div>
          <div class="col-4 form-check cursor-pointer" @click="editedSettings.frontend = 'bootstrap'">
            <img v-if="editedSettings.frontend == 'bootstrap'" alt="bootstrap" src="../assets/bootstrap.webp" height="50">
            <img v-else alt="bootstrap" src="../assets/bootstrap_disabled.webp" height="50">
            <span class="ms-2">Bootstrap</span>
          </div>
          <div class="col-4 form-check cursor-pointer" @click="editedSettings.frontend = 'tailwind'">
            <img v-if="editedSettings.frontend == 'tailwind'" alt="tailwind" src="../assets/tailwind.webp" height="50">
            <img v-else alt="tailwind" src="../assets/tailwind_disabled.webp" height="50">
            <span class="ms-2">Tailwind</span>
          </div>
        </div>
        <h5 class="mt-4">Editable</h5>
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
        <h5 class="mt-4">Theme color</h5>
        <div class="d-flex align-center">
          <input type="color" class="form-control form-control-color" id="colorInput" v-model="editedSettings.color" title="Choose your color" @click="triggerColorPicker">
          <label for="colorInput" class="p-2">{{ editedSettings.color }}</label>
        </div>
        <h5 class="mt-4">Table settings</h5>
        <ul class="nav nav-tabs">
          <li class="nav-item" v-for="myClass in editedSettings.classSettings" :key="myClass.name">
            <a class="nav-link" :class="{'active': myClass.name === selectedClass.name}" @click="selectClass(myClass)">{{ myClass.name }}</a>
          </li>
        </ul>
        <table class="table text-start" v-if="selectedClass">
          <thead>
            <tr class="table-light">
              <td scope="col">&nbsp;</td>
              <th scope="col" v-for="myClass in selectedClass.propertySettings" :key="myClass.name">{{ myClass.name }}</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <th scope="row">Visible Name</th>
              <td class="align-middle" v-for="myClass in selectedClass.propertySettings" :key="myClass.name">
                <div class="col-12 d-flex justify-content-start">
                  <input type="text" class="form-control" placeholder="Visible Name" v-model="myClass.visibleName" :aria-label="myClass.name">
                </div>
              </td>
            </tr>
            <tr v-if="!editedSettings.isReadonly">
              <th scope="row">Is Read Only</th>
              <td class="align-middle" v-for="myClass in selectedClass.propertySettings" :key="myClass.name">
                <div class="col-12 d-flex justify-content-start" v-if="myClass.isReadOnly != null">
                  <div class="form-check">
                    <input class="form-check-input" type="checkbox" v-model="myClass.isReadOnly" :aria-label="myClass.name">
                  </div>
                </div>
              </td>
            </tr>
            <tr>
              <th scope="row">Is Hidden</th>
              <td class="align-middle" v-for="myClass in selectedClass.propertySettings" :key="myClass.name">
                <div class="col-12 d-flex justify-content-start">
                  <div class="form-check">
                    <input class="form-check-input" type="checkbox" v-model="myClass.isHidden" :aria-label="myClass.name">
                  </div>
                </div>
              </td>
            </tr>
            <tr>
              <th scope="row">Show As Image</th>
              <td class="align-middle" v-for="myClass in selectedClass.propertySettings" :key="myClass.name">
                <div class="col-12 d-flex justify-content-start" v-if="myClass.showAsImage != null">
                  <div class="form-check">
                    <input class="form-check-input" type="checkbox" v-model="myClass.showAsImage" :aria-label="myClass.name">
                  </div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div class="alert alert-warning error-alert-container mx-2" v-if="errorMessage">
        <div class="text-center">
          {{ errorMessage }} <br>
        </div>
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
  emits: ['cancel', 'save', 'update:modelValue'],
  props: {
    modelValue: Object
  },
  setup(props, context) {
    const editedSettings = ref({  });
    const selectedClass = ref(props.modelValue.classSettings[0]);
    const errorMessage = ref('');

    function resetSettings() {
      editedSettings.value = JSON.parse(JSON.stringify(props.modelValue));
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
      resetSettings();
      context.emit('cancel');
    }
    function save() {
      if(checkVisisbleNameIsEmpty()) {
        context.emit('update:modelValue', { ...editedSettings.value, color: editedSettings.value.color.substr(1) });
        context.emit('save');
        errorMessage.value = '';
      } else {
        errorMessage.value = 'The visible name cannot be empty.';
      }
    }
    function checkVisisbleNameIsEmpty() {
      for(let i = 0; i < editedSettings.value.classSettings.length; i++) {
        for(let j = 0; j < editedSettings.value.classSettings[i].propertySettings.length; j++) {
          if(editedSettings.value.classSettings[i].propertySettings[j].visibleName == ''){
            return false;
          }
        }
      }
      return true;
    }
    return { editedSettings, selectedClass, errorMessage, triggerColorPicker, selectClass, save, cancel }
  }
});
</script>

<style scoped>
.nav-item {
  cursor: pointer;
}
.nav-tabs {
  border-bottom: 1px solid #42b983;
}
.nav-link.active {
  color: #42b983;
  border-color: #42b983 #42b983 transparent;
}
.nav-link {
  color: unset;
}
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
.modal-header {
  border-bottom: unset;
}
.modal-footer {
  border-top: unset;
}
.table>:not(caption)>*>* {
  border-bottom-width: 0;
}
@media (min-width: 576px) {
  .modal-dialog {
    max-width: unset;
  }
}
</style>