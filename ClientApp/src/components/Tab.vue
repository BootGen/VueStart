<template>
  <button type="button" class="btn-site" @click="$emit('select')" aria-label="tab">
    <div class="d-flex">
      <span v-if="icon" :class="`bi bi-${icon}`" aria-hidden="true"></span>
      <img v-else-if="img == 'tailwind_green'" alt="tailwind green" src="../assets/tailwind_green.webp" height="25">
      <img v-else-if="img == 'tailwind_white'" alt="tailwind white" src="../assets/tailwind_white.webp" height="25">
      <img v-else-if="img == 'bootstrap_green'" alt="bootstrap green" src="../assets/bootstrap_green.webp" height="25">
      <img v-else-if="img == 'bootstrap_white'" alt="bootstrap white" src="../assets/bootstrap_white.webp" height="25">
      <img v-else-if="img == 'vanilla_green'" alt="vanilla green" src="../assets/css_green.webp" height="25">
      <img v-else-if="img == 'vanilla_white'" alt="vanilla white" src="../assets/css_white.webp" height="25">
      <span class="px-2">{{ title }}</span>
    </div>
  </button>
</template>

<script>
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'Tab',
  props: {
    icon: String,
    img: String,
    title: String
  },
  setup() {}
});

</script>

<style>
  .vr {
    height: 20px;
    width: 1px;
    background-color: #eee;
  }
  .btn-site {
    width: 30%;
    padding: unset;
    border: unset;
    display: flex;
    justify-content: space-between;
    padding: 5px;
    padding-left: 10px;
    align-items: center;
    cursor: default!important;
  }
  .btn-site.active {
    background-color: #fff;
    color: #42b983!important;
    border-top-left-radius: 10px;
    border-top-right-radius: 10px;
  }
  .btn-site.inactive {
    background-color: #42b983;
    color: #fff!important;
  }
  .btn-site.plus {
    width: max-content;
    padding: 5px 20px 5px 10px;
  }
  .border-bottom-left {
    border-bottom-left-radius: 10px;
  }
  .border-bottom-right {
    border-bottom-right-radius: 10px;
  }

  @media (max-width: 576px) {
    .btn-site {
      font-size: 10pt;
      padding-left: 7px;
    }
    .px-2 {
      padding-right: 0.25rem!important;
      padding-left: 0.25rem!important;
    }
  }
</style>
