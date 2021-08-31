<template>
  <div class="card">
    <div class="row">
      <div class="col-12 d-flex justify-content-end">
        <button type="button" class="btn btn-default btn-sm" @click="$emit('edit')">
          <span class="bi bi-pencil" aria-hidden="true"></span> Edit
        </button>
        <button type="button" class="btn btn-default btn-sm" @click="$emit('delete')">
          <span class="bi bi-trash" aria-hidden="true"></span> Delete
        </button>
      </div>
    </div>
    <div class="row">
      <div class="col-12">
        <ul class="list-group list-group-flush">
          <li class="list-group-item">
            <div class="row">
              <div class="col-4 d-flex justify-content-start text-break text-start">
                UserName:
              </div>
              <div class="col-8 d-flex justify-content-start text-break text-start">
                {{ modelValue.userName }}
              </div>
            </div>
          </li>
          <li class="list-group-item">
            <div class="row">
              <div class="col-4 d-flex justify-content-start text-break text-start">
                Email:
              </div>
              <div class="col-8 d-flex justify-content-start text-break text-start">
                {{ modelValue.email }}
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
import { defineComponent } from 'vue';
import PetList from '../components/PetList.vue';

export default defineComponent({
  components: { PetList },
  name: 'UserView',
  props: {
    modelValue: Object
  },
  methods: {
    onUpdate(pets) {
      const newValue = { ...this.modelValue }
      newValue.pets = pets
      this.$emit('update:modelValue', newValue)
    }
  }
});
</script>
