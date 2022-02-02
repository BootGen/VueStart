<template>
  <div class="col-8 supporters-page">
    <h1>The Team</h1>
    <div class="row">
    <div class="col-2">
      <git-hub-user username="echopot"></git-hub-user>
    </div>
      <div class="col-2">
        <git-hub-user username="agabor"></git-hub-user>
      </div>
    </div>
    <h1>Thank You for Your Support!</h1>
    <div class="row">
      <div class="col-2" v-for="username in userNames" :key="username">
        <git-hub-user :username="username"></git-hub-user>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import { defineComponent, reactive } from 'vue';
import GitHubUser from "@/components/GitHubUser";
export default defineComponent({
  name: "Supporters",
  components: {GitHubUser},
  setup() {
    const userNames = reactive([]);
    axios.get('https://api.github.com/repos/BootGen/VueStart/stargazers').then(resp => {
      const data = resp.data;
      for (let user of data) {
        userNames.push(user.login)
      }
    });
    return {userNames}
  }
})
</script>

<style scoped>
.supporters-page {
  margin-top: 30px;
  margin-left: auto;
  margin-right: auto;
}
</style>