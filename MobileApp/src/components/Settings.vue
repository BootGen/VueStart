<template>
  <div class="fab-icon-holder hamb active" @click="toggleSettings">
    <span :class="showSetting ? 'bi bi-x-lg' : 'bi bi-gear'" aria-hidden="true"></span>
  </div>
  <div class="offcanvas offcanvas-end" :class="{'show': showSetting}" data-bs-scroll="true">
    <div class="offcanvas-header">
      <h5 class="offcanvas-title">Settings</h5>
    </div>
    <div class="offcanvas-body">
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
          <input class="form-check-input" type="radio" name="editable" id="editable" :checked="!editedSettings.isReadonly" @click="editedSettings.isReadonly = fale">
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
      <div class="d-flex align-items-center pb-4">
        <input type="color" class="form-control form-control-color" id="colorInput" v-model="color" title="Choose your color"  @click="triggerColorPicker">
        <p class="m-0 px-2">{{ color }}</p>
      </div>
    </div>
  </div>
  
</template>

<script>
import { defineComponent, ref, watch } from 'vue';

export default defineComponent({
  name: "Settings",
  props: {
    generateSettings: Object
  },
  emits: ['save'],
  setup(props, context) {
    const editedSettings = ref(props.generateSettings);
    const showSetting = ref(false);
    const color = ref('#' + editedSettings.value.color);
    
    watch(() => [props.generateSettings.frontend, props.generateSettings.isReadonly, props.generateSettings.color], () => {
      editedSettings.value = props.generateSettings;
      color.value = '#' + editedSettings.value.color;
    });
    function triggerColorPicker() {
      document.getElementById("colorInput").click();
    }
    function openSettings(){
      showSetting.value = true;
    }
    function closeSettings(){
      showSetting.value = false;
      context.emit('save', { ...editedSettings.value, color: color.value.substr(1) });
    }
    function toggleSettings() {
      if (showSetting.value)
        closeSettings();
      else
        openSettings();
    }
    return { editedSettings, color, showSetting, triggerColorPicker, toggleSettings, closeSettings }
  }  
})
</script>

<style>
.offcanvas-body {
  overflow-y: unset;
}
.form-check-input:checked {
  background-color: #42b983;
  border-color: #42b983;
}
.hamb {
  width: max-content;
  position: fixed;
  z-index: 9999;
  right: 0;
  top: .5rem;
}
.copied {
  transition: all 1s ease-in-out;
  transition-delay: 10ms;
  opacity: 0;
  color: #42b983;
  display: flex;
  justify-content: center;
}
.offcanvas {
  transition: all .5s ease-in-out;
  visibility: hidden;
}
.offcanvas.show {
  visibility: visible;
}
</style>