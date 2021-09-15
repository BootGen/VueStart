<template>
  <div class="h-100">
    <div class="d-flex justify-content-center align-items-center jumbotron" :class="{ 'landing': !showNav, 'content' : showNav }">
      <div class="col-8">
        <h1>Start Vue!</h1>
        <div class="jumbo-text" :class="{ 'landing': !showNav, 'content' : showNav }">
          <p class="lead text-justify">
            Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.
          </p>
          <button class="btn btn-outline-primary rounded-pill" @click="clickBtn()">Go!</button>
        </div>
        <div class="slogen-text" :class="{ 'landing': !showNav, 'content' : showNav }">
          <p class="lead text-justify">
            Lorem Ipsum is simply dummy text of the printing and typesetting industry.
          </p>
        </div>
      </div>
      <div class="col-4">
        <vueuen type="default"></vueuen>
      </div>
    </div>
    <div class="position-absolute end-50 codemirror custom-card" :class="{ 'landing': !showNav, 'content' : showNav, }">
      <code-mirror></code-mirror>
    </div>
    <div class="position-absolute start-50 browser custom-card" :class="{ 'landing': !showNav, 'content' : showNav, }">
      <browser-frame></browser-frame>
    </div>
    <div class="d-flex position-absolute footer" :class="{ 'landing': !showNav, 'content' : showNav, }">
      <p>Powered by <a href="https://bootgen.com" target="_blank">BootGen</a> | Created by <a href="https://codesharp.hu" target="_blank">Code Sharp Kft.</a></p>
    </div>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
import Vueuen from '../components/Vueuen.vue';
import CodeMirror from '../components/CodeMirror.vue';
import BrowserFrame from '../components/BrowserFrame.vue'

export default defineComponent({
  name: 'LandingPage',
  components: { Vueuen, CodeMirror, BrowserFrame },
  setup() {
    const showNav = ref(false);
    return { showNav }
  },
  methods: {
    clickBtn() {
      this.showNav = !this.showNav;
    }
  }
});

</script>

<style scoped>
  .btn {
    font-size: 1.5em;
  }
  .jumbotron {
    transition: all 1s ease-in-out;
    padding: 5vmin;
  }
  .jumbotron.landing {
    height: calc( 100vh - 2.5rem );
    background-color: white;
    transition-delay: 300ms;
    border-bottom-left-radius: 0;
  }
  .jumbotron.content {
    height: 15vh;
    background-color: #f1f1f1;
    border-bottom-left-radius: 20px;
  }
  .jumbo-text{
    transition: all 1s ease-in-out;
    overflow: hidden;
  }
  .jumbo-text.content{
    opacity: 0;
    height: 0rem;
  }
  .jumbo-text.landing{
    opacity: 1;
    height: 10rem;
  }
  .slogen-text{
    transition: all 1s ease-in-out;
    overflow: hidden;
  }
  .slogen-text.content{
    opacity: 1;
    height: 100%;
    visibility: visible;
  }
  .slogen-text.landing{
    opacity: 0;
    height: 0;
    visibility: hidden;
  }
  .codemirror{
    width: 46%;
    margin: 2%;
    transition: all 1s ease-in-out;
    transition-delay: 150ms;
    overflow: hidden;
  }
  .codemirror.content{
    opacity: 1;
    height: calc( 80vh - 2.5rem );
    top: 15vh;
    visibility: visible;
  }
  .codemirror.landing{
    opacity: 0;
    height: 0vh;
    top: 95vh;
    visibility: hidden;
  }
  .browser{
    width: 46%;
    margin: 2%;
    transition: all 1s ease-in-out;
    overflow: hidden;
    vertical-align: bottom;
  }
  .browser.content{
    opacity: 1;
    height: calc( 80vh - 2.5rem );
    transition-delay: 300ms;
    top: 15vh;
    visibility: visible;
  }
  .browser.landing{
    opacity: 0;
    height: 0vh;
    top: 95vh;
    visibility: hidden;
  }
  .footer{
    right: 0;
    margin: 2%;
    transition: all 1s ease-in-out;
    overflow: hidden;
    vertical-align: bottom;
  }
  .footer.content{
    opacity: 1;
    height: 2.5rem;
    transition-delay: 500ms;
    top: calc( 100vh - 5rem );
    visibility: visible;
  }
  .footer.landing{
    opacity: 0;
    height: 0vh;
    top: 95vh;
    visibility: hidden;
  }
</style>
