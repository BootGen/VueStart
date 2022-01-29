<template>
  <div class="download-panel custom-card shadow">
    <div class="d-flex justify-content-between align-items-center">
      <h5>Download</h5>
      <button type="button" class="m-1 btn close-btn" @click="$emit('close')"><span class="bi bi-x px-2" aria-hidden="true"></span></button>
    </div>
    <div class="d-flex pt-1 flex-wrap btn-container">
      <div class="d-flex flex-column align-items-center p-2 px-4">
        <img class="logo img-fluid" alt="vue" src="../assets/html_css_js_logo.webp">
        <button class="btn fill-btn rounded-pill m-1 btn" @click="$emit('download')">Tradition Webpage</button>
        <p>With Vue 3 from CDN</p>
      </div>
      <div class="d-flex flex-column align-items-center p-2 px-4">
        <img class="logo img-fluid" alt="vue" src="../assets/vue_coming_soon.webp">
        <button class="btn fill-btn rounded-pill m-1 btn" disabled>NPM project</button>
        <p>With standard Vue tooling</p>
      </div>
      <div class="d-flex flex-column align-items-center p-2 px-4">
        <img class="logo img-fluid" alt="vue" src="../assets/nuxt_coming_soon.webp">
        <button class="btn fill-btn rounded-pill m-1 btn" disabled>Nuxt project</button>
        <p>The Intuitive Vue Framework</p>
      </div>
    </div>
  </div>
</template>

<script>
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'DownloadPanel'
});

</script>

<style scoped>
.download-panel{
  background: #fff;
  padding: 1rem;
  border-radius: 5px;
}
.logo {
  height: 150px;
  width: 150px;
  object-fit: contain;
  margin: 1rem;
}
.close-btn {
  font-size: 1.5rem;
  color: #42b983!important;
  padding: 0;
}
p {
  font-style: italic;
}
@media (max-width: 768px) {
  .btn-container {
    flex-direction: column;
  }
  .logo {
    height: 90px;
  }
}
    
</style>