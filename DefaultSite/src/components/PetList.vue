<template>
  <div class="row">
    <div class="col-12 d-flex justify-content-between">
      Pets:
      <button type="button" class="btn btn-default btn-sm" @click="addNewItem()">
        <span class="bi bi-plus" aria-hidden="true"></span> Add
      </button>
    </div>
    <div class="col-12 mb-2" v-for="item in modelValue" :key="item.id">
        <div class="card">
          <ul class="list-group list-group-flush">
            <pet-edit v-model="item.value" @canceled="finishEditing()" @saved="finishEditing()" v-if="editedItemId === item.id"></pet-edit>
            <pet-view v-model="item.value" @delete="deleteItem(item)" @edit="editItem(item)" v-else></pet-view>
          </ul>
        </div>
    </div>
    <div class="col-12 mb-2" v-if="newItem != null">
        <div class="card">
          <ul class="list-group list-group-flush">
            <pet-edit v-model="newItem" @canceled="cancelAdd" @saved="saveNewItem()"></pet-edit>
          </ul>
        </div>
    </div>
  </div>
</template>


<script>
import { defineComponent, ref } from 'vue';
import PetView from './PetView.vue';
import PetEdit from './PetEdit.vue';

export default defineComponent({
  components: { PetView, PetEdit },
  name: 'PetList',
  props: {
    modelValue: Object
  },
  emits: ['update:modelValue'],
  setup(props, context) {
    const editedItemId = ref(-1);
    const newItem = ref(null);

    function editItem(item) {
      editedItemId.value = item.id;
    }
    function deleteItem(item) {
      if (confirm('Are you sure to delete this pet?')){
        context.emit('update:modelValue', props.modelValue.filter(i => i.id !== item.id));
      }
    }
    function finishEditing() {
      editedItemId.value = -1;
    }
    function addNewItem() {
      newItem.value = {
        name: '',
        species: ''
      };
    }
    function saveNewItem() {
        const items = [...props.modelValue];
        const lastId = items.length == 0 ? 0 : items[items.length-1].id;
        items.push({id: lastId + 1, value: newItem.value});
        context.emit('update:modelValue', items);
        newItem.value = null;
    }
    function cancelAdd() {
        newItem.value = null;
    }

    return { editedItemId, newItem, editItem, deleteItem, finishEditing, addNewItem, saveNewItem, cancelAdd };
  }
});
</script>