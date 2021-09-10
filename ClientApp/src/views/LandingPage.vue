<template>
  <div class="container h-100">
    <div class="row justify-content-center align-items-center anim" :class="{ 'landing': !showNav, 'content' : showNav, }">
      <div class="col-8">
        <h1>Start Vue!</h1>
        <div class="opacityAnim" :class="{ 'landing': !showNav, 'content' : showNav, }">
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
    <div class="col-6 h-100 opacityAnim" :class="{ 'landing': showNav, 'content' : !showNav, }">
      <code-mirror></code-mirror>
    </div>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
import Vueuen from '../components/Vueuen.vue';
import CodeMirror from '../components/CodeMirror.vue';

export default defineComponent({
  name: 'LandingPage',
  components: { Vueuen, CodeMirror },
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
  .anim {
    transition: height 1s ease-in-out;
  }
  .anim.landing {
    height: 100%;
  }
  .anim.content {
    height: 10%;
  }
  .opacityAnim{
    transition: all 1s ease-in-out;
    overflow: hidden;
  }
  .opacityAnim.content{
    opacity: 0;
    height: 0;
  }
  .opacityAnim.landing{
    opacity: 1;
    height: 20rem;
  }
</style>
