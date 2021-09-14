<template>
  <li class="list-group-item">
    <button type="button" class="btn btn-default btn-sm" @click="$emit('edit')">
      <span class="bi bi-pencil" aria-hidden="true"></span> Edit
    </button>
    <button type="button" class="btn btn-default btn-sm" @click="$emit('delete')">
      <span class="bi bi-trash" aria-hidden="true"></span> Delete
    </button>
  </li>
  <li class="list-group-item">
    <div class="row">
      <div class="col-4 d-flex justify-content-start text-break text-start">
        name:
      </div>
      <div class="col-8 d-flex justify-content-start text-break">
        {{ modelValue.name }}
      </div>
    </div>
  </li>
  <li class="list-group-item">
    <div class="row">
      <div class="col-4 d-flex justify-content-start text-break text-start">
        species:
      </div>
      <div class="col-8 d-flex justify-content-start text-break text-start">
        {{ modelValue.species }}
      </div>
    </div>
  </li>
</template>

<script>
import { defineComponent } from "vue";

export default defineComponent({
  name: "PetView",
  props: {
    modelValue: Object,
  },
  emits: ['edit',  'delete']
});
</script>