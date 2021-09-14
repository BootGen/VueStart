<template>
  <div class="container-fluid">
    <user-list v-model="userList"></user-list>
  </div>
</template>

<script>
import { defineComponent, ref } from 'vue';
import UserList from './components/UserList.vue';

function toDataModel(data) {
  return data.map((val, idx) => {
    const v = { ... val }
    for (const property in v) {
      if(Array.isArray(v[property])){
        v[property] = toDataModel(v[property]);
      }
    }
    return {
      id: idx,
      value: v
    }
  })
}
function toSimpleArray(data) {
  let arr =  data.map(val => {
    return { ...val.value }
  })
  arr.forEach(item => {
    for (const property in item) {
      if(Array.isArray(item[property])){
        item[property] = toSimpleArray(item[property]);
      }
    }
  })
  return arr;
}

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
    const userList = ref(toDataModel(users))

    const saveToLocalStorage = function(newValue) {
        let minimized = JSON.stringify(newValue);
        let oldValue = localStorage.getItem('json');
        if (minimized != oldValue) {
          localStorage.setItem('json', minimized);
        }
    }
    return { userList, saveToLocalStorage }
  },
  mounted(){
    window.addEventListener('storage', () => {
      const users = JSON.parse(localStorage.getItem('json')).users;
      this.userList = toDataModel(users);
    });
  },
  watch: {
    userList: {
      handler() {
        this.saveToLocalStorage({users: toSimpleArray(this.userList)});
      },
      deep: true
    },  
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
