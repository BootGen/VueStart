<template>
  <div class="row">
    <div class="col-12 d-flex justify-content-between">
      Users:
      <button type="button" class="btn btn-default btn-sm" @click="addNewItem()">
        <span class="bi bi-plus" aria-hidden="true"></span> Add
      </button>
    </div>
    <div class="col-xxl-3 col-md-6 col-sm-12">
      <div class="m-2" v-for="item in modelValue" :key="item.id">
        <div class="card">
          <ul class="list-group list-group-flush">
            <user-edit v-model="item.value" @canceled="finishEditing()" @saved="finishEditing()" v-if="editedItemId === item.id"></user-edit>
            <user-view v-model="item.value" @delete="deleteItem(item)" @edit="editItem(item)" v-else></user-view>
            <li class="list-group-item">
              <pet-list v-model="item.value.pets"></pet-list>
            </li>
          </ul>
        </div>
      </div>
      <div class="col-12 mb-2" v-if="newItem != null">
          <div class="card">
            <ul class="list-group list-group-flush">
              <user-edit v-model="newItem" @canceled="cancelAdd" @saved="saveNewItem()"></user-edit>
            </ul>
          </div>
      </div>
    </div>
  </div>
</template>


<script>
import { defineComponent, ref } from 'vue';
import UserView from '../components/UserView.vue';
import UserEdit from '../components/UserEdit.vue';
import PetList from '../components/PetList.vue';

export default defineComponent({
  components: { UserView, UserEdit, PetList },
  name: 'UserList',
  props: {
    modelValue: Object
  },
  emits: ['update:modelValue'],
  setup(props, context) {
    const editedItemId = ref(-1);
    const newItem = ref(null);

    const editItem = function(item){
      editedItemId.value = item.id;
    };
    
    const deleteItem = function(item){
      if (confirm('Are you sure to delete this user?')) {
        console.log(props.modelValue)
        context.emit('update:modelValue', props.modelValue.filter(i => i.id !== item.id))
      }
    };
    
    const finishEditing = function() {
      editedItemId.value = -1;
    };

    const addNewItem = function() {
      newItem.value = {
        userName: '',
        email: '',
        pets: []
      };
    };

    const saveNewItem = function() {
        const items = [...props.modelValue];
        const lastId = items.length == 0 ? 0 : items[items.length-1].id;
        items.push({id: lastId + 1, value: newItem.value});
        context.emit('update:modelValue', items);
        newItem.value = null;
    };

    const cancelAdd = function() {
        newItem.value = null;
    };

    return { editedItemId, newItem, editItem, deleteItem, finishEditing, addNewItem, saveNewItem, cancelAdd };
  }
});
</script>