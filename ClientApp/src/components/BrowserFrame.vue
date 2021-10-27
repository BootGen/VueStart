<template>
  <div class="col-12 browser-frame">
    <div class="d-flex browser-nav py-1">
      <button type="button" class="btn-site inactive w-auto" :class="{ 'border-bottom-right' : borderRadius }">
        <div class="d-flex align-items-center px-1 ">
          <span class="dot" style="background:#ED594A;"></span>
          <span class="dot" style="background:#FDD800;"></span>
          <span class="dot" style="background:#5AFF5A;"></span>
        </div>
      </button>
      <slot></slot>
    </div>
    <div class="d-flex justify-content-around align-items-center menu">
      <div class="d-flex">
        <span class="bi bi-arrow-left px-2 icon" aria-hidden="true"></span>
        <span class="bi bi-arrow-right px-2 icon" aria-hidden="true"></span>
        <span class="bi bi-arrow-clockwise px-2 icon" aria-hidden="true"></span>
      </div>
      <div class="middle">
        <input type="text" value="https://yourownwebsite.com">
      </div>
      <span class="bi bi-three-dots-vertical px-2 icon" aria-hidden="true"></span>
    </div>
    <div class="col-12 content">
      <iframe id="frameA" class="h-100 w-100" :class="{hidden: !frameA}" :src="urlA" title="CodeSharp"></iframe>
      <iframe id="frameB" class="h-100 w-100" :class="{hidden: frameA}" :src="urlB" title="CodeSharp"></iframe>
    </div>
  </div>
</template>

<script>
import { defineComponent, watch, ref, onMounted } from 'vue';

export default defineComponent({
  name: 'BrowserFrame',
  props: {
    borderRadius: Boolean,
    modelValue: String,
  },
  setup(props) {
    const frameA = ref(true);
    const urlA = ref(props.modelValue);
    const urlB = ref("");
    onMounted(function(){
      watch(props, function(){
        if (frameA.value) {
          if (urlA.value === props.modelValue)
            return;
          urlB.value = props.modelValue
          const onLoad = function() {
              document.getElementById('frameB').onload = undefined;
              frameA.value = false;
          }
          document.getElementById('frameB').onload = onLoad;
        } else {
          if (urlB.value === props.modelValue)
            return;
          urlA.value = props.modelValue
          const onLoad = function() {
              document.getElementById('frameA').onload = undefined;
              frameA.value = true;
          }
          document.getElementById('frameA').onload = onLoad;
        }
      })
    })

    return {frameA, urlA, urlB}
  }
});
</script>

<style scoped>
.browser-nav {
  position: relative;
  margin-bottom: 5px;
  z-index: 99;
}
.browser-frame{
  height: calc( 100% - 3rem);
}
.menu {    
  background-color: #fff;
  position: absolute;
  width: 100%;
  top: 20px;
  z-index: 9;
  padding: 5px;
  padding-top: 25px;
  border-bottom: solid 1px #eee;
}
.icon {
  color: #42b983;
}
.middle {
  width: 100%;
}

/* Three dots */
.dot {
  margin: 4px;
  height: 12px;
  width: 12px;
  background-color: #bbb;
  border-radius: 50%;
  display: inline-block;
}

/* Style the input field */
input[type=text] {
  width: 100%;
  border-radius: 15px;
  border: none;
  background-color: #eee;
  margin-top: -8px;
  height: 25px;
  color: #42b983;
  padding: 5px;
  padding-left: 15px;
}

/* Three bars (hamburger menu) */
.bar {
  width: 17px;
  height: 3px;
  background-color: #aaa;
  margin: 3px 0;
  display: block;
}

/* Page content */
.content {
  border-radius: 5px;
  height: calc( 100% - 1rem);
  margin-top: 25px;
}

.hidden {
  display: none;
}

</style>
