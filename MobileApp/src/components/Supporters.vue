<template>
  <div class="col-lg-8 col-md-12 supporters-page">
    <div class="d-flex flex-column align-items-center">
      <p>Staring our project on  GitHub may seem like a small thing, but it really keeps us motivated. We thank you for your support!</p>
      <button class="btn fill-btn rounded-pill m-1 btn-lg" @click="back">Back</button>
    </div>
    <h3>The Team</h3>
    <div class="row">
      <div class="col-lg-6 col-md-6 col-sm-12">
        <git-hub-user username="echopot"></git-hub-user>
      </div>
      <div class="col-lg-6 col-md-6 col-sm-12">
        <git-hub-user username="agabor"></git-hub-user>
      </div>
    </div>
    <h3>Supporters</h3>
    <div class="row">
      <div class="col-xxl-10 col-xl-12">
        <div class="row">
          <div class="col-lg-1 col-md-2 col-sm-2 col-3" v-for="user in users" :key="user.username">
            <div class="usertag">
              <a :href="`https://github.com/${user.username}`" target="_blank">
                <img class="avatar avatar-user" :src="user.avatar" width="48" height="48" :alt="user.username">
              </a>
              <span class="usertagtext">{{user.username}}</span>
            </div>
          </div>
        </div>
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
    const users = reactive([]);
    axios.get('https://api.github.com/repos/BootGen/VueStart/stargazers').then(resp => {
      const data = resp.data;
      for (let user of data) {
        users.push({username: user.login, avatar: user.avatar_url})
      }
    });
    function back() {
      window.scrollTo(0, 0);
      history.back()
    }
    return {users, back}
  }
})
</script>

<style scoped>
h3 {
  margin-top: 30px;
  margin-bottom: 30px;
}
.avatar-user {
  border-radius: 50% !important;
  margin-bottom: 10px;
}
.supporters-page {
  margin-top: 30px;
  margin-left: auto;
  margin-right: auto;
  padding: 20px;
}
.usertag {
  position: relative;
  display: inline-block;
}

.usertag .usertagtext {
  visibility: hidden;
  width: 120px;
  background-color: black;
  color: #fff;
  text-align: center;
  border-radius: 6px;
  padding: 5px 0;
  position: absolute;
  z-index: 1;
  bottom: 150%;
  left: 50%;
  margin-left: -60px;
}

.usertag .usertagtext::after {
  content: "";
  position: absolute;
  top: 100%;
  left: 50%;
  margin-left: -5px;
  border-width: 5px;
  border-style: solid;
  border-color: black transparent transparent transparent;
}

.usertag:hover .usertagtext {
  visibility: visible;
}
</style>