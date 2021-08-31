<template>
  <div class="container-fluid">
    <div class="d-flex flex-wrap" v-for="item in userList" :key="item.id">
      <user-list :modelValue="userList"></user-list>
    </div>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
import UserList from './components/UserList.vue';

export default defineComponent({
  name: 'App',
  components: { UserList },
  setup: function () {
    const users = [
      {
        userName: 'Jon',
        email: 'jon@arbuckle.com',
        pets: [
          {
            name: 'Garfield',
            species: 'cat',
          },
          {
            name: 'Odie',
            species: 'dog',
          }
        ]
      }
    ]
    const userList = ref(users.map((val, idx) => {
      return {
        id: idx,
        value: val
      }
    }))
    return { userList }
  },
});
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
</style>
