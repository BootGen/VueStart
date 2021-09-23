<template>
  <div class="col-12 h-100 container">
    <div class="row">
      <div class="column left">
        <span class="dot" style="background:#ED594A;"></span>
        <span class="dot" style="background:#FDD800;"></span>
        <span class="dot" style="background:#5AC05A;"></span>
      </div>
      <div class="column middle">
        <input type="text" value="https://yourownwebsite.com">
      </div>
      <div class="column right">
        <div style="float:right">
          <span class="bar"></span>
          <span class="bar"></span>
          <span class="bar"></span>
        </div>
      </div>
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
    modelValue: String,
  },
  setup(props) {
    const frameA = ref(true);
    const urlA = ref(props.modelValue);
    const urlB = ref("");
    onMounted(function(){
      watch(props, function(){
        if (frameA.value) {
          urlB.value = props.modelValue
          const onLoad = function() {
              document.getElementById('frameB').onload = undefined;
              frameA.value = false;
          }
          document.getElementById('frameB').onload = onLoad;
        } else {
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
/* The browser window */
.container {
  border-top-left-radius: 4px;
  border-top-right-radius: 4px;
  background-color: #f1f1f1;
  padding: 10px;
}

/* Container for columns and the top "toolbar" */
.row {
  padding: 10px;
  margin: 0;
  height: 50px;
  background: #f1f1f1;
  border-top-left-radius: 4px;
  border-top-right-radius: 4px;
}

/* Create three unequal columns that floats next to each other */
.column {
  float: left;
}

.left {
  width: 15%;
}

.right {
  width: 10%;
}

.middle {
  width: 75%;
}

/* Clear floats after the columns */
.row:after {
  content: "";
  display: table;
  clear: both;
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
  border-radius: 3px;
  border: none;
  background-color: white;
  margin-top: -8px;
  height: 25px;
  color: #666;
  padding: 5px;
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
  height: clac(100% - 50px);
  padding: 0!important;
  height: calc(100% - 50px);
}

.hidden {
  display: none;
}

</style>
