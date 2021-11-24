<template>
  <div>
    <div class="download-panel-browser custom-card shadow pt-1">
      <div class="d-flex browser-nav py-1">
        <button type="button" class="btn-site inactive w-auto" :class="{ 'border-bottom-right' : downloadMode == downloadModes.HTMLCSSJS }">
          <div class="d-flex align-items-center">
          </div>
        </button>
        <tab :title="downloadModes.HTMLCSSJS" icon="pencil" :showVr="downloadMode != downloadModes.HTMLCSSJS && downloadMode != downloadModes.VueSPA" @select="changeGeneratedMode(downloadModes.HTMLCSSJS)" :class="{ 'inactive': downloadMode != downloadModes.HTMLCSSJS, 'active' : downloadMode == downloadModes.HTMLCSSJS, 'border-bottom-right' : downloadMode == downloadModes.VueSPA }"></tab>
        <tab :title="downloadModes.VueSPA" icon="eye" :showVr="downloadMode != downloadModes.VueSPA && downloadMode != downloadModes.NuxtSPA" @select="changeGeneratedMode(downloadModes.VueSPA)" :class="{ 'inactive': downloadMode != downloadModes.VueSPA, 'active' : downloadMode == downloadModes.VueSPA, 'border-bottom-right' : downloadMode == downloadModes.NuxtSPA, 'border-bottom-left' : downloadMode == downloadModes.HTMLCSSJS }"></tab>
        <tab :title="downloadModes.NuxtSPA" icon="file-earmark-code" :showVr="downloadMode != downloadModes.NuxtSPA" @select="changeGeneratedMode(downloadModes.NuxtSPA)" :class="{ 'inactive': downloadMode != downloadModes.NuxtSPA, 'active' : downloadMode == downloadModes.NuxtSPA, 'border-bottom-left' : downloadMode == downloadModes.VueSPA }"></tab>
        <button type="button" class="btn-site inactive" :class="{ 'border-bottom-left' : downloadMode == downloadModes.NuxtSPA }"><span class="bi bi-plus" aria-hidden="true"></span></button>
      </div>
    </div>
    <div class="col-12 download-panel-content">
      <div class="d-flex py-4" v-if="downloadMode == downloadModes.HTMLCSSJS">
        <div class="col-6">
          <h5>Type:</h5>
          <div class="form-check">
            <input class="form-check-input" type="radio" name="typeOptions" id="typeOptions1" value="Editor" :checked="selectedType == 'editor'">
            <label class="form-check-label" for="typeOptions1">Editor</label>
          </div>
          <div class="form-check">
            <input class="form-check-input" type="radio" name="typeOptions" id="typeOptions2" value="View" :checked="selectedType == 'view'">
            <label class="form-check-label" for="typeOptions2">View</label>
          </div>
          <div class="form-check">
            <input class="form-check-input" type="radio" name="typeOptions" id="typeOptions3" value="Form" :checked="selectedType == 'form'">
            <label class="form-check-label" for="typeOptions3">Form</label>
          </div>
        </div>
        <div class="col-6">
          <h5>Layout:</h5>
          <div class="form-check">
            <input class="form-check-input" type="radio" name="modeOptions" id="modeOptions1" value="Card" :checked="selectedMode == 'card'">
            <label class="form-check-label" for="modeOptions1">Card</label>
          </div>
          <div class="form-check">
            <input class="form-check-input" type="radio" name="modeOptions" id="modeOptions2" value="Accordion" :checked="selectedMode == 'accordion'">
            <label class="form-check-label" for="modeOptions2">Accordion</label>
          </div>
          <div class="form-check">
            <input class="form-check-input" type="radio" name="modeOptions" id="modeOptions3" value="Table" :checked="selectedMode == 'table'">
            <label class="form-check-label" for="modeOptions3">Table</label>
          </div>
        </div>
      </div>
      <p class="fw-lighter fst-italic pt-4" v-if="downloadMode != downloadModes.HTMLCSSJS">Coming soon!</p>
      <div class="p d-flex">
        <button class="btn fill-btn rounded-pill m-1 btn" @click="$emit('close')">Cancel</button>
        <button class="btn fill-btn rounded-pill m-1 btn" :class="{ 'disabled' : downloadMode != downloadModes.HTMLCSSJS }" @click="$emit('download', selectedType, selectedMode)">Download</button>
      </div>
    </div>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
import Tab from '../components/Tab.vue'

export default defineComponent({
  name: 'DownloadPanel',
  components: { Tab },
  props: {
    generateType: String,
    layoutMode: String
  },
  setup(props) {
    const downloadModes = {
      VueSPA: 'Vue SPA',
      NuxtSPA: 'Nuxt SPA',
      HTMLCSSJS: 'HTML/CSS/JS',
    }
    const downloadMode = ref(downloadModes.HTMLCSSJS);
    const selectedType = ref(props.generateType);
    const selectedMode = ref(props.layoutMode);

    function changeGeneratedMode(type) {
      downloadMode.value = type;
    }
    return { downloadModes, downloadMode, selectedType, selectedMode, changeGeneratedMode }
  }
});

</script>

<style scoped>
.download-panel-content{
  background: #fff;
  margin-top: -1rem;
  padding: 1rem;
}
.download-panel-browser{
    z-index: 9;
    border-radius: 5px;
    background-color: #42b983;
    transition: all 1s ease-in-out;
    overflow: hidden;
    vertical-align: bottom;
    width: 100%;
}
.form-check-input:checked {
    background-color: #42b983;
    border-color: #42b983;
}
</style>