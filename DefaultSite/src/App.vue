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
    return { userList }
  },
  mounted(){
    window.addEventListener('storage', () => {
      const users = JSON.parse(localStorage.getItem('json')).users;
      const newUserList = toDataModel(users);
      this.setUserList(newUserList);
    });
  },
  watch: {
    userList: {
      handler() {
        saveToLocalStorage({users: toSimpleArray(this.userList)});
      },
      deep: true
    },  
  },
  methods: {
    setUserList(list) {
      if(JSON.stringify(list) != JSON.stringify(this.userList)){
        this.userList = [...list];
      }
    },
    saveToLocalStorage(newValue) {
      try {
        let obj = JSON.parse(newValue);
        let minimized = JSON.stringify(obj);
        let oldValue = localStorage.getItem('json');
        if (minimized != oldValue) {
          localStorage.setItem('json', minimized);
        }
      } catch (e) {
        console.log(e)
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
