<template>
  <div class="row">
    <div class="col-12 d-flex justify-content-between">
      Pets:
      <button type="button" class="btn btn-default btn-sm" @click="addNewItem()">
        <span class="bi bi-plus" aria-hidden="true"></span> Add
      </button>
    </div>
    <div class="col-12 mb-2" v-for="item in modelValue" :key="item.id">
      <pet-edit v-model="item.value" @canceled="finishEditing()" @saved="finishEditing()" v-if="editedPetId === item.id"></pet-edit>
      <pet-view v-model="item.value" @delete="deleteItem(item)" @edit="editItem(item)" v-else></pet-view>
    </div>
    <div class="col-12 mb-2">
      <pet-edit v-model="newItem" @canceled="cancelAdd" @saved="saveNewItem()" v-if="newItem != null"></pet-edit>
    </div>
  </div>
</template>


<script>
import { defineComponent, ref } from 'vue';
import PetView from '../components/PetView.vue';
import PetEdit from '../components/PetEdit.vue';

export default defineComponent({
  components: { PetView, PetEdit },
  name: 'PetList',
  props: {
    modelValue: Object
  },
  setup(props, context) {
    const editedPetId = ref(-1);
    const newItem = ref(null);


    const editItem = function(item) {
      editedPetId.value = item.id;
    };

    const deleteItem = function(item){
      if (confirm(`Are you sure to delete this pet?`)){
        context.emit('update:modelValue', props.modelValue.filter(i => i.id !== item.id));
      }
    };

    const finishEditing = function() {
      editedPetId.value = -1;
    };

    const addNewItem = function() {
      newItem.value = {
        name: '',
        species: ''
      };
    };

    const saveNewItem = function() {
        const items = [...props.modelValue];
        items.push({id: items[items.length-1].id + 1, value: newItem.value});
        context.emit('update:modelValue', items);
        newItem.value = null;
    };

    const cancelAdd = function() {
        newItem.value = null;
    };

    return { editedPetId, newItem, editItem, deleteItem, finishEditing, addNewItem, saveNewItem, cancelAdd };
  }
});
</script>