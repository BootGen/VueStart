<template>
  <div class="h-100">
    <div class="row justify-content-center align-items-center jumbotron" :class="{ 'landing': !showNav, 'content' : showNav, }">
      <div class="col-8">
        <h1>Start Vue!</h1>
        <div class="jumbo-text" :class="{ 'landing': !showNav, 'content' : showNav, }">
          <p class="lead text-justify">
            Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.
          </p>
          <button class="btn btn-outline-primary rounded-pill" @click="clickBtn()">Go!</button>
        </div>
      </div>
      <div class="col-4">
        <vueuen type="default"></vueuen>
      </div>
    </div>
    <div class="position-absolute end-50 codemirror" :class="{ 'landing': !showNav, 'content' : showNav, }">
      <code-mirror></code-mirror>
    </div>
    <div class="position-absolute start-50 browser" :class="{ 'landing': !showNav, 'content' : showNav, }">
      <browser-frame></browser-frame>
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
    transition: height 1s ease-in-out;
  }
  .jumbotron.landing {
    height: 100%;
  }
  .jumbotron.content {
    height: 10%;
  }
  .jumbo-text{
    transition: all 1s ease-in-out;
    overflow: hidden;
  }
  .jumbo-text.content{
    opacity: 0;
    top: 10rem;
  }
  .jumbo-text.landing{
    opacity: 1;
    top: 80rem;
  }
  .codemirror{
    width: 40%;
    margin: 5%;
    transition: all 1s ease-in-out;
    overflow: hidden;
  }
  .codemirror.content{
    opacity: 1;
    top: 10rem;
  }
  .codemirror.landing{
    opacity: 0;
    top: 80rem;
  }
  .browser{
    width: 40%;
    margin: 5%;
    transition: all 1s ease-in-out;
    transition-delay: 300ms;
     overflow: hidden;
  }
  .browser.content{
    opacity: 1;
    top: 10rem;
  }
  .browser.landing{
    opacity: 0;
    top: 80rem;
  }
</style>
