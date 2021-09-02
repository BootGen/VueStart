<template>
  <div class="row">
    <div class="col-12 d-flex justify-content-start">
      Pets:
    </div>
    <div class="col-12 mb-2" v-for="item in modelValue" :key="item.id">
      <pet-edit v-model="item.value" @close="finishEditing()" v-if="editedPetId === item.id"></pet-edit>
      <pet-view v-model="item.value" @delete="deleteItem(item)" @edit="editItem(item)" v-else></pet-view>
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
  setup() {
    const editedPetId = ref(-1)
    return { editedPetId }
  },
  /*watch: {
    petList: {
      handler() {
        console.log("pet changed!");
        this.$emit('update:modelValue', this.petList.map(i => i.value))
      },
      deep: true
    }
  },*/
  methods: {
    editItem(item){
      this.editedPetId = item.id;
    },
    deleteItem(item){
      if (confirm(`Are you sure to delete Pet with name ${ item.value.name }?`)){
        this.$emit('update:modelValue', this.modelValue.filter(i => i.id !== item.id))
      }
    },
    finishEditing() {
      this.editedPetId = -1;
    }
  }
});
</script>