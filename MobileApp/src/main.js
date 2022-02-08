import App from './App.vue'
import { createApp } from 'vue'
import axios from 'axios';


const app = createApp(App);
app.mount('#app')

app.config.errorHandler = function (err, vm, info) {
  axios.post('api/errors', { ...err, info, vm })
};

window.onerror = function(event, source, lineno, colno, error) {
  axios.post('api/errors', { ...error, event, lineno, colno, source })
 };