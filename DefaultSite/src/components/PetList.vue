<template>
  <div class="row">
    <div class="col-12 d-flex justify-content-start">
      Pets:
    </div>
    <div class="col-12 mb-2" v-for="item in petList" :key="item.id">
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
  setup(props) {
    const editedPetId = ref(-1)
    const petList = ref(props.modelValue.map((val, idx) => {
      return {
        id: idx,
        value: val
      }
    }))
    return { editedPetId, petList }
  },
  methods: {
    editItem(item){
      this.editedPetId = item.id;
    },
    deleteItem(item){
      if (confirm(`Are you sure to delete Pet with name ${ item.value.name }?`)){
        this.petList = this.petList.filter(i => i.id !== item.id);
        this.$emit('update:modelValue', this.petList.map(i => i.value))
      }
    },
    finishEditing() {
      this.editedPetId = -1;
    }
  }
});
</script>