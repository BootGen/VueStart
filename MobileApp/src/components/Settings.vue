<template>
  <div class="fab-icon-holder hamb active" @click="cancel" data-bs-toggle="offcanvas" data-bs-target="#offcanvasWithBothOptions" aria-controls="offcanvasWithBothOptions">
    <span class="bi bi-list" aria-hidden="true"></span>
  </div>

  <div class="offcanvas offcanvas-end" data-bs-scroll="true" tabindex="-1" id="offcanvasWithBothOptions" aria-labelledby="offcanvasWithBothOptionsLabel">
    <div class="offcanvas-header">
      <h5 class="offcanvas-title" id="offcanvasWithBothOptionsLabel">Settings</h5>
    </div>
    <div class="offcanvas-body">
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
      <div class="d-flex align-items-center pb-4">
        <input type="color" class="form-control form-control-color" id="colorInput" v-model="newColor" title="Choose your color"  @click="triggerColorPicker">
        <p class="m-0 px-2">{{ newColor }}</p>
      </div>
      <div class="fab-icon-holder w-100 active" @click="$emit('save', newFrontend, newEditable, newColor)" data-bs-dismiss="offcanvas">
        <span class="bi bi-save" aria-hidden="true"></span>
        <span class="ps-2">Save</span>
      </div>
      <div class="fab-icon-holder w-100 inactive" @click="cancel" data-bs-dismiss="offcanvas">
        <span class="bi bi-backspace" aria-hidden="true"></span>
        <span class="ps-2">Cancel</span>
      </div>
      <div id="share-btn" class="fab-icon-holder w-100 active" @click="share">
        <span class="bi bi-share" aria-hidden="true"></span>
        <span class="ps-2">Share</span>
      </div>
        <span :class="{'disable-share': shareLinkOnClipboard}" class="copied">Copied!</span>
    </div>
  </div>
  
</template>

<script>
import { defineComponent, ref, watch } from 'vue';
import axios from "axios";

export default defineComponent({
  name: "Settings",
  props: {
    frontendMode: String,
    editable: Boolean,
    color: String,
    json: String
  },
  emits: ['save'],
  setup(props, context) {
    const newFrontend = ref(props.frontendMode);
    const newEditable = ref(props.editable);
    const newColor = ref(props.color);
    const shareLinkOnClipboard = ref(false);
    let sharedJson = '';
    let sharedLink = '';

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
    }

    async function share() {
      let shareableJson = JSON.stringify(props.json);
      if(shareableJson !== sharedJson) {
        sharedLink = await axios.post(`api/share/${newFrontend.value}/${newEditable.value}/${newColor.value.slice(1, 7)}`, JSON.parse(props.json));
        sharedJson = shareableJson;
      }
      navigator.clipboard.writeText(window.location.origin + '/' + sharedLink.data.hash);
      shareLinkOnClipboard.value = true;
      setTimeout(()=> {
        shareLinkOnClipboard.value = false;
      }, 800);
    }
    return { newFrontend, newEditable, newColor, shareLinkOnClipboard, triggerColorPicker, cancel, share }
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
.disable-share {
  opacity: 1!important;
}
</style>