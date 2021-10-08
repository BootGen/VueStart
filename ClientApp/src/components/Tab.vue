<template>
  <button type="button" class="btn-site" @click="$emit('select')">
    <div class="d-flex">
      <span :class="`bi bi-${icon}`" aria-hidden="true"></span>
      <span class="px-2">{{ title }}</span>
    </div>
    <div class="d-flex align-items-center">
      <span class="bi bi-x px-2" aria-hidden="true"></span>
      <div class="vr" v-if="showVr"></div>
    </div>
  </button>
</template>

<script>
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'Tab',
  props: {
    icon: String,
    title: String,
    showVr: Boolean
  },
  setup() {}
});

</script>

<style>
  .vr {
    height: 20px;
    width: 1px;
    background-color: #61C7FD;
  }
  .btn-site {
    width: 100%;
    padding: unset;
    border: unset;
    display: flex;
    justify-content: space-between;
    padding: 5px;
    padding-left: 15px;
    align-items: center;
  }
  .btn-site.active {
    background-color: #fff;
    color: #61C7FD!important;
    border-top-left-radius: 10px;
    border-top-right-radius: 10px;
  }
  .btn-site.inactive {
    background-color: #265388;
    color: #61C7FD!important;
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
</style>
