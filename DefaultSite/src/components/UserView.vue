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
</template>

<script>
import { defineComponent } from "vue";

export default defineComponent({
  name: "UserView",
  props: {
    modelValue: Object,
  }
});
</script>
