import App from './App.vue'
import router from './router'
import { createStore } from 'vuex'
import { createApp } from 'vue'

const store = createStore({
    state () {
      return {
        vuecoonType: 'default'
      }
    },
    mutations: {
      setType (state, type) {
        state.vuecoonType = type
      }
    }
})

Vue.config.errorHandler = function (err, vm, info) {
  console.log (err, vm, info)
}

createApp(App).use(store).use(router).mount('#app')
