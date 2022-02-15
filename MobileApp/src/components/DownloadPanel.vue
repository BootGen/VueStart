<template>
  <div class="row fixed-bottom download-panel custom-card shadow m-2">
    <div class="col-12 align-items-center">
      <div class="row">
        <h5 class="col-10">Download</h5>
        <button type="button" class="col-2 btn close-btn text-end" @click="$emit('close')"><span class="bi bi-x px-2" aria-hidden="true"></span></button>
      </div>
    </div>
    <div class="col-12 justify-content-center">
      <div class="row">
        <div class="col-lg-4 col-md-4 col-sm-4 d-flex flex-column align-items-center">
          <img class="logo img-fluid" alt="vue" src="../assets/html_css_js_logo.webp">
          <button class="btn fill-btn rounded-pill m-1 btn" @click="$emit('download')">Traditional Webpage</button>
          <p class="text-center">With Vue 3 from CDN</p>
        </div>
        <div class="col-lg-4 col-md-4 col-sm-4 d-flex flex-column align-items-center">
          <img class="logo img-fluid" alt="vue" src="../assets/vue_coming_soon.webp">
          <button class="btn fill-btn rounded-pill m-1 btn" disabled>NPM project</button>
          <p class="text-center">With standard Vue tooling</p>
        </div>
        <div class="col-lg-4 col-md-4 col-sm-4 d-flex flex-column align-items-center">
          <img class="logo img-fluid" alt="vue" src="../assets/nuxt_coming_soon.webp">
          <button class="btn fill-btn rounded-pill m-1 btn" disabled>Nuxt project</button>
          <p class="text-center">The Intuitive Vue Framework</p>
        </div>
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
  height: 90px;
  width: 90px;
  object-fit: contain;
}
.close-btn {
  font-size: 1.5rem;
  color: #42b983!important;
  padding: 0;
}
p {
  font-style: italic;
}
    
</style>