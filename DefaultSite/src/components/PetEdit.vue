<template>
  <li class="list-group-item">
    <button type="button" class="btn btn-default btn-sm" @click="save">
      <span class="bi bi-save" aria-hidden="true"></span> Save
    </button>
    <button type="button" class="btn btn-default btn-sm" @click="cancel">
      <span class="bi bi-backspace" aria-hidden="true"></span> Cancel
    </button>
  </li>
  <li class="list-group-item">
    <div class="row d-flex align-items-center">
      <div class="col-4 d-flex justify-content-start text-break text-start">
        name:
      </div>
      <div class="col-8 d-flex justify-content-start">
        <input v-model="editedItem.name" type="text" class="form-control" />
      </div>
    </div>
  </li>
  <li class="list-group-item">
    <div class="row d-flex align-items-center">
      <div class="col-4 d-flex justify-content-start text-break text-start">
        species:
      </div>
      <div class="col-8 d-flex justify-content-start">
        <input v-model="editedItem.species" type="text" class="form-control" />
      </div>
    </div>
  </li>
</template>

<script>
import { defineComponent, ref } from "vue";

export default defineComponent({
  name: "PetView",
  props: {
    modelValue: Object,
  },
  emits: ['update:modelValue', 'canceled', 'saved'],
  setup(props, context) {
    const editedItem = ref({ ...props.modelValue });

    const save = function () {
      context.emit("update:modelValue", editedItem.value);
      context.emit("saved");
    };

    const cancel = function () {
      editedItem.value = { ...props.modelValue };
      context.emit("canceled");
    };

    return { editedItem, save, cancel };
  },
});
</script>