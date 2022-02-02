<template>
  <a :href="`https://github.com/${username}`" target="_blank">
    <img class="avatar avatar-user" :src="image" width="48" height="48" :alt="username">
  </a>&nbsp;
  <a :href="`https://github.com/${username}`" target="_blank">{{name}}</a>
</template>

<script>
import axios from "axios";
import { defineComponent, ref } from 'vue';

export default defineComponent({
  name: "GitHubUser",
  props: {
    username: String
  },
  setup(props) {
    const image = ref('');
    const name = ref(props.username);
    axios.get(`https://api.github.com/users/${props.username}`).then(resp => {
      const data = resp.data;
      image.value = data.avatar_url;
      if (data.name)
        name.value = data.name;
    });
    return { image, name }
  }
})
</script>

<style scoped>
.avatar-user {
  border-radius: 50% !important;
}
</style>