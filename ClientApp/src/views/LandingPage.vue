<template>
  <div class="container h-100">
    <transition name="fade">
      <div class="row justify-content-center align-items-center anim" :class="{ 'toShrink': ! showNav, 'toGrow' : showNav, }">
        <div class="col-6">
          <h1>Start Vue!</h1>
          <div>
            <p class="lead text-justify">
              Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.
            </p>
            <button class="btn btn-outline-primary rounded-pill" @click="clickBtn()">Go!</button>
          </div>
        </div>
        <div class="col-6">
          <vueuen type="default"></vueuen>
        </div>
      </div>
    </transition>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
import Vueuen from '../components/Vueuen.vue'

export default defineComponent({
  components: { Vueuen },
  name: 'LandingPage',
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
  .anim.toShrink {
    height: 100%;
  }
  .anim.toGrow {
    height: 45%;
  }
</style>
