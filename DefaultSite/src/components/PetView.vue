<template>
  <div class="card col-md-12">
    <div class="row">
      <div class="col-12 d-flex justify-content-end">
        <button type="button" class="btn btn-default btn-sm"  @click="$emit('edit')">
          <span class="bi bi-pencil" aria-hidden="true"></span> Edit
        </button>
        <button type="button" class="btn btn-default btn-sm"  @click="$emit('delete')">
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
        </ul>
      </div>
    </div>
  </div>
</template>

<script>
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'PetView',
  props: {
    modelValue: Object
  },
});
</script>