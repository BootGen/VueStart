<template>
  <div class="container-fluid">
    <user-list v-model="userList"></user-list>
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
  mounted(){
    window.addEventListener('storage', () => {
      const users = JSON.parse(JSON.parse(localStorage.getItem('json'))).users;
      const newUserList = users.map((val, idx) => {
        return {
          id: idx,
          value: val
        }
      })
      this.setUserList(newUserList);
    });
  },
  watch: {
    userList: {
      handler() {
        localStorage.setItem('json', JSON.stringify({users: this.userList.map(item => item.value)}));
      },
      deep: true
    },  
  },
  methods: {
    setUserList(list) {
      if(JSON.stringify(list) != JSON.stringify(this.userList)){
        console.log('list', JSON.stringify(list));
        console.log('this.userList', JSON.stringify(this.userList));
        this.userList = [...list];
        console.log('new this.userList', JSON.stringify(this.userList));
      }
    }
  }
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
