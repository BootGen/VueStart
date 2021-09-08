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
    <nav aria-label="Page navigation example">
      <ul class="pagination">
        <li class="page-item">
          <button type="button" class="page-link" v-if="page != 0" @click="page--"> Previous </button>
        </li>
        <li v-for="(page, idx) in Math.ceil(modelValue.length/5)" :key="idx" class="page-item" @click="selectedPage = idx">
          <a class="page-link">{{ idx+1 }}</a>
        </li>
        <li class="page-item">
          <button type="button" class="page-link" v-if="Math.ceil(modelValue.length/5)" @click="page++"> Next </button>
        </li>
      </ul>
    </nav>
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
  setup() {
    const editedPetId = ref(-1);
    const newItem = ref(null);
    const selectedPage = 0;
    return { editedPetId, newItem, selectedPage };
  },
  methods: {
    editItem(item){
      this.editedPetId = item.id;
    },
    deleteItem(item){
      if (confirm(`Are you sure to delete Pet with name ${ item.value.name }?`)){
        this.$emit('update:modelValue', this.modelValue.filter(i => i.id !== item.id));
      }
    },
    finishEditing() {
      this.editedPetId = -1;
    },
    addNewItem() {
      this.newItem = {
        name: '',
        species: ''
      };
    },
    saveNewItem() {
        const items = [...this.modelValue];
        items.push({id: items[items.length-1].id + 1, value: this.newItem});
        this.$emit('update:modelValue', items);
        this.newItem = null;
    },
    cancelAdd() {
        this.newItem = null;
    }
  }
});
</script>