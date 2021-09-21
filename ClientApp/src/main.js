import App from './App.vue'
import router from './router'
import { createStore } from 'vuex'
import { createApp } from 'vue'

const store = createStore({
    state () {
      return {
        vueuenType: 'default'
      }
    },
    mutations: {
      setType (state, type) {
        state.vueuenType = type
      }
    }
})

createApp(App).use(store).use(router).mount('#app')
