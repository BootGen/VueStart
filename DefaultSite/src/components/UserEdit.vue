<template>
  <div class="card">
    <div class="row">
      <div class="col-12 d-flex justify-content-end">
        <button type="button" class="btn btn-default btn-sm" @click="save">
          <span class="bi bi-save" aria-hidden="true"></span> Save
        </button>
        <button type="button" class="btn btn-default btn-sm" @click="cancel">
          <span class="bi bi-backspace" aria-hidden="true"></span> Cancel
        </button>
      </div>
    </div>
    <div class="row">
      <div class="col-12">
        <ul class="list-group list-group-flush">
          <li class="list-group-item">
            <div class="row d-flex align-items-center">
              <div class="col-4 d-flex justify-content-start text-break text-start">
                UserName:
              </div>
              <div class="col-8 d-flex justify-content-start">
                <input
                  v-model="editedUser.userName"
                  type="text"
                  class="form-control"
                  id="userName"
                />
              </div>
            </div>
          </li>
          <li class="list-group-item">
            <div class="row d-flex align-items-center">
              <div class="col-4 d-flex justify-content-start text-break text-start">
                Email:
              </div>
              <div class="col-8 d-flex justify-content-start">
                <input
                  v-model="editedUser.email"
                  type="text"
                  class="form-control"
                  id="email"
                />
              </div>
            </div>
          </li>
          <li class="list-group-item">
            <pet-list :modelValue="modelValue.pets" @update:modelValue="onUpdate"></pet-list>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script>
import { defineComponent, ref } from "vue";
import PetList from '../components/PetList.vue';

export default defineComponent({
  components: { PetList },
  name: 'UserView',
  props: {
    modelValue: Object
  },
  setup(props, context) {
    const editedUser = ref({ ...props.modelValue });

    const save = function() {
      context.emit('update:modelValue', editedUser.value)
      context.emit('saved')
    };
    
    const cancel = function() {
      editedUser.value = { ...props.modelValue };
      context.emit('canceled')
    };
    
    return { editedUser, save, cancel }
  }
});
</script>