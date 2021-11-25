import App from './App.vue'
import { createStore } from 'vuex'
import { createApp } from 'vue'
import axios from 'axios';

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
});

const app = createApp(App);
app.use(store).mount('#app')

app.config.errorHandler = function (err, vm, info) {
  axios.post('api/errors', { ...err, info, vm })
};

window.onerror = function(event, source, lineno, colno, error) {
  axios.post('api/errors', { ...error, event, lineno, colno, source })
 };